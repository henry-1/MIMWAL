using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertToBooleanTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBoolean_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                // Boolean values
                new object[] { "Boolean true",      true,    true },
                new object[] { "Boolean false",     false,   false },

                // String values
                new object[] { "String true",       "true",  true },
                new object[] { "String false",      "false", false },
                new object[] { "Uppercase true",    "TRUE",  true },
                new object[] { "Uppercase false",   "FALSE", false },
                new object[] { "Mixed-case true",   "TrUe",  true },
                new object[] { "Mixed-case false",  "FaLsE", false },

                // Integer values
                new object[] { "Integer zero",      0,       false },
                new object[] { "Positive integer",  1,       true },
                new object[] { "Negative integer", -1,       true },

                // Long values
                new object[] { "Long zero",         0L,      false },
                new object[] { "Positive long",     1L,      true },
                new object[] { "Negative long",    -1L,      true },

                // Null is accepted by VerifyType
                new object[] { "Null",              null,    false }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertToBoolean",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBoolean_InvalidString_ShouldThrowInvalidFunctionFormatException()
        {
            string[] testCases =
            {
                string.Empty,
                "Test",
                "yes",
                "no",
                "1",
                "0"
            };

            foreach (string parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToBoolean",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBoolean_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                1.0,
                1.0f,
                1.0m,
                DateTime.Now,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToBoolean",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBoolean_ParseMode_ShouldReturnFalse()
        {
            object[] testCases =
            {
                true,
                false,
                "true",
                "false",
                1,
                0,
                -1,
                1L,
                0L,
                -1L,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertToBoolean",
                    parameter);

                // Assert
                Assert.AreEqual(
                    false,
                    result,
                    "Failed for parameter: " +
                    (parameter == null ? "null" : parameter.ToString()));
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBoolean_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                1.0,
                1.0f,
                DateTime.Now,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.RunParse(
                            "ConvertToBoolean",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBoolean_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { true, false }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToBoolean",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}