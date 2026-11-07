using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertToNumberTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                // Strings
                new object[] { "Positive string",    "42",   42L },
                new object[] { "Negative string",   "-42",  -42L },
                new object[] { "Zero string",        "0",     0L },

                // Boolean
                new object[] { "Boolean true",       true,    1L },
                new object[] { "Boolean false",      false,   0L },

                // Integer
                new object[] { "Positive integer",   42,     42L },
                new object[] { "Negative integer",  -42,    -42L },
                new object[] { "Zero integer",        0,      0L },

                // Long
                new object[] { "Positive long",      42L,    42L },
                new object[] { "Negative long",     -42L,   -42L },
                new object[] { "Zero long",           0L,     0L },
                new object[] { "Long minimum",        long.MinValue, long.MinValue },
                new object[] { "Long maximum",        long.MaxValue, long.MaxValue },

                // Null
                new object[] { "Null",                null,    0L }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                long expected = (long)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertToNumber",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_InvalidString_ShouldThrowFormatException()
        {
            string[] testCases =
            {
                string.Empty,
                "Test",
                "1.5",
                "true"
            };

            foreach (string parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToNumber",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_StringOutsideLongRange_ShouldThrowInvalidFunctionFormatException()
        {
            string[] testCases =
            {
                "9223372036854775808",
                "-9223372036854775809"
            };

            foreach (string parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToNumber",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "ConvertToNumber",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_ParseMode_ShouldReturnZero()
        {
            object[] testCases =
            {
                "42",
                true,
                false,
                42,
                42L,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertToNumber",
                    parameter);

                // Assert
                Assert.AreEqual(
                    0L,
                    result,
                    "Failed for parameter: " +
                    (parameter == null ? "null" : parameter.ToString()));
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertToNumber",
                        1.0);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToNumber_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 1, 2 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToNumber",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}