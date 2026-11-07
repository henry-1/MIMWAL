
using Microsoft.Win32.SafeHandles;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    [SuppressMessage("SonarAnalyzer.CSharp", "S3869", Justification = "SafeHandle.DangerousGetHandle is required here")]
    public static class MimProcessLauncher
    {
        private static volatile bool _loadUserProfilePrivilegesInitialized = false;
        private static readonly object _loadUserProfilePrivilegesLock = new object();

        private static volatile bool _impersonateUserPrivilegesInitialized = false;
        private static readonly object _impersonateUserPrivilegesLock = new object();

        private const int MaxCapturedBytes = 1024 * 1024 * 1024; // 1GB per stream

        public static PowerShellResult RunPowershellAsService(string psExePath, string programDataFolder, string encodedCommand, int PowerShellTimeout)
        {
            var processResult = new PowerShellResult { StandardOutput = string.Empty, StandardError = string.Empty, ExitCode = 0, TimedOut = false };

            // ------------------------------------------------------------
            // Execute via external PowerShell
            // ------------------------------------------------------------
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = psExePath,
                Arguments = "-NoProfile -NonInteractive -WindowStyle Hidden -ExecutionPolicy Bypass -EncodedCommand " + encodedCommand,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = programDataFolder,
                LoadUserProfile = true
            };

            var stdOutBuilder = new StringBuilder();
            var stdErrBuilder = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo = psi; // ProcessStartInfo Objekt

                // Event-Handler für asynchronous read
                process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    if (e.Data != null) stdOutBuilder.AppendLine(e.Data);
                };
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    if (e.Data != null) stdErrBuilder.AppendLine(e.Data);
                };

                process.Start();

                // start to read results
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // timeout check only when set to a value greater then zero
                if(PowerShellTimeout != 0)
                    processResult.TimedOut = !process.WaitForExit(PowerShellTimeout);

                if (processResult.TimedOut)
                {
                    var msg = string.Format(
                        "PowerShell process (PID {0}) exceeded timeout of {1} ms and is being terminated.", process.Id, PowerShellTimeout);
                    Logger.Instance.WriteError(msg);

                    // kill the process tree
                    KillProcessTree(process.Id);
                    
                    processResult.StandardOutput = string.Empty;                    
                    processResult.StandardError = string.Format(
                        "PowerShell process (PID {0}) exceeded timeout of {1} ms and was terminated.",
                        process.Id, PowerShellTimeout);
                    processResult.ExitCode = -2;
                }
                else
                {
                    // wait for the process to finish and get results back                            
                    process.WaitForExit();
                    processResult.ExitCode = process.ExitCode;
                    processResult.StandardOutput = stdOutBuilder.ToString().Trim();
                    processResult.StandardError = stdErrBuilder.ToString().Trim();
                }
            }

            return processResult;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S107", Justification = "Parameters are all required.")]
        public static PowerShellResult RunPowerShellAsUser(
            string username, string domain, string password,
            string psExePath, string encodedCommand,
            LogOnType requestedLogonType,
            bool loadUserProfile,
            Dictionary<string, string> customEnv,
            int PowershellTimeout)
        {
            var processResult = new PowerShellResult { StandardOutput = string.Empty, StandardError = string.Empty, ExitCode = 0, TimedOut = false };

            SafeTokenHandle tokenHandle = null;
            SafeTokenHandle primaryToken = null;

            NativeMethods.PROFILEINFO profileInfo = new NativeMethods.PROFILEINFO();
            bool profileLoaded = false;            

            try
            {
                EnsureImpersonationPrivileges();                

                // You cannot request to load user profile for network logon
                if (loadUserProfile && (requestedLogonType == LogOnType.LogOnNetwork))
                {
                    processResult.StandardError = "Cannot load user profile for Logon type 'Network'.";
                    Logger.Instance.WriteError(processResult.StandardError);
                    throw new InvalidOperationException(processResult.StandardError);
                }

                // Add privileges to the service token
                if (loadUserProfile && (requestedLogonType != LogOnType.LogOnNetwork))
                {
                    EnsureProfileLoadPrivileges();
                }

                // Try to log on user
                if (!NativeMethods.LogonUser(username, domain, password, requestedLogonType, LogOnProvider.ProviderWinNT50, out tokenHandle))
                {
                    var ex = new Win32Exception(Marshal.GetLastWin32Error());
                    processResult.StandardError = string.Format("Unable to log on user: {0}", ex.Message);
                    System.Diagnostics.Debug.WriteLine(processResult.StandardError);
                    Logger.Instance.WriteError(processResult.StandardError);
                    throw new InvalidOperationException(processResult.StandardError);
                }

                // Create a PRIMARY token explicitly for loading the user profile
                if (loadUserProfile && !NativeMethods.DuplicateTokenEx(tokenHandle, NativeMethods.TOKEN_ALL_ACCESS, IntPtr.Zero,
                    SecurityImpersonationLevel.SecurityImpersonation, TokenType.TokenPrimary, out primaryToken))
                {
                    var ex = new Win32Exception(Marshal.GetLastWin32Error());
                    processResult.StandardError = string.Format("Unable to duplicate user token required to load user profile: {0}", ex.Message);
                    Logger.Instance.WriteError(processResult.StandardError);
                    throw new InvalidOperationException(processResult.StandardError);
                }

                // Load user profile if requested
                if (loadUserProfile)
                    profileLoaded = LoadUserProfile(username, primaryToken, out profileInfo);                

                // Run Powershell
                processResult = RunPowershellProcess(
                    psExePath, 
                    encodedCommand, 
                    tokenHandle,
                    customEnv,
                    PowershellTimeout);               
               
            }
            catch(Exception ex)
            {                               
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Logger.Instance.ReportError(EventIdentifier.RunPowerShellScriptAsUserError, ex);
                processResult.ExitCode = 1;
                processResult.StandardError = ex.Message;         
            }
            finally
            {
                // Clean up handles
                if (profileLoaded)
                {
                    NativeMethods.UnloadUserProfile(primaryToken.DangerousGetHandle(), profileInfo.hProfile);
                }

                if (primaryToken != null) primaryToken.Dispose();                
                if (tokenHandle != null) tokenHandle.Dispose();                
            }
                        
            return processResult;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Aproved.")]
        private static PowerShellResult RunPowershellProcess(
            string psExePath, string encodedCommand, SafeTokenHandle tokenHandle,
            Dictionary<string, string> customEnv, int PowershellTimeout)
        {
            var processResult = new PowerShellResult { StandardOutput = string.Empty, StandardError = string.Empty, ExitCode = 0 , TimedOut = false};
            IntPtr envBlock = IntPtr.Zero;
            bool envBlockIsCustomAllocated = false;
            NativeMethods.PROCESS_INFORMATION pi = new NativeMethods.PROCESS_INFORMATION();
            NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();

            SafeFileHandle hOutRead = null, hOutWrite = null;
            SafeFileHandle hErrRead = null, hErrWrite = null;

            try
            {
                si.cb = Marshal.SizeOf(si);

                CreateAsyncOutputPipe(out hOutRead, out hOutWrite);
                CreateAsyncOutputPipe(out hErrRead, out hErrWrite);

                si.dwFlags = NativeMethods.STARTF_USESTDHANDLES;
                si.hStdOutput = hOutWrite.DangerousGetHandle();
                si.hStdError = hErrWrite.DangerousGetHandle();
                si.hStdInput = IntPtr.Zero;

                string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string commandLine = string.Format("\"{0}\" -NoProfile -NonInteractive -WindowStyle Hidden -ExecutionPolicy Bypass -EncodedCommand {1}", psExePath, encodedCommand);

                // Get the user's default environment block
                IntPtr rawUserEnvBlock;
                if (!NativeMethods.CreateEnvironmentBlock(out rawUserEnvBlock, tokenHandle.DangerousGetHandle(), false))
                {
                    var ex = new Win32Exception(Marshal.GetLastWin32Error());
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    Logger.Instance.ReportError(EventIdentifier.RunPowerShellScriptCreateEnvironmentError, ex);                    
                    throw ex;
                }
                // Merge custom environment block
                if (customEnv != null && customEnv.Count > 0)
                {
                    var merged = ReadEnvironmentBlock(rawUserEnvBlock);
                    NativeMethods.DestroyEnvironmentBlock(rawUserEnvBlock);

                    foreach (var kvp in customEnv)
                    {
                        if (kvp.Value == null) continue; // skip rather than set an empty-string value
                        merged[kvp.Key] = kvp.Value;      // overrides existing keys (e.g. VerbosePreference), adds new ones (e.g. AECRequestId)
                    }

                    envBlock = BuildEnvironmentBlock(merged);
                    envBlockIsCustomAllocated = true;
                }
                else
                {
                    envBlock = rawUserEnvBlock;
                }

                int creationFlags = NativeMethods.CREATE_NO_WINDOW | NativeMethods.CREATE_UNICODE_ENVIRONMENT;

                bool processCreated = NativeMethods.CreateProcessAsUserW(
                    tokenHandle.DangerousGetHandle(),
                    null,
                    commandLine,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    true,
                    creationFlags,
                    envBlock,
                    systemPath,
                    ref si,
                    out pi);

                if (!processCreated)
                {
                    int error = Marshal.GetLastWin32Error();
                    var msg = string.Format("Unable to create new process to run PowerShell.\nWindows Error Number was: {0}", error);
                    var ex = new Win32Exception(
                        error,
                        msg);

                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    Logger.Instance.ReportError(EventIdentifier.RunPowerShellScriptProcessCreationError, ex);
                    throw ex;
                }

                // Close our copies of the write ends - the child holds its own inherited copies now.
                hOutWrite.Dispose(); hOutWrite = null;
                hErrWrite.Dispose(); hErrWrite = null;

                string stdOutResult = null, stdErrResult = null;
                bool stdOutTruncated = false, stdErrTruncated = false;
                Exception stdOutEx = null, stdErrEx = null;

                var outDone = new ManualResetEvent(false);
                var errDone = new ManualResetEvent(false);


                ReadPipeAsync(hOutRead, (text, truncated, ex) =>
                {
                    stdOutResult = text;
                    stdOutTruncated = truncated;
                    stdOutEx = ex;
                    outDone.Set();
                });

                ReadPipeAsync(hErrRead, (text, truncated, ex) =>
                {
                    stdErrResult = text;
                    stdErrTruncated = truncated;
                    stdErrEx = ex;
                    errDone.Set();
                });

                uint dwTimeout = PowershellTimeout > 0 ? (uint)PowershellTimeout : NativeMethods.INFINITE;
                uint waitResult = NativeMethods.WaitForSingleObject(pi.hProcess, dwTimeout);
                int waitError = Marshal.GetLastWin32Error();

                if (waitResult == NativeMethods.WAIT_FAILED)
                    throw new Win32Exception(waitError, "WaitForSingleObject on PowerShell process failed");                

                if (waitResult == NativeMethods.WAIT_TIMEOUT)
                {
                    var msg = string.Format(
                        "PowerShell process (PID {0}) exceeded timeout of {1} ms and is being terminated.",
                        pi.dwProcessId,
                        PowershellTimeout);

                    Logger.Instance.WriteError(msg);

                    KillProcessTree(pi.dwProcessId);

                    processResult.TimedOut = true;
                    processResult.ExitCode = -2;
                    processResult.StandardError = string.Format(
                        "PowerShell process timed out after {0} ms and was terminated.",
                        PowershellTimeout);
                }               

                // Sequential waits, not WaitHandle.WaitAll - avoids the STA-thread restriction
                // some workflow/COM-interop threads are subject to.
                // Termination (or normal exit) closes the child's handles, which triggers EOF on our pipe reads.
                // Bound this wait defensively - it should resolve almost immediately once the process handle signals.
                int drainWaitMs = Math.Max(2000, Math.Min(PowershellTimeout / 10, 30000));
                bool outReady = outDone.WaitOne(drainWaitMs);
                bool errReady = errDone.WaitOne(drainWaitMs);

                if (processResult.TimedOut)
                    throw new FormatException(string.Format("PowerShell process timed out after {0} ms and was terminated.", PowershellTimeout));
                
                int processExitCode = 0;

                if (!NativeMethods.GetExitCodeProcess(
                    pi.hProcess,
                    out processExitCode))
                {
                    int error =
                        Marshal.GetLastWin32Error();

                    throw new Win32Exception(
                        error,
                        "Unable to retrieve PowerShell process exit code.");
                }

                processResult.ExitCode =
                    processExitCode;

                if (!outReady || !errReady)
                {
                    Logger.Instance.WriteError(string.Format(
                        "Pipe read did not complete {0}ms after process exit (PID {1}, stdoutReady={2}, stderrReady={3}) - " +
                        "output may be incomplete.",
                        drainWaitMs, pi.dwProcessId, outReady, errReady));

                    // Being explicit rather than silently treating partial data as complete.
                    if (!outReady) throw new FormatException("[stdout capture incomplete after process exit]");
                    if (!errReady) throw new FormatException("[stderr capture incomplete after process exit]");
                }

                // some exception was thrown in Powershell
                if (stdOutEx != null) throw stdOutEx;

                // in case the output was truncated it cannot be converted back later into clean JSON
                if (stdOutTruncated)
                {
                    var msg = string.Format(CultureInfo.InvariantCulture, "PowerShell stdout exceeded {0} MB and was truncated.", MaxCapturedBytes / 1024 / 1024);
                    Logger.Instance.WriteError(msg);
                    throw new FormatException(msg);
                }

                if (stdErrTruncated)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, "PowerShell stderr exceeded {0} MB and was truncated.", MaxCapturedBytes / 1024 / 1024);
                    Logger.Instance.WriteError(message);
                }

                // get results back either fom pipleline output or from exception
                processResult.StandardOutput = stdOutResult ?? string.Empty;
                processResult.StandardError = stdErrEx != null ? stdErrEx.Message : (stdErrResult ?? string.Empty);

                // clear error string which sometimes contains fragments of powershell output which is not an error
                if (processResult.ExitCode == 0) 
                {
                    processResult.StandardError = string.Empty;                    
                }

                if(!string.IsNullOrEmpty(processResult.StandardError))
                    processResult.StandardError = PowerShellErrorParser.ExtractCleanErrors(processResult.StandardError, psExePath);

                // shorten error string to be able to display it in request history in MIM portal
                if (processResult.StandardError.Length > 850)
                    processResult.StandardError = processResult.StandardError.Substring(0, 800) + " Error message was cutted!";
            }
            catch (Exception ex)
            {
                processResult.ExitCode = -2;
                processResult.StandardError = ex.Message;
                processResult.StandardOutput = string.Empty;
            }
            finally
            {
                if (envBlock != IntPtr.Zero)
                {
                    if (envBlockIsCustomAllocated)
                        Marshal.FreeHGlobal(envBlock);
                    else
                        NativeMethods.DestroyEnvironmentBlock(envBlock);
                }

                if (hOutWrite != null) hOutWrite.Dispose();
                if (hErrWrite != null) hErrWrite.Dispose();
                if (hOutRead != null) hOutRead.Dispose();
                if (hErrRead != null) hErrRead.Dispose();

                if (pi.hProcess != IntPtr.Zero) NativeMethods.CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero) NativeMethods.CloseHandle(pi.hThread);
            }

            return processResult;
        }
        
        private static Dictionary<string, string> ReadEnvironmentBlock(IntPtr envBlock)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (envBlock == IntPtr.Zero) return result;

            long offsetChars = 0;
            while (true)
            {
                string entry = Marshal.PtrToStringUni(new IntPtr(envBlock.ToInt64() + offsetChars * 2));
                if (string.IsNullOrEmpty(entry))
                    break; // double null terminator reached

                // Some entries look like "=C:=C:\Users\..." (per-drive cwd tracking) and legitimately
                // start with '=' - only treat the first char as data, look for the real separator after it.
                int searchStart = entry.Length > 0 && entry[0] == '=' ? 1 : 0;
                int eqIndex = entry.IndexOf('=', searchStart);

                if (eqIndex > 0)
                {
                    string key = entry.Substring(0, eqIndex);
                    string value = entry.Substring(eqIndex + 1);
                    result[key] = value;
                }

                offsetChars += entry.Length + 1; // skip this string + its null terminator
            }

            return result;
        }
       
        private static IntPtr BuildEnvironmentBlock(Dictionary<string, string> env)
        {
            var keys = new List<string>(env.Keys);
            keys.Sort(StringComparer.OrdinalIgnoreCase); // matches Windows' own convention for env blocks

            var sb = new StringBuilder();
            foreach (var key in keys)
                sb.Append(key).Append('=').Append(env[key]).Append('\0');
            sb.Append('\0'); // final double-null terminator

            // Freed with Marshal.FreeHGlobal - NOT DestroyEnvironmentBlock, since userenv.dll didn't allocate this one.
            return Marshal.StringToHGlobalUni(sb.ToString());
        }

        private static bool LoadUserProfile(string username, SafeTokenHandle primaryToken, out NativeMethods.PROFILEINFO profileInfo)
        {
            var loaded = false;
            var pi = new NativeMethods.PROFILEINFO();
            pi.dwSize = Marshal.SizeOf(pi);
            pi.lpUserName = username;
            pi.dwFlags = 1; // PI_NOUI
            pi.lpDefaultPath = null;

            // Load the profile using the PRIMARY token handle
            loaded = NativeMethods.LoadUserProfile(primaryToken.DangerousGetHandle(), ref pi);

            if (!loaded)
            {
                var ex = new Win32Exception(Marshal.GetLastWin32Error());
                var msg = string.Format("Unable to load user profile: {0}", ex.Message);
                throw new InvalidOperationException(msg);
            }                         

            profileInfo = pi;
            return loaded; 
        }
        
        private static bool SetCurrentThreadPrivilege(string privilegeName, bool enable)
        {
            IntPtr hToken = IntPtr.Zero;

            // Constant pseudo-handles defined by the Windows Kernel
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            IntPtr CURRENT_PROCESS_HANDLE = new IntPtr(-1); 
            IntPtr CURRENT_THREAD_HANDLE = new IntPtr(-2);  

            try
            {
                // 1. Try to open the current THREAD token (captures the MIMWAL impersonation context)
                // Third parameter 'true' means open it using the process's security context if thread token is unavailable
                bool tokenOpened = NativeMethods.OpenThreadToken(CURRENT_THREAD_HANDLE, 
                    NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, true, out hToken);

                if (!tokenOpened)
                {
                    tokenOpened = NativeMethods.OpenProcessToken(CURRENT_PROCESS_HANDLE,
                            NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, out hToken);
                }

                if (!tokenOpened)
                {
                    Win32Exception exception = new Win32Exception(Marshal.GetLastWin32Error(),
                            "Unable to open the current thread or process token.");

                    Logger.Instance.WriteError(exception.Message);

                    return false;
                }

               
                NativeMethods.LUID luid = new NativeMethods.LUID();
                if (!NativeMethods.LookupPrivilegeValue(null, privilegeName, ref luid))
                {
                    var ex = new Win32Exception(Marshal.GetLastWin32Error(), "LookupPrivilegeValue failed for " + privilegeName);
                    Logger.Instance.WriteError(ex.Message);
                    return false;
                }                    

                NativeMethods.TOKEN_PRIVILEGES tp = new NativeMethods.TOKEN_PRIVILEGES();
                tp.PrivilegeCount = 1;
                tp.Privilege.Luid = luid;
                tp.Privilege.Attributes = enable ? (uint)NativeMethods.SE_PRIVILEGE_ENABLED : 0;

                // 2. Adjust the token
                bool adjusted = NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                int adjustError = Marshal.GetLastWin32Error();

                // Local Security Policy or GPO does not grant the required privilege
                if (!adjusted)
                {
                    var ex = new Win32Exception(adjustError, "AdjustTokenPrivileges failed for " + privilegeName);
                    Logger.Instance.WriteError(ex.Message);
                    return false;
                }

                // AdjustTokenPrivileges can succeed but silently not apply the change
                // if the privilege isn't held by the account at all.
                if (adjustError == NativeMethods.ERROR_NOT_ALL_ASSIGNED)
                {
                    var ex = new InvalidOperationException(
                        "The privilege '" + privilegeName + "' is not held by this account. " +
                        "Grant it via Local Security Policy / GPO to the identity this service runs as.");
                    Logger.Instance.WriteError(ex.Message);
                    return false;
                }            
            }
            finally
            {
                if (hToken != IntPtr.Zero && hToken != INVALID_HANDLE_VALUE)
                {
                    NativeMethods.CloseHandle(hToken);
                }
            }

            return true;
        }

        private static void EnsureImpersonationPrivileges()
        {
            if (_impersonateUserPrivilegesInitialized) return;

            lock(_impersonateUserPrivilegesLock)
            {
                bool assignTokenPrivilegeOk = SetCurrentThreadPrivilege("SeAssignPrimaryTokenPrivilege", true);
                if(!assignTokenPrivilegeOk)
                    Logger.Instance.WriteError("Service account is missing SeAssignPrimaryTokenPrivilege - Impersonation calls will fail.");
                
                _impersonateUserPrivilegesInitialized = assignTokenPrivilegeOk;
            }            
        }

        private static void EnsureProfileLoadPrivileges()
        {
            if (_loadUserProfilePrivilegesInitialized) return; // fast path for every call after the first that needs it

            lock (_loadUserProfilePrivilegesLock)
            {
                if (_loadUserProfilePrivilegesInitialized) return; // re-check inside the lock

                bool backupOk = SetCurrentThreadPrivilege(NativeMethods.SE_BACKUP_NAME, true);
                bool restoreOk = SetCurrentThreadPrivilege(NativeMethods.SE_RESTORE_NAME, true);

                if (!backupOk)
                    Logger.Instance.WriteError("Service account is missing SeBackupPrivilege - LoadUserProfile calls will fail.");
                if (!restoreOk)
                    Logger.Instance.WriteError("Service account is missing SeRestorePrivilege - LoadUserProfile calls will fail.");

                _loadUserProfilePrivilegesInitialized = backupOk && restoreOk;
            }
        }

        private static void CreateAsyncOutputPipe(out SafeFileHandle serverRead, out SafeFileHandle clientWrite)
        {
            string pipeName = @"\\.\pipe\MIMWAL_" + Guid.NewGuid().ToString("N");

            var saServer = new NativeMethods.SECURITY_ATTRIBUTES();
            saServer.nLength = Marshal.SizeOf(saServer);
            saServer.bInheritHandle = false; // our read end must not be inherited by the child
            saServer.lpSecurityDescriptor = IntPtr.Zero;

            serverRead = NativeMethods.CreateNamedPipeW(
                pipeName,
                NativeMethods.PIPE_ACCESS_INBOUND | NativeMethods.FILE_FLAG_OVERLAPPED,
                NativeMethods.PIPE_TYPE_BYTE | NativeMethods.PIPE_WAIT,
                1,      // this pipe instance is used by exactly one process
                4096,
                4096,
                0,
                ref saServer);

            if (serverRead.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateNamedPipe failed");

            var saClient = new NativeMethods.SECURITY_ATTRIBUTES();
            saClient.nLength = Marshal.SizeOf(saClient);
            saClient.bInheritHandle = true; // the child process must inherit this one
            saClient.lpSecurityDescriptor = IntPtr.Zero;

            clientWrite = NativeMethods.CreateFileW(
                pipeName,
                NativeMethods.GENERIC_WRITE,
                0,
                ref saClient,
                NativeMethods.OPEN_EXISTING,
                NativeMethods.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero);

            if (clientWrite.IsInvalid)
            {
                var err = Marshal.GetLastWin32Error();
                serverRead.Dispose();
                throw new Win32Exception(err, "CreateFile (pipe client end) failed");
            }
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Aproved.")]
        private static void ReadPipeAsync(SafeFileHandle readHandle, Action<string, bool, Exception> onComplete)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(readHandle, FileAccess.Read, 4096, true); // isAsync: true -> binds to IOCP
            }
            catch (Exception ex)
            {
                onComplete(null, false, ex);
                return;
            }

            var buffer = new MemoryStream();
            var chunk = new byte[4096];
            bool truncated = false;

            AsyncCallback callback = null;
            callback = ar =>
            {
                try
                {
                    int bytesRead = fs.EndRead(ar);
                    if (bytesRead > 0)
                    {
                        if (!truncated)
                        {
                            if (buffer.Length + bytesRead > MaxCapturedBytes)
                            {
                                // Write only enough to hit the cap exactly, then stop accumulating.
                                int remainingCapacity = (int)(MaxCapturedBytes - buffer.Length);
                                if (remainingCapacity > 0)
                                    buffer.Write(chunk, 0, remainingCapacity);

                                truncated = true;
                            }
                            else
                            {
                                buffer.Write(chunk, 0, bytesRead);
                            }
                        }
                        // else: past the cap - deliberately discard this chunk, but keep reading below
                        // so the pipe keeps draining and the child process never blocks on a full buffer.

                        fs.BeginRead(chunk, 0, chunk.Length, callback, null); // keep going regardless
                    }
                    else
                    {
                        fs.Dispose();
                        onComplete(Encoding.UTF8.GetString(buffer.ToArray()), truncated, null); // EOF
                    }
                }
                catch (Exception ex)
                {
                    fs.Dispose();
                    onComplete(buffer.Length > 0 ? Encoding.UTF8.GetString(buffer.ToArray()) : null, truncated, ex);
                }
            };

            try
            {
                fs.BeginRead(chunk, 0, chunk.Length, callback, null);
            }
            catch (Exception ex)
            {
                fs.Dispose();
                onComplete(null, false, ex);
            }
        }

        private static void KillProcessTree(int ProcessID)
        {
            string systemDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.System);

            using (Process killer = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(systemDirectory, "taskkill.exe"),
                Arguments = "/F /T /PID " + ProcessID,
                CreateNoWindow = true,
                UseShellExecute = false
            }))
            {
                killer.WaitForExit(2000); // wait for the kill request
            }
        }



    }
}

