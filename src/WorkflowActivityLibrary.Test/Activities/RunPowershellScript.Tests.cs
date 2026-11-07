using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using System.Collections;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System.Reflection;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using System.Workflow.Activities;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.ComponentActivities;
using System.Workflow.ComponentModel;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class RunPowershellScriptTests
    {

        // enable this test to get a full picture 
        // it is temporarily commented out to not slow down other test series

        /*
        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_Timeout_ShouldThrowExternalException()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                1000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "Start-Sleep -Seconds 10";

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "RunScript",
                    script,
                    null,
                    null);

                Assert.Fail(
                    "Expected ExternalException was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsNotNull(
                    ex.InnerException);

                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(ExternalException));

                Assert.IsTrue(
                    ex.InnerException.Message.StartsWith(
                        "PowerShell process (PID ",
                        StringComparison.Ordinal));

                Assert.IsTrue(
                    ex.InnerException.Message.EndsWith(
                        ") exceeded timeout of 1000 ms and was terminated.",
                        StringComparison.Ordinal));
            }
        }
        */
        [TestMethod]
        public void Activities_RunPowerShellScript_IsConsistentType_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Empty list",
                    new List<object>(),
                    true
                },
                new object[]
                {
                    "Single string",
                    new List<object>
                    {
                        "One"
                    },
                    true
                },
                new object[]
                {
                    "Multiple strings",
                    new List<object>
                    {
                        "One",
                        "Two",
                        "Three"
                    },
                    true
                },
                new object[]
                {
                    "Multiple integers",
                    new List<object>
                    {
                        1,
                        2,
                        3
                    },
                    true
                },
                new object[]
                {
                    "Different types",
                    new List<object>
                    {
                        "One",
                        2
                    },
                    false
                },
                new object[]
                {
                    "Null among strings",
                    new List<object>
                    {
                        "One",
                        null,
                        "Three"
                    },
                    false
                },
                new object[]
                {
                    "Null first then string",
                    new List<object>
                    {
                        null,
                        "One"
                    },
                    false
                },
                new object[]
                {
                    "All null",
                    new List<object>
                    {
                        null,
                        null
                    },
                    true
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                List<object> values = (List<object>)testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "IsConsistentType",
                        values);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }

        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetListResults_ConsistentTypes_ShouldReturnList()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Strings",
                    new object[] { "One", "Two", "Three" }
                },
                new object[]
                {
                    "Integers",
                    new object[] { 1, 2, 3 }
                },
                new object[]
                {
                    "Nulls",
                    new object[] { null, null }
                },
                new object[]
                {
                    "Empty",
                    new object[0]
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object[] input = (object[])testCase[1];

                // Act
                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "GetListResults",
                        (object)input);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(List<object>),
                    "Failed test case: " + description);

                List<object> resultList =
                    (List<object>)result;

                CollectionAssert.AreEqual(
                    input,
                    resultList.ToArray(),
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetDictionaryResults_ShouldReturnHashtable()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Empty dictionary",
                    new Dictionary<string, object>()
                },
                new object[]
                {
                    "String value",
                    new Dictionary<string, object>
                    {
                        { "Name", "Max" }
                    }
                },
                new object[]
                {
                    "Different value types",
                    new Dictionary<string, object>
                    {
                        { "Name", "Max" },
                        { "Age", 42 },
                        { "Enabled", true }
                    }
                },
                new object[]
                {
                    "Null value",
                    new Dictionary<string, object>
                    {
                        { "Name", null }
                    }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];

                Dictionary<string, object> input =
                    (Dictionary<string, object>)testCase[1];

                // Act
                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "GetDictionaryResults",
                        input);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(Hashtable),
                    "Failed test case: " + description);

                Hashtable resultTable =
                    (Hashtable)result;

                Assert.AreEqual(
                    input.Count,
                    resultTable.Count,
                    "Failed test case: " + description);

                foreach (KeyValuePair<string, object> item in input)
                {
                    Assert.IsTrue(
                        resultTable.ContainsKey(item.Key),
                        "Expected key was not found: " +
                        item.Key +
                        ". Test case: " +
                        description);

                    Assert.AreEqual(
                        item.Value,
                        resultTable[item.Key],
                        "Unexpected value for key: " +
                        item.Key +
                        ". Test case: " +
                        description);
                }
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetListResults_InconsistentTypes_ShouldThrowFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "String and integer",
                    new object[] { "One", 2 }
                },
                new object[]
                {
                    "String and null",
                    new object[] { "One", null }
                },
                new object[]
                {
                    "Null and string",
                    new object[] { null, "One" }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object[] input = (object[])testCase[1];

                try
                {
                    // Act
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "GetListResults",
                        (object)input);

                    Assert.Fail(
                        "Expected FormatException was not thrown. Test case: " +
                        description);
                }
                catch (FormatException)
                {
                    // Expected.
                }
            }
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_ToPowerShellLiteral_SupportedScalarValue_ShouldReturnExpectedLiteral()
        {
            Guid guid =
                new Guid("c302a07d-5400-43af-94dd-83aa65e5aed4");

            DateTime date =
                new DateTime(
                    2026,
                    10,
                    30,
                    12,
                    30,
                    45,
                    DateTimeKind.Utc);

            object[][] testCases =
            {
                new object[]
                {
                    "Null",
                    null,
                    "$null"
                },
                new object[]
                {
                    "String",
                    "Hello",
                    "'Hello'"
                },
                new object[]
                {
                    "String containing apostrophe",
                    "O'Brien",
                    "'O''Brien'"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    "''"
                },
                new object[]
                {
                    "True",
                    true,
                    "$true"
                },
                new object[]
                {
                    "False",
                    false,
                    "$false"
                },
                new object[]
                {
                    "Character",
                    'A',
                    "'A'"
                },
                new object[]
                {
                    "Apostrophe character",
                    '\'',
                    "''''"
                },
                new object[]
                {
                    "Integer",
                    42,
                    "42"
                },
                new object[]
                {
                    "Negative integer",
                    -42,
                    "-42"
                },
                new object[]
                {
                    "Long",
                    42L,
                    "42"
                },
                new object[]
                {
                    "Decimal",
                    12.5m,
                    "12.5"
                },
                new object[]
                {
                    "DateTime",
                    date,
                    "[datetime]'2026-10-30T12:30:45.0000000Z'"
                },
                new object[]
                {
                    "Guid",
                    guid,
                    "[guid]'c302a07d-5400-43af-94dd-83aa65e5aed4'"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                object value =
                    testCase[1];

                string expected =
                    (string)testCase[2];

                // Act
                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "ToPowerShellLiteral",
                        value);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_ToPowerShellLiteral_Enumerable_ShouldReturnExpectedLiteral()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    "@()"
                },
                new object[]
                {
                    "String list",
                    new List<string>
                    {
                        "One",
                        "Two",
                        "Three"
                    },
                    "@('One', 'Two', 'Three')"
                },
                new object[]
                {
                    "Integer list",
                    new List<int>
                    {
                        1,
                        2,
                        3
                    },
                    "@(1, 2, 3)"
                },
                new object[]
                {
                    "Mixed values",
                    new List<object>
                    {
                        "One",
                        2,
                        true,
                        null
                    },
                    "@('One', 2, $true, $null)"
                },
                new object[]
                {
                    "String containing apostrophe",
                    new List<string>
                    {
                        "O'Brien",
                        "Test"
                    },
                    "@('O''Brien', 'Test')"
                },
                new object[]
                {
                    "Nested collections",
                    new List<object>
                    {
                        new List<int>
                        {
                            1,
                            2
                        },
                        new List<string>
                        {
                            "One",
                            "Two"
                        }
                    },
                    "@(@(1, 2), @('One', 'Two'))"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                object value =
                    testCase[1];

                string expected =
                    (string)testCase[2];

                // Act
                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "ToPowerShellLiteral",
                        value);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_ToPowerShellLiteral_UnsupportedType_ShouldThrowNotSupportedException()
        {
            object[] testCases =
            {
                new object(),
                TimeSpan.FromMinutes(5),
                new Version(1, 0)
            };

            foreach (object value in testCases)
            {
                try
                {
                    // Act
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "ToPowerShellLiteral",
                        value);

                    Assert.Fail(
                        "Expected NotSupportedException was not thrown for type: " +
                        value.GetType().FullName);
                }
                catch (NotSupportedException)
                {
                    // Expected.
                }
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_CreatePowershellExecutionString_NoInput_ShouldReturnExpectedScript()
        {
            // Arrange
            string script =
                "Write-Output 'Hello'";

            Dictionary<string, string> preferences =
                new Dictionary<string, string>
                {
                    { "ErrorActionPreference", "Stop" },
                    { "ProgressPreference", "SilentlyContinue" }
                };

            Dictionary<string, string> environment =
                new Dictionary<string, string>();

            environment.Add(
                "AECRequestId",
                "Request-123");

            environment.Add(
                "TestValue",
                "O'Brien");

            // Act
            object result =
                ActivityTestHelper.InvokeStaticPrivateMethod(
                    typeof(RunPowerShellScript),
                    "CreatePowershellExecutionString",
                    script,
                    null,
                    null,
                    environment,
                    preferences);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(string));

            string executionScript =
                (string)result;

            StringAssert.Contains(
                executionScript,
                "$env:AECRequestId = 'Request-123';");

            StringAssert.Contains(
                executionScript,
                "$env:TestValue = 'O''Brien';");

            StringAssert.Contains(
                executionScript,
                "$scriptBlock = {");

            StringAssert.Contains(
                executionScript,
                "Write-Output 'Hello'");

            StringAssert.Contains(
                executionScript,
                "$result = & $scriptBlock;");

            StringAssert.Contains(
                executionScript,
                "if ($null -eq $result) { return $null };");

            StringAssert.Contains(
                executionScript,
                "return ($result | ConvertTo-Json -Compress);");

            Assert.IsFalse(
                executionScript.Contains("$args = @("));

            Assert.IsFalse(
                executionScript.Contains("$params = @{"));

            Assert.IsFalse(
                executionScript.Contains("@args;"));

            Assert.IsFalse(
                executionScript.Contains("@params;"));

            StringAssert.Contains(
                executionScript,
                "$ErrorActionPreference = 'Stop';");

            StringAssert.Contains(
                executionScript,
                "$ProgressPreference = 'SilentlyContinue';");

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ErrorActionPreference"));

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ProgressPreference"));
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_CreatePowershellExecutionString_WithArguments_ShouldReturnExpectedScript()
        {

            Dictionary<string, string> preferences =
                new Dictionary<string, string>
                {
                    { "ErrorActionPreference", "Stop" },
                    { "ProgressPreference", "SilentlyContinue" }
                };

            // Arrange
            string script =
                "Write-Output $args";

            ArrayList arguments =
                new ArrayList
                {
                    "O'Brien",
                    42,
                    true,
                    null
                };

            Dictionary<string, string> environment =
                new Dictionary<string, string>();

            // Act
            object result =
                ActivityTestHelper.InvokeStaticPrivateMethod(
                    typeof(RunPowerShellScript),
                    "CreatePowershellExecutionString",
                    script,
                    arguments,
                    null,
                    environment,
                    preferences);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(string));

            string executionScript =
                (string)result;

            StringAssert.Contains(
                executionScript,
                "$args = @(");

            StringAssert.Contains(
                executionScript,
                "    'O''Brien'");

            StringAssert.Contains(
                executionScript,
                "    42");

            StringAssert.Contains(
                executionScript,
                "    $true");

            StringAssert.Contains(
                executionScript,
                "    $null");

            StringAssert.Contains(
                executionScript,
                "$result = & $scriptBlock @args;");

            Assert.IsFalse(
                executionScript.Contains("$params = @{"));

            Assert.IsFalse(
                executionScript.Contains("@params;"));

            StringAssert.Contains(
                executionScript,
                "$ErrorActionPreference = 'Stop';");

            StringAssert.Contains(
                executionScript,
                "$ProgressPreference = 'SilentlyContinue';");

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ErrorActionPreference"));

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ProgressPreference"));
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_CreatePowershellExecutionString_WithParameters_ShouldReturnExpectedScript()
        {
            // Arrange
            Dictionary<string, string> preferences =
                new Dictionary<string, string>
                {
                    { "ErrorActionPreference", "Stop" },
                    { "ProgressPreference", "SilentlyContinue" }
                };

            string script =
                "param($Name, $Age, $Enabled)";

            Dictionary<string, object> parameters =
                new Dictionary<string, object>
                {
                    { "Name", "O'Brien" },
                    { "Age", 42 },
                    { "Enabled", true }
                };

            Dictionary<string, string> environment =
                new Dictionary<string, string>();

            // Act
            object result =
                ActivityTestHelper.InvokeStaticPrivateMethod(
                    typeof(RunPowerShellScript),
                    "CreatePowershellExecutionString",
                    script,
                    null,
                    parameters,
                    environment,
                    preferences);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(string));

            string executionScript =
                (string)result;

            StringAssert.Contains(
                executionScript,
                "$params = @{");

            StringAssert.Contains(
                executionScript,
                "    Name = 'O''Brien'");

            StringAssert.Contains(
                executionScript,
                "    Age = 42");

            StringAssert.Contains(
                executionScript,
                "    Enabled = $true");

            StringAssert.Contains(
                executionScript,
                "$result = & $scriptBlock @params;");

            Assert.IsFalse(
                executionScript.Contains("$args = @("));

            Assert.IsFalse(
                executionScript.Contains("@args;"));

            StringAssert.Contains(
                executionScript,
                "$ErrorActionPreference = 'Stop';");

            StringAssert.Contains(
                executionScript,
                "$ProgressPreference = 'SilentlyContinue';");

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ErrorActionPreference"));

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ProgressPreference"));
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_CreatePowershellExecutionString_ArgumentsAndParameters_ShouldPreferArguments()
        {
            // Arrange
            Dictionary<string, string> preferences =
                new Dictionary<string, string>
                {
                    { "ErrorActionPreference", "Stop" },
                    { "ProgressPreference", "SilentlyContinue" }
                };
            
            string script =
                "Write-Output 'Test'";

            ArrayList arguments =
                new ArrayList
                {
                    "ArgumentValue",
                    42
                };

            Dictionary<string, object> parameters =
                new Dictionary<string, object>
                {
                    { "Name", "ParameterValue" }
                };

            Dictionary<string, string> environment =
                new Dictionary<string, string>();

            // Act
            object result =
                ActivityTestHelper.InvokeStaticPrivateMethod(
                    typeof(RunPowerShellScript),
                    "CreatePowershellExecutionString",
                    script,
                    arguments,
                    parameters,
                    environment,
                    preferences);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(string));

            string executionScript =
                (string)result;

            StringAssert.Contains(
                executionScript,
                "$args = @(");

            StringAssert.Contains(
                executionScript,
                "'ArgumentValue'");

            StringAssert.Contains(
                executionScript,
                "42");

            StringAssert.Contains(
                executionScript,
                "$result = & $scriptBlock @args;");

            Assert.IsFalse(
                executionScript.Contains("$params = @{"));

            Assert.IsFalse(
                executionScript.Contains("@params;"));

            Assert.IsFalse(
                executionScript.Contains("ParameterValue"));

            StringAssert.Contains(
                executionScript,
                "$ErrorActionPreference = 'Stop';");

            StringAssert.Contains(
                executionScript,
                "$ProgressPreference = 'SilentlyContinue';");

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ErrorActionPreference"));

            Assert.IsFalse(
                executionScript.Contains(
                    "$env:ProgressPreference"));
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_GetPSExecutableName_ShouldReturnExpectedExecutable()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "No version",
                    null,
                    "powershell.exe"
                },
                new object[]
                {
                    "Empty version",
                    string.Empty,
                    "powershell.exe"
                },
                new object[]
                {
                    "Windows PowerShell",
                    PowershellVersion.WindowsPowershell.ToString(),
                    "powershell.exe"
                },
                new object[]
                {
                    "PowerShell",
                    PowershellVersion.Powershell.ToString(),
                    "pwsh.exe"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string version = (string)testCase[1];
                string expected = (string)testCase[2];

                RunPowerShellScript activity =
                    new RunPowerShellScript();

                activity.PowerShellVersion =
                    version;

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "GetPSExecutableName");

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_GetPSExePath_ExecutableExists_ShouldReturnFullPath()
        {
            // Arrange
            string originalPath =
                Environment.GetEnvironmentVariable("PATH");

            string temporaryDirectory =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString("N"));

            string executableName =
                "test-powershell.exe";

            string executablePath =
                Path.Combine(
                    temporaryDirectory,
                    executableName);

            Directory.CreateDirectory(
                temporaryDirectory);

            File.WriteAllText(
                executablePath,
                string.Empty);

            try
            {
                Environment.SetEnvironmentVariable(
                    "PATH",
                    temporaryDirectory);

                // Act
                object result =
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "GetPSExePath",
                        executableName);

                // Assert
                Assert.AreEqual(
                    executablePath,
                    result);
            }
            finally
            {
                Environment.SetEnvironmentVariable(
                    "PATH",
                    originalPath);

                if (File.Exists(executablePath))
                {
                    File.Delete(executablePath);
                }

                if (Directory.Exists(temporaryDirectory))
                {
                    Directory.Delete(temporaryDirectory);
                }
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPSExePath_ExecutableNotFound_ShouldThrowFileNotFoundException()
        {
            // Arrange
            string originalPath =
                Environment.GetEnvironmentVariable("PATH");

            string temporaryDirectory =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString("N"));

            string executableName =
                "missing-powershell-" +
                Guid.NewGuid().ToString("N") +
                ".exe";

            Directory.CreateDirectory(
                temporaryDirectory);

            try
            {
                Environment.SetEnvironmentVariable(
                    "PATH",
                    temporaryDirectory);

                try
                {
                    // Act
                    ActivityTestHelper.InvokeStaticPrivateMethod(
                        typeof(RunPowerShellScript),
                        "GetPSExePath",
                        executableName);

                    Assert.Fail(
                        "Expected FileNotFoundException was not thrown.");
                }
                catch (FileNotFoundException)
                {
                    // Expected.
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(
                    "PATH",
                    originalPath);

                if (Directory.Exists(temporaryDirectory))
                {
                    Directory.Delete(
                        temporaryDirectory);
                }
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetCredential_MissingUserOrPassword_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Missing user",
                    null,
                    "Password"
                },
                new object[]
                {
                    "Empty user",
                    string.Empty,
                    "Password"
                },
                new object[]
                {
                    "Missing password",
                    "PowerShellUser",
                    null
                },
                new object[]
                {
                    "Empty password",
                    "PowerShellUser",
                    string.Empty
                },
                new object[]
                {
                    "Both missing",
                    null,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                RunPowerShellScript activity =
                    new RunPowerShellScript();

                activity.PowerShellUser =
                    (string)testCase[1];

                activity.PowerShellUserPassword =
                    (string)testCase[2];

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "GetCredential");

                // Assert
                Assert.IsNull(
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellUserPassword_EncryptedPassword_ShouldReturnPlainText()
        {
            // Arrange
            string expectedPassword =
                "TestPassword!42";

            byte[] passwordBytes =
                Encoding.Unicode.GetBytes(
                    expectedPassword);

            byte[] encryptedBytes =
                System.Security.Cryptography.ProtectedData.Protect(
                    passwordBytes,
                    null,
                    DataProtectionScope.LocalMachine);

            string encryptedPassword =
                Convert.ToBase64String(
                    encryptedBytes);

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellUserPassword =
                encryptedPassword;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetPowershellUserPassword");

            // Assert
            Assert.AreEqual(
                expectedPassword,
                result);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetCredential_ValidCredentials_ShouldReturnExpectedCredential()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Domain user",
                    @"CONTOSO\PowerShellUser",
                    "PowerShellUser",
                    "CONTOSO"
                },
                new object[]
                {
                    "User without domain",
                    "PowerShellUser",
                    "PowerShellUser",
                    string.Empty
                }
            };

            string expectedPassword =
                "TestPassword!42";

            byte[] passwordBytes =
                Encoding.Unicode.GetBytes(
                    expectedPassword);

            byte[] encryptedBytes =
                System.Security.Cryptography.ProtectedData.Protect(
                    passwordBytes,
                    null,
                    DataProtectionScope.LocalMachine);

            string encryptedPassword =
                Convert.ToBase64String(
                    encryptedBytes);

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string configuredUser =
                    (string)testCase[1];

                string expectedUserName =
                    (string)testCase[2];

                string expectedDomain =
                    (string)testCase[3];

                RunPowerShellScript activity =
                    new RunPowerShellScript();

                activity.PowerShellUser =
                    configuredUser;

                activity.PowerShellUserPassword =
                    encryptedPassword;

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "GetCredential");

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(NetworkCredential),
                    "Failed test case: " + description);

                NetworkCredential credential =
                    (NetworkCredential)result;

                Assert.AreEqual(
                    expectedUserName,
                    credential.UserName,
                    "Failed test case: " + description);

                Assert.AreEqual(
                    expectedPassword,
                    credential.Password,
                    "Failed test case: " + description);

                Assert.AreEqual(
                    expectedDomain,
                    credential.Domain,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellUserPassword_EmptyPassword_ShouldReturnEmptyString()
        {
            object[] testCases =
            {
                null,
                string.Empty
            };

            foreach (object password in testCases)
            {
                // Arrange
                RunPowerShellScript activity =
                    new RunPowerShellScript();

                activity.PowerShellUserPassword =
                    (string)password;

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "GetPowershellUserPassword");

                // Assert
                Assert.AreEqual(
                    string.Empty,
                    result);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellScript_WorkflowDefinition_ShouldReturnConfiguredScript()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output 'Hello World'";

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetPowershellScript");

            // Assert
            Assert.AreEqual(
                activity.Script,
                result);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellScript_Disk_ShouldReturnFileContent()
        {
            // Arrange
            string script =
                "param($Name)" +
                Environment.NewLine +
                "Write-Output $Name";

            string scriptPath =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString("N") + ".ps1");

            File.WriteAllText(
                scriptPath,
                script);

            try
            {
                RunPowerShellScript activity =
                    new RunPowerShellScript();

                activity.ScriptLocation =
                    PowerShellScriptLocation.Disk;

                activity.Script =
                    scriptPath;

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "GetPowershellScript");

                // Assert
                Assert.AreEqual(
                    script,
                    result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                {
                    File.Delete(scriptPath);
                }
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellScript_DiskFileMissing_ShouldThrowWorkflowActivityLibraryException()
        {
            // Arrange
            string scriptPath =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString("N") + ".ps1");

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.Disk;

            activity.Script =
                scriptPath;

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetPowershellScript");

                Assert.Fail(
                    "Expected exception was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(WorkflowActivityLibraryException));
            }
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellScript_Resource_ShouldReturnResolvedScript()
        {
            // Arrange
            const string scriptLookup =
                "[//Target/PowerShellScript]";

            const string expectedScript =
                "Write-Output 'Hello World'";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.Resource;

            activity.Script =
                scriptLookup;

            ExpressionEvaluator evaluator =
                activity.ActivityExpressionEvaluator;

            Dictionary<string, object> lookupCache =
                (Dictionary<string, object>)ActivityTestHelper.GetField<Dictionary<string, object>>(
                    instance: evaluator,
                    fieldName: "lookupCache");

            lookupCache[scriptLookup] =
                expectedScript;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetPowershellScript");

            // Assert
            Assert.AreEqual(
                expectedScript,
                result);
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellScript_ResourceMissingAndFailOnMissingFalse_ShouldReturnEmptyString()
        {
            // Arrange
            const string scriptLookup =
                "[//Target/PowerShellScript]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.Resource;

            activity.Script =
                scriptLookup;

            activity.FailOnMissing =
                false;

            ExpressionEvaluator evaluator =
                activity.ActivityExpressionEvaluator;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    evaluator,
                    "lookupCache");

            lookupCache[scriptLookup] =
                null;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetPowershellScript");

            // Assert
            Assert.AreEqual(
                string.Empty,
                result);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetPowershellScript_ResourceMissingAndFailOnMissingTrue_ShouldThrowWorkflowActivityLibraryException()
        {
            // Arrange
            const string scriptLookup =
                "[//Target/PowerShellScript]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.Resource;

            activity.Script =
                scriptLookup;

            activity.FailOnMissing =
                true;

            ExpressionEvaluator evaluator =
                activity.ActivityExpressionEvaluator;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    evaluator,
                    "lookupCache");

            lookupCache[scriptLookup] =
                null;

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetPowershellScript");

                Assert.Fail(
                    "Expected WorkflowActivityLibraryException was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(WorkflowActivityLibraryException));
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetScriptArguments_ShouldReturnResolvedArguments()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Arguments =
                new ArrayList
                {
                    "42",
                    "true",
                    "false"
                };

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetScriptArguments");

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(ArrayList));

            ArrayList resolvedArguments =
                (ArrayList)result;

            Assert.AreEqual(
                3,
                resolvedArguments.Count);

            // MIM/FIM integer expressions resolve as Int64.
            Assert.AreEqual(
                42L,
                resolvedArguments[0]);

            Assert.AreEqual(
                true,
                resolvedArguments[1]);

            Assert.AreEqual(
                false,
                resolvedArguments[2]);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_GetScriptParameters_ShouldReturnResolvedNonNullParameters()
        {
            // Arrange
            const string missingLookup =
                "[//Target/MissingValue]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            List<Definition> parameters =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "parameters");

            parameters.Add(
                new Definition(
                    "Age",
                    "42",
                    false));

            parameters.Add(
                new Definition(
                    "Enabled",
                    "true",
                    false));

            parameters.Add(
                new Definition(
                    "Missing",
                    missingLookup,
                    false));

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[missingLookup] =
                null;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetScriptParameters");

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Dictionary<string, object>));

            Dictionary<string, object> scriptParameters =
                (Dictionary<string, object>)result;

            Assert.AreEqual(
                2,
                scriptParameters.Count);

            Assert.IsTrue(
                scriptParameters.ContainsKey("Age"));

            Assert.AreEqual(
                42L,
                scriptParameters["Age"]);

            Assert.IsTrue(
                scriptParameters.ContainsKey("Enabled"));

            Assert.AreEqual(
                true,
                scriptParameters["Enabled"]);

            Assert.IsFalse(
                scriptParameters.ContainsKey("Missing"));
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_ActivityExecutionConditionSatisfied_NoCondition_ShouldReturnTrue()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ActivityExecutionCondition =
                null;

            ConditionalEventArgs arguments =
                new ConditionalEventArgs();

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ActivityExecutionConditionSatisfied_Condition",
                null,
                arguments);

            // Assert
            Assert.IsTrue(
                arguments.Result);
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_ActivityExecutionConditionSatisfied_Condition_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "True condition",
                    "true",
                    true
                },
                new object[]
                {
                    "False condition",
                    "false",
                    false
                },
                new object[]
                {
                    "Non-Boolean condition",
                    "\"Test\"",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string condition =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                RunPowerShellScript activity =
                    new RunPowerShellScript();

                activity.ActivityExecutionCondition =
                    condition;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ActivityExecutionConditionSatisfied_Condition",
                    null,
                    arguments);

                // Assert
                Assert.AreEqual(
                    expected,
                    arguments.Result,
                    "Failed test case: " + description);
            }
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_OnSequenceComplete_ShouldClearVariableCacheValues()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            Dictionary<string, object> variableCache =
                activity.ActivityExpressionEvaluator.VariableCache;

            variableCache["FirstVariable"] =
                "Value";

            variableCache["SecondVariable"] =
                42L;

            variableCache["ThirdVariable"] =
                true;

            int expectedCount =
                variableCache.Count;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "OnSequenceComplete",
                (object)null);

            // Assert
            Assert.AreEqual(
                expectedCount,
                variableCache.Count);

            foreach (KeyValuePair<string, object> variable in variableCache)
            {
                Assert.IsNull(
                    variable.Value,
                    "Variable was not cleared: " + variable.Key);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_Prepare_AdvancedFalse_ShouldClearAdvancedSettings()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Advanced =
                false;

            activity.ActivityExecutionCondition =
                "true";

            activity.PowerShellUser =
                @"CONTOSO\PowerShellUser";

            activity.PowerShellUserPassword =
                "EncryptedPassword";

            activity.ImpersonatePowerShellUser =
                true;

            activity.ImpersonatePowerShellUserLoadUserProfile =
                true;

            activity.ImpersonatePowerShellUserLogOnType =
                LogOnType.LogOnInteractive;

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output 'Test'";

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNull(
                activity.ActivityExecutionCondition);

            Assert.IsNull(
                activity.PowerShellUser);

            Assert.IsNull(
                activity.PowerShellUserPassword);

            Assert.IsFalse(
                activity.ImpersonatePowerShellUser);

            Assert.IsFalse(
                activity.ImpersonatePowerShellUserLoadUserProfile);

            Assert.AreEqual(
                LogOnType.None,
                activity.ImpersonatePowerShellUserLogOnType);
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_Prepare_AdvancedTrue_ShouldPreserveAdvancedSettings()
        {
            // Arrange
            const string executionCondition =
                "true";

            const string powerShellUser =
                "\"CONTOSO\\PowerShellUser\"";

            const string powerShellPassword =
                "\"EncryptedPassword\"";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Advanced =
                true;

            activity.ActivityExecutionCondition =
                executionCondition;

            activity.PowerShellUser =
                powerShellUser;

            activity.PowerShellUserPassword =
                powerShellPassword;

            activity.ImpersonatePowerShellUser =
                true;

            activity.ImpersonatePowerShellUserLoadUserProfile =
                true;

            activity.ImpersonatePowerShellUserLogOnType =
                LogOnType.LogOnInteractive;

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output 'Test'";

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                executionCondition,
                activity.ActivityExecutionCondition);

            Assert.AreEqual(
                powerShellUser,
                activity.PowerShellUser);

            Assert.AreEqual(
                powerShellPassword,
                activity.PowerShellUserPassword);

            Assert.IsTrue(
                activity.ImpersonatePowerShellUser);

            Assert.IsTrue(
                activity.ImpersonatePowerShellUserLoadUserProfile);

            Assert.AreEqual(
                LogOnType.LogOnInteractive,
                activity.ImpersonatePowerShellUserLogOnType);
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_Prepare_ResourceScript_ShouldAddScriptLookupToCache()
        {
            // Arrange
            const string scriptLookup =
                "[//Target/PowerShellScript]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Advanced =
                true;

            activity.ScriptLocation =
                PowerShellScriptLocation.Resource;

            activity.Script =
                scriptLookup;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            Assert.IsFalse(
                lookupCache.ContainsKey(scriptLookup));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsTrue(
                lookupCache.ContainsKey(scriptLookup));

            Assert.IsNull(
                lookupCache[scriptLookup]);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_Prepare_Arguments_ShouldAddArgumentLookupsToCache()
        {
            // Arrange
            const string firstArgument =
                "[//Target/AccountName]";

            const string secondArgument =
                "[//Target/DisplayName]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Advanced =
                true;

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output $args";

            activity.InputType =
                PowerShellInputType.Arguments;

            activity.Arguments =
                new ArrayList
                {
                    firstArgument,
                    secondArgument
                };

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            Assert.IsFalse(
                lookupCache.ContainsKey(firstArgument));

            Assert.IsFalse(
                lookupCache.ContainsKey(secondArgument));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsTrue(
                lookupCache.ContainsKey(firstArgument));

            Assert.IsNull(
                lookupCache[firstArgument]);

            Assert.IsTrue(
                lookupCache.ContainsKey(secondArgument));

            Assert.IsNull(
                lookupCache[secondArgument]);
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_Prepare_Parameters_ShouldCreateDefinitionsAndAddLookupsToCache()
        {
            // Arrange
            const string firstLookup =
                "[//Target/AccountName]";

            const string secondLookup =
                "[//Target/DisplayName]";

            Hashtable parametersTable =
                new Hashtable();

            parametersTable["Count"] =
                2;

            parametersTable["0:0"] =
                "AccountName";

            parametersTable["0:1"] =
                firstLookup;

            parametersTable["0:2"] =
                false;

            parametersTable["1:0"] =
                "DisplayName";

            parametersTable["1:1"] =
                secondLookup;

            parametersTable["1:2"] =
                false;

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Advanced =
                true;

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "param($AccountName, $DisplayName)";

            activity.InputType =
                PowerShellInputType.Parameters;

            activity.ParametersTable =
                parametersTable;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            List<Definition> definitions =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "parameters");

            Assert.AreEqual(
                2,
                definitions.Count);

            Assert.AreEqual(
                "AccountName",
                definitions[0].Left);

            Assert.AreEqual(
                firstLookup,
                definitions[0].Right);

            Assert.IsFalse(
                definitions[0].Check);

            Assert.AreEqual(
                "DisplayName",
                definitions[1].Left);

            Assert.AreEqual(
                secondLookup,
                definitions[1].Right);

            Assert.IsFalse(
                definitions[1].Check);

            Assert.IsTrue(
                lookupCache.ContainsKey(firstLookup));

            Assert.IsNull(
                lookupCache[firstLookup]);

            Assert.IsTrue(
                lookupCache.ContainsKey(secondLookup));

            Assert.IsNull(
                lookupCache[secondLookup]);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_Run_EmptyScript_Should_NotThrow()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                string.Empty;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            // No exception means the empty-script early-return path succeeded.
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_ToPowerShellLiteral_Enum_ShouldReturnNumericValue()
        {
            // Arrange
            PowerShellInputType value =
                PowerShellInputType.Arguments;

            string expected =
                Convert.ToInt32(
                    value,
                    CultureInfo.InvariantCulture)
                    .ToString(CultureInfo.InvariantCulture);

            // Act
            object result =
                ActivityTestHelper.InvokeStaticPrivateMethod(
                    typeof(RunPowerShellScript),
                    "ToPowerShellLiteral",
                    value);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_ScalarResult_ShouldReturnDeserializedValue()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "Write-Output 'MIMWAL-Test-Result'";

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.AreEqual(
                "MIMWAL-Test-Result",
                result);
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_ListResult_ShouldReturnList()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "Write-Output @('One', 'Two', 'Three')";

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<object>));

            List<object> values =
                (List<object>)result;

            Assert.AreEqual(
                3,
                values.Count);

            Assert.AreEqual(
                "One",
                values[0]);

            Assert.AreEqual(
                "Two",
                values[1]);

            Assert.AreEqual(
                "Three",
                values[2]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_DictionaryResult_ShouldReturnHashtable()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "$result = @{}; " +
                "$result['Name'] = 'Max'; " +
                "$result['Age'] = 42; " +
                "$result['Enabled'] = $true; " +
                "Write-Output $result";

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Hashtable));

            Hashtable table =
                (Hashtable)result;

            Assert.AreEqual(
                3,
                table.Count);

            Assert.AreEqual(
                "Max",
                table["Name"]);

            Assert.AreEqual(
                42,
                table["Age"]);

            Assert.AreEqual(
                true,
                table["Enabled"]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_NoResult_ShouldReturnNull()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "$value = 42";

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.IsNull(result);
        }


        [TestMethod]
        public void Activities_RunPowerShellScript_CreateEnvironment_ShouldContainExpectedValues()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellUser =
                @"CONTOSO\PowerShellUser";

            string password =
                "SecretPassword";

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CreateEnvironment",
                    password);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Dictionary<string, string>));

            Dictionary<string, string> environment =
                (Dictionary<string, string>)result;

            Assert.AreEqual(
                @"CONTOSO\PowerShellUser",
                environment["CredentialUser"]);

            Assert.AreEqual(
                password,
                environment["CredentialPassword"]);

            Assert.IsTrue(
                environment.ContainsKey("AECWorkflowInstanceId"));

            Assert.IsTrue(
                environment.ContainsKey("AECRequestId"));

            Assert.IsTrue(
                environment.ContainsKey("AECActorId"));

            Assert.IsTrue(
                environment.ContainsKey("AECTargetId"));

            Assert.IsTrue(
                environment.ContainsKey("AECWorkflowDefinitionId"));

            Assert.IsFalse(
                environment.ContainsKey("ErrorActionPreference"));

            Assert.IsFalse(
                environment.ContainsKey("ProgressPreference"));

            Assert.IsFalse(
                environment.ContainsKey("DebugPreference"));

            Assert.IsFalse(
                environment.ContainsKey("VerbosePreference"));
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_ShouldSetErrorActionPreferenceToStop()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                 "Write-Output $ErrorActionPreference.ToString()";

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.AreEqual(
                "Stop",
                result);
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_WriteError_ShouldThrowExternalException()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "Write-Error 'Expected test error'; " +
                "Write-Output 'Should not be reached'";

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "RunScript",
                    script,
                    null,
                    null);

                Assert.Fail(
                    "Expected ExternalException was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsNotNull(
                    ex.InnerException);

                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(ExternalException));

                StringAssert.Contains(
                    ex.InnerException.Message,
                    "Powershell script ended with one or more error(s):");

                StringAssert.Contains(
                    ex.InnerException.Message,
                    "Expected test error");

            }
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_ErrorActionPreferenceOverridden_ShouldReturnResult()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "$ErrorActionPreference = 'Continue'; " +
                "Write-Error 'Expected non-terminating error'; " +
                "Write-Output 'Completed'";

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.AreEqual(
                "Completed",
                result);
        }


        

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_WithArguments_ShouldReturnExpectedResult()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "$args[0] + '|' + $args[1] + '|' + $args[2]";

            ArrayList arguments =
                new ArrayList
                {
                    "O'Brien",
                    42,
                    true
                };

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        arguments,
                        null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.AreEqual(
                "O'Brien|42|True",
                result);
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_WithParameters_ShouldReturnExpectedResult()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "param($Name, $Age, $Enabled) " +
                "$Name + '|' + $Age + '|' + $Enabled";

            Dictionary<string, object> parameters =
                new Dictionary<string, object>
                {
            { "Name", "O'Brien" },
            { "Age", 42 },
            { "Enabled", true }
                };

            // Act
            object result;

            try
            {
                result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "RunScript",
                        script,
                        null,
                        parameters);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    Assert.Fail(
                        "RunScript failed with: " +
                        ex.InnerException);
                }

                throw;
            }

            // Assert
            Assert.AreEqual(
                "O'Brien|42|True",
                result);
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_CreatePreferences_ShouldContainExpectedPreferences()
        {
            // Act
            object result =
                ActivityTestHelper.InvokeStaticPrivateMethod(
                    typeof(RunPowerShellScript),
                    "CreatePreferences");

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Dictionary<string, string>));

            Dictionary<string, string> preferences =
                (Dictionary<string, string>)result;

            Assert.AreEqual(
                4,
                preferences.Count);

            Assert.AreEqual(
                "Stop",
                preferences["ErrorActionPreference"]);

            Assert.IsTrue(
                preferences["ProgressPreference"] == "Continue" ||
                preferences["ProgressPreference"] == "SilentlyContinue");

            Assert.IsTrue(
                preferences["DebugPreference"] == "Continue" ||
                preferences["DebugPreference"] == "SilentlyContinue");

            Assert.IsTrue(
                preferences["VerbosePreference"] == "Continue" ||
                preferences["VerbosePreference"] == "SilentlyContinue");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_ExplicitResult_ShouldPublishResult()
        {
            // Arrange
            const string expectedValue =
                "MIMWAL-Test-Result";

            const string returnValueLookup =
                "[//WorkflowData/TestResult]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output '" + expectedValue + "'";

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Explicit;

            activity.ReturnValueLookup =
                returnValueLookup;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                1,
                definitions.Count);

            UpdateLookupDefinition definition =
                definitions[0];

            Assert.AreEqual(
                returnValueLookup,
                definition.TargetLookup);

            Assert.AreEqual(
                expectedValue,
                definition.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                definition.Mode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_ExplicitNullResult_ShouldPublishNullValue()
        {
            // Arrange
            const string returnValueLookup =
                "[//WorkflowData/TestResult]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "$value = 42";

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Explicit;

            activity.ReturnValueLookup =
                returnValueLookup;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                1,
                definitions.Count);

            UpdateLookupDefinition definition =
                definitions[0];

            Assert.AreEqual(
                returnValueLookup,
                definition.TargetLookup);

            Assert.IsNull(
                definition.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                definition.Mode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_TableResult_ShouldPublishEachValue()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "$result = @{}; " +
                "$result['Name'] = 'Max'; " +
                "$result['Age'] = 42; " +
                "Write-Output $result";

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Table;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                2,
                definitions.Count);

            Dictionary<string, UpdateLookupDefinition> definitionsByLookup =
                new Dictionary<string, UpdateLookupDefinition>();

            foreach (UpdateLookupDefinition definition in definitions)
            {
                definitionsByLookup.Add(
                    definition.TargetLookup,
                    definition);
            }

            const string nameLookup =
                "[//WorkflowData/Name]";

            const string ageLookup =
                "[//WorkflowData/Age]";

            Assert.IsTrue(
                definitionsByLookup.ContainsKey(nameLookup));

            Assert.AreEqual(
                "Max",
                definitionsByLookup[nameLookup].Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                definitionsByLookup[nameLookup].Mode);

            Assert.IsTrue(
                definitionsByLookup.ContainsKey(ageLookup));

            Assert.AreEqual(
                42,
                definitionsByLookup[ageLookup].Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                definitionsByLookup[ageLookup].Mode);
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_TableWithNullResult_ShouldPublishNoValues()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "$value = 42";

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Table;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                0,
                definitions.Count);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_Arguments_ShouldResolveExecuteAndPublishResult()
        {
            // Arrange
            const string returnValueLookup =
                "[//WorkflowData/TestResult]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "$args[0] + '|' + $args[1]";

            activity.InputType =
                PowerShellInputType.Arguments;

            activity.Arguments =
                new ArrayList
                {
            "\"O'Brien\"",
            "42"
                };

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Explicit;

            activity.ReturnValueLookup =
                returnValueLookup;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                1,
                definitions.Count);

            UpdateLookupDefinition definition =
                definitions[0];

            Assert.AreEqual(
                returnValueLookup,
                definition.TargetLookup);

            Assert.AreEqual(
                "O'Brien|42",
                definition.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                definition.Mode);
        }


        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_Parameters_ShouldResolveExecuteAndPublishResult()
        {
            // Arrange
            const string returnValueLookup =
                "[//WorkflowData/TestResult]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "param($Name, $Age) " +
                "$Name + '|' + $Age";

            activity.InputType =
                PowerShellInputType.Parameters;

            List<Definition> parameters =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "parameters");

            parameters.Add(
                new Definition(
                    "Name",
                    "\"O'Brien\"",
                    false));

            parameters.Add(
                new Definition(
                    "Age",
                    "42",
                    false));

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Explicit;

            activity.ReturnValueLookup =
                returnValueLookup;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                1,
                definitions.Count);

            UpdateLookupDefinition definition =
                definitions[0];

            Assert.AreEqual(
                returnValueLookup,
                definition.TargetLookup);

            Assert.AreEqual(
                "O'Brien|42",
                definition.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                definition.Mode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_RepeatedExecution_ShouldReplacePublishedResults()
        {
            // Arrange
            const string returnValueLookup =
                "[//WorkflowData/TestResult]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.Explicit;

            activity.ReturnValueLookup =
                returnValueLookup;

            activity.Script =
                "Write-Output 'FirstResult'";

            // Act: first execution
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> firstDefinitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                firstDefinitions);

            Assert.AreEqual(
                1,
                firstDefinitions.Count);

            Assert.AreEqual(
                "FirstResult",
                firstDefinitions[0].Value);

            // Arrange: change script for second execution
            activity.Script =
                "Write-Output 'SecondResult'";

            // Act: second execution
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            List<UpdateLookupDefinition> secondDefinitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            // Assert
            Assert.IsNotNull(
                secondDefinitions);

            Assert.AreEqual(
                1,
                secondDefinitions.Count);

            Assert.AreEqual(
                returnValueLookup,
                secondDefinitions[0].TargetLookup);

            Assert.AreEqual(
                "SecondResult",
                secondDefinitions[0].Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                secondDefinitions[0].Mode);

            Assert.AreNotSame(
                firstDefinitions,
                secondDefinitions);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_NoReturnType_ShouldPublishNoValues()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output 'Unpublished result'";

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.None;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                0,
                definitions.Count);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_Run_None_ShouldPublishNoValues()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output 'Unpublished result'";

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            activity.ReturnType =
                PowerShellReturnType.None;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Run_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            object publishOutput =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "PublishOutput");

            Assert.IsNotNull(
                publishOutput);

            Type publishOutputType =
                publishOutput.GetType();

            FieldInfo definitionsPropertyField =
                publishOutputType.GetField(
                    "UpdateLookupDefinitionsProperty",
                    BindingFlags.Public |
                    BindingFlags.Static);

            Assert.IsNotNull(
                definitionsPropertyField);

            DependencyProperty definitionsProperty =
                (DependencyProperty)definitionsPropertyField.GetValue(
                    null);

            DependencyObject dependencyObject =
                (DependencyObject)publishOutput;

            List<UpdateLookupDefinition> definitions =
                (List<UpdateLookupDefinition>)dependencyObject.GetValue(
                    definitionsProperty);

            Assert.IsNotNull(
                definitions);

            Assert.AreEqual(
                0,
                definitions.Count);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Activities_RunPowerShellScript_RunScript_InconsistentListTypes_ShouldThrowFormatException()
        {
            // Arrange
            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.PowerShellVersion =
                PowershellVersion.WindowsPowershell.ToString();

            activity.PowerShellTimeout =
                10000;

            activity.ImpersonatePowerShellUser =
                false;

            const string script =
                "Write-Output @('Text', 42)";

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "RunScript",
                    script,
                    null,
                    null);

                Assert.Fail(
                    "Expected FormatException was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsNotNull(
                    ex.InnerException);

                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(FormatException));

                Assert.AreEqual(
                    "Inconsistent Type",
                    ex.InnerException.Message);
            }
        }

        [TestMethod]
        public void Activities_RunPowerShellScript_Prepare_PowerShellCredentials_ShouldAddLookupsToCache()
        {
            // Arrange
            const string userLookup =
                "[//WorkflowData/PowerShellUser]";

            const string passwordLookup =
                "[//WorkflowData/PowerShellUserPassword]";

            RunPowerShellScript activity =
                new RunPowerShellScript();

            activity.Advanced =
                true;

            activity.ScriptLocation =
                PowerShellScriptLocation.WorkflowDefinition;

            activity.Script =
                "Write-Output 'Test'";

            activity.PowerShellUser =
                userLookup;

            activity.PowerShellUserPassword =
                passwordLookup;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            Assert.IsFalse(
                lookupCache.ContainsKey(userLookup));

            Assert.IsFalse(
                lookupCache.ContainsKey(passwordLookup));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsTrue(
                lookupCache.ContainsKey(userLookup));

            Assert.IsNull(
                lookupCache[userLookup]);

            Assert.IsTrue(
                lookupCache.ContainsKey(passwordLookup));

            Assert.IsNull(
                lookupCache[passwordLookup]);
        }



    }    
}
