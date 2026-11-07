//-----------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">
//      Copyright (c) Microsoft. All Rights Reserved.
//      Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>
// <summary>
// Native Method calls
// </summary>
//-----------------------------------------------------------------------------------------------------------------------
#pragma warning disable CA5122
namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
    using System.Text;
    using System.Security;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Class to call Win32 API's
    /// </summary>
    [System.Security.SecurityCritical]
    [SuppressMessage("SonarAnalyzer.CSharp", "S101", Justification = "Approved.")]
    internal static class NativeMethods
    {
        #region Impersonation API's

        public const int DefaultTimeoutExitCode = -2; // sentinel: distinguishes between "script was killed" and a real script exit code

        public const int ERROR_NOT_ALL_ASSIGNED = 1300;

        public const uint TOKEN_ALL_ACCESS = 0xF01FF;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;        
        internal const int TOKEN_QUERY = 0x00000008;

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;        
        internal const string SE_BACKUP_NAME = "SeBackupPrivilege";
        internal const string SE_RESTORE_NAME = "SeRestorePrivilege";

        internal const int CREATE_NO_WINDOW = 0x08000000;
        internal const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        internal const uint INFINITE = 0xFFFFFFFF;
        internal const int STARTF_USESTDHANDLES = 0x00000100;
        internal const int HANDLE_FLAG_INHERIT = 0x00000001;

        public const uint PIPE_ACCESS_INBOUND = 0x00000001;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint PIPE_TYPE_BYTE = 0x00000000;
        public const uint PIPE_WAIT = 0x00000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        public const uint WAIT_TIMEOUT = 0x00000102; // 258
        public const uint WAIT_OBJECT_0 = 0x00000000;
        public const uint WAIT_FAILED = 0xFFFFFFFF;

        [StructLayout(LayoutKind.Sequential)]
        internal struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }

        
        [StructLayout(LayoutKind.Sequential)]
        internal struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privilege;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [SecurityCritical]
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenThreadToken(IntPtr ThreadHandle, uint DesiredAccess, bool OpenAsSelf, out IntPtr TokenHandle);

        [SecurityCritical]
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

        [SecurityCritical]
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);

        [SecurityCritical]
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool DuplicateTokenEx(
            SafeTokenHandle hExistingToken,
            uint dwDesiredAccess,
            IntPtr lpTokenAttributes,
            SecurityImpersonationLevel ImpersonationLevel,
            TokenType TokenType,
            out SafeTokenHandle phNewToken);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateNamedPipeW(
            string lpName,
            uint dwOpenMode,
            uint dwPipeMode,
            uint nMaxInstances,
            uint nOutBufferSize,
            uint nInBufferSize,
            uint nDefaultTimeOut,
            ref SECURITY_ATTRIBUTES lpSecurityAttributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFileW(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            ref SECURITY_ATTRIBUTES lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        /// <summary>
        /// Advapi32 function used for logging in as a user
        /// </summary>
        /// <param name="lpszUsername">The user name of the user to log in as</param>
        /// <param name="lpszDomain">The domain the user is in</param>
        /// <param name="lpszPassword">The users password</param>
        /// <param name="dwLogonType">The type of logon operation to perform</param>
        /// <param name="dwLogonProvider">Specifies the logon provider</param>
        /// <param name="phToken">A pointer to a handle variable that receives a handle to a token that represents the specified user</param>
        /// <returns>If the function succeeds, the function returns nonzero</returns>
        [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "CA5122 is firing in this case because the module is build against 3.5, which causes the entire assembly to be safe critical as the security transparency features of the .NET framework began in 4.0")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using case as defined in the implemented interface")]
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, LogOnType dwLogonType, LogOnProvider dwLogonProvider, out SafeTokenHandle phToken);

        /// <summary>
        /// Advapi32 function used to close the handle to impersonated user token
        /// </summary>
        /// <param name="handle">Token returned from LogOnUser</param>
        /// <returns>Returns true if error</returns>
        [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "CA5122 is firing in this case because the module is build against 3.5, which causes the entire assembly to be safe critical as the security transparency features of the .NET framework began in 4.0")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);

        [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "CA5122 is firing in this case because the module is build against 3.5, which causes the entire assembly to be safe critical as the security transparency features of the .NET framework began in 4.0")]
        [DllImport("userenv.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LoadUserProfile(IntPtr hToken, ref PROFILEINFO lpProfileInfo);

        [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "CA5122 is firing in this case because the module is build against 3.5, which causes the entire assembly to be safe critical as the security transparency features of the .NET framework began in 4.0")]
        [DllImport("Userenv.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnloadUserProfile(IntPtr hToken, IntPtr hProfile);

        [SecurityCritical]
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool CreateProcessAsUserW(
            IntPtr hToken, string lpApplicationName, string lpCommandLine,
            IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles,
            int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [SecurityCritical]
        [DllImport("userenv.dll", SetLastError = true)]
        internal static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);
        
        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [SecurityCritical]
        internal static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [SecurityCritical]
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode);

        [SecurityCritical]
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        #endregion Impersonation API's

        #region Registry API's

        /// <summary>
        /// Closes the open registry key.
        /// </summary>
        /// <param name="hKey">A handle to the open key to be closed.</param>
        /// <returns>If the function succeeds, the return value is ERROR_SUCCESS.</returns>
        [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "CA5122 is firing in this case because the module is build against 3.5, which causes the entire assembly to be safe critical as the security transparency features of the .NET framework began in 4.0")]
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int RegCloseKey(IntPtr hKey);

        #endregion Registry API's

        #region Impersonation Structures

        /// <summary>
        /// Contains information about a user profile
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct PROFILEINFO
        {
            /// <summary>
            /// Specifies the size of the structure, in bytes.
            /// </summary>
            public int dwSize;

            /// <summary>
            /// This member can be one of the following flags: 
            /// PI_NOUI or PI_APPLYPOLICY
            /// PI_NOUI: Prevents the display of profile error messages.
            /// PI_APPLYPOLICY : Apply Microsoft Windows NT 4.0-style policy.
            /// </summary>
            public int dwFlags;

            /// <summary>
            /// Pointer to the name of the user.
            /// This member is used as the base name of the directory in which to scope a new profile.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpUserName;

            /// <summary>
            /// Pointer to the roaming user profile path.
            /// If the user does not have a roaming profile, this member can be NULL.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpProfilePath;

            /// <summary>
            /// Pointer to the default user profile path. This member can be NULL.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDefaultPath;

            /// <summary>
            /// Pointer to the name of the validating domain controller, in NetBIOS format.
            /// If this member is NULL, the Windows NT 4.0-style policy will not be applied.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpServerName;

            /// <summary>
            /// Pointer to the path of the Windows NT 4.0-style policy file. 
            /// This member can be NULL.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpPolicyPath;

            /// <summary>
            /// Handle to the HKEY_CURRENT_USER registry key.
            /// </summary>
            public IntPtr hProfile;
        }

        #endregion Impersonation Structures
        
    }
}
