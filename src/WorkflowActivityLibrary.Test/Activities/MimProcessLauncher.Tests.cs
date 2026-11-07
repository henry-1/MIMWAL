using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using System.Text;
using System.IO;
using System.ComponentModel;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class MimProcessLauncherTests
    {
        // enable these tests to get a full picture 
        // it is temporarily commented out to not slow down other test series

        /*
         
        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_Timeout_ShouldTerminateProcess()
        {
            // Arrange
            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "Start-Sleep -Seconds 10";

            byte[] commandBytes =
                Encoding.Unicode.GetBytes(command);

            string encodedCommand =
                Convert.ToBase64String(commandBytes);

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            const int timeout =
                1000;

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    timeout);

            // Assert
            Assert.IsTrue(
                result.TimedOut);

            Assert.AreEqual(
                -2,
                result.ExitCode);

            Assert.AreEqual(
                string.Empty,
                result.StandardOutput);

            Assert.IsFalse(
                string.IsNullOrEmpty(result.StandardError));

            StringAssert.Contains(
                result.StandardError,
                "exceeded timeout");


            Assert.IsTrue(
                result.StandardError.StartsWith(
                    "PowerShell process (PID ",
                    StringComparison.Ordinal));

            Assert.IsTrue(
                result.StandardError.EndsWith(
                    ") exceeded timeout of 1000 ms and was terminated.",
                    StringComparison.Ordinal));
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_TimeoutZero_ShouldWaitForCompletion()
        {
            // Arrange
            const string expectedOutput =
                "Completed";

            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "Start-Sleep -Milliseconds 500; " +
                "Write-Output '" + expectedOutput + "'";

            byte[] commandBytes =
                Encoding.Unicode.GetBytes(command);

            string encodedCommand =
                Convert.ToBase64String(commandBytes);

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    0);

            // Assert
            Assert.IsFalse(
                result.TimedOut);

            Assert.AreEqual(
                0,
                result.ExitCode);

            Assert.AreEqual(
                expectedOutput,
                result.StandardOutput);
        }

        */


        [TestMethod]
        public void Activities_MimProcessLauncher_EnvironmentBlock_RoundTrip_ShouldPreserveValues()
        {
            // Arrange
            Dictionary<string, string> expected =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            expected["AECRequestId"] =
                "Request-123";

            expected["CredentialUser"] =
                @"CONTOSO\PowerShellUser";

            expected["CredentialPassword"] =
                "TestPassword!42";

            expected["EmptyValue"] =
                string.Empty;

            IntPtr environmentBlock =
                IntPtr.Zero;

            try
            {
                // Act
                object blockResult =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(MimProcessLauncher),
                        "BuildEnvironmentBlock",
                        expected);

                environmentBlock =
                    (IntPtr)blockResult;

                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(MimProcessLauncher),
                        "ReadEnvironmentBlock",
                        environmentBlock);

                // Assert
                Assert.AreNotEqual(
                    IntPtr.Zero,
                    environmentBlock);

                Assert.IsInstanceOfType(
                    result,
                    typeof(Dictionary<string, string>));

                Dictionary<string, string> actual =
                    (Dictionary<string, string>)result;

                Assert.AreEqual(
                    expected.Count,
                    actual.Count);

                foreach (KeyValuePair<string, string> item in expected)
                {
                    Assert.IsTrue(
                        actual.ContainsKey(item.Key),
                        "Environment variable was not found: " +
                        item.Key);

                    Assert.AreEqual(
                        item.Value,
                        actual[item.Key],
                        "Unexpected value for environment variable: " +
                        item.Key);
                }
            }
            finally
            {
                if (environmentBlock != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(
                        environmentBlock);
                }
            }
        }

        [TestMethod]
        public void Activities_MimProcessLauncher_EnvironmentBlock_DriveCurrentDirectory_ShouldPreserveEntry()
        {
            // Arrange
            Dictionary<string, string> expected =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            expected["=C:"] =
                @"C:\Users\Test";

            expected["NormalVariable"] =
                "NormalValue";

            IntPtr environmentBlock =
                IntPtr.Zero;

            try
            {
                // Act
                environmentBlock =
                    (IntPtr)ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(MimProcessLauncher),
                        "BuildEnvironmentBlock",
                        expected);

                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(MimProcessLauncher),
                        "ReadEnvironmentBlock",
                        environmentBlock);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(Dictionary<string, string>));

                Dictionary<string, string> actual =
                    (Dictionary<string, string>)result;

                Assert.AreEqual(
                    2,
                    actual.Count);

                Assert.IsTrue(
                    actual.ContainsKey("=C:"));

                Assert.AreEqual(
                    @"C:\Users\Test",
                    actual["=C:"]);

                Assert.IsTrue(
                    actual.ContainsKey("NormalVariable"));

                Assert.AreEqual(
                    "NormalValue",
                    actual["NormalVariable"]);
            }
            finally
            {
                if (environmentBlock != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(
                        environmentBlock);
                }
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_ValidCommand_ShouldReturnOutput()
        {
            // Arrange
            const string expectedOutput =
                "MIMWAL-Test-Result";

            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "Write-Output '" + expectedOutput + "'";

            byte[] commandBytes =
                Encoding.Unicode.GetBytes(command);

            string encodedCommand =
                Convert.ToBase64String(commandBytes);

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    10000);

            // Assert
            Assert.IsFalse(
                result.TimedOut);

            Assert.AreEqual(
                0,
                result.ExitCode);

            Assert.AreEqual(
                expectedOutput,
                result.StandardOutput);            
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_FailingCommand_ShouldReturnNonZeroExitCode()
        {
            // Arrange
            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "Write-Error 'Expected test error'; exit 1";

            byte[] commandBytes =
                Encoding.Unicode.GetBytes(command);

            string encodedCommand =
                Convert.ToBase64String(commandBytes);

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    10000);

            // Assert
            Assert.IsFalse(
                result.TimedOut);

            Assert.AreNotEqual(
                0,
                result.ExitCode);

            Assert.IsFalse(
                string.IsNullOrEmpty(result.StandardError));
        }

        
        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_NoOutput_ShouldReturnEmptyOutput()
        {
            // Arrange
            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "$value = 42";

            byte[] commandBytes =
                Encoding.Unicode.GetBytes(command);

            string encodedCommand =
                Convert.ToBase64String(commandBytes);

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    10000);

            // Assert
            Assert.IsFalse(
                result.TimedOut);

            Assert.AreEqual(
                0,
                result.ExitCode);

            Assert.AreEqual(
                string.Empty,
                result.StandardOutput);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_MultipleOutputLines_ShouldCaptureAllLinesInOrder()
        {
            // Arrange
            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "Write-Output 'First'; " +
                "Write-Output 'Second'; " +
                "Write-Output 'Third'";

            byte[] commandBytes =
                Encoding.Unicode.GetBytes(command);

            string encodedCommand =
                Convert.ToBase64String(commandBytes);

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            string expectedOutput =
                "First" +
                Environment.NewLine +
                "Second" +
                Environment.NewLine +
                "Third";

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    10000);

            // Assert
            Assert.IsFalse(
                result.TimedOut);

            Assert.AreEqual(
                0,
                result.ExitCode);

            Assert.AreEqual(
                expectedOutput,
                result.StandardOutput);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_ExecutableMissing_ShouldThrowWin32Exception()
        {
            // Arrange
            string executablePath =
                Path.Combine(
                    Path.Combine(
                        Path.GetTempPath(),
                        Guid.NewGuid().ToString("N")),
                    "missing-powershell.exe");

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            string command =
                "Write-Output 'Test'";

            string encodedCommand =
                Convert.ToBase64String(
                    Encoding.Unicode.GetBytes(command));

            try
            {
                // Act
                MimProcessLauncher.RunPowershellAsService(
                    executablePath,
                    workingDirectory,
                    encodedCommand,
                    10000);

                Assert.Fail(
                    "Expected Win32Exception was not thrown.");
            }
            catch (Win32Exception)
            {
                // Expected.
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_MimProcessLauncher_RunPowershellAsService_NonTerminatingError_ShouldReturnZeroExitCodeAndErrorOutput()
        {
            // Arrange
            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    @"WindowsPowerShell\v1.0\powershell.exe");

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            string command =
                "$ErrorActionPreference = 'Continue'; " +
                "Write-Error 'Expected non-terminating error'; " +
                "Write-Output 'Completed'";

            string encodedCommand =
                Convert.ToBase64String(
                    Encoding.Unicode.GetBytes(command));

            string workingDirectory =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowershellAsService(
                    powerShellPath,
                    workingDirectory,
                    encodedCommand,
                    10000);

            // Assert
            Assert.IsFalse(
                result.TimedOut);

            Assert.AreEqual(
                0,
                result.ExitCode);

            Assert.AreEqual(
                "Completed",
                result.StandardOutput);

            Assert.IsFalse(
                string.IsNullOrEmpty(result.StandardError));
        }

        /*
        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("RequiresCredentials")]
        [TestCategory("RequiresImpersonationEnvironment")]
        public void Activities_MimProcessLauncher_RunPowerShellAsUser_ValidCredentials_ShouldRunAsConfiguredUser()
        {
            // Arrange
            string userName =
                Environment.GetEnvironmentVariable(
                    "MIMWAL_TEST_USERNAME");

            string domain =
                Environment.GetEnvironmentVariable(
                    "MIMWAL_TEST_DOMAIN");

            string password =
                Environment.GetEnvironmentVariable(
                    "MIMWAL_TEST_PASSWORD");

            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password))
            {
                Assert.Inconclusive(
                    "Set MIMWAL_TEST_USERNAME and MIMWAL_TEST_PASSWORD " +
                    "before running this integration test.");
            }

            string powerShellPath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                   @"WindowsPowerShell\v1.0\powershell.exe");  

            Assert.IsTrue(
                File.Exists(powerShellPath),
                "Windows PowerShell executable was not found: " +
                powerShellPath);

            const string expectedEnvironmentValue =
                "MIMWAL-Impersonation-Test";

            Dictionary<string, string> customEnvironment =
                new Dictionary<string, string>();

            customEnvironment["MIMWAL_TEST_VALUE"] =
                expectedEnvironmentValue;

            string command =
                "Write-Output " +
                "([Environment]::UserName + '|' + $env:MIMWAL_TEST_VALUE)";

            string encodedCommand =
                Convert.ToBase64String(
                    Encoding.Unicode.GetBytes(command));

            // Act
            PowerShellResult result =
                MimProcessLauncher.RunPowerShellAsUser(
                    userName,
                    domain,
                    password,
                    powerShellPath,
                    encodedCommand,
                    LogOnType.LogOnInteractive,
                    true,
                    customEnvironment,
                    10000);

            // Assert
            Assert.IsFalse(
                result.TimedOut,
                result.StandardError);

            Assert.AreEqual(
                0,
                result.ExitCode,
                result.StandardError);

            Assert.AreEqual(
                userName + "|" + expectedEnvironmentValue,
                result.StandardOutput);

            Assert.AreEqual(
                string.Empty,
                result.StandardError);
        }

        */
    }
}