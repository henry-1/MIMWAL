using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeFormatTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_ValidParameters_ShouldReturnExpectedResult()
        {
            DateTime date = new DateTime(
                2026,
                10,
                26,
                14,
                35,
                42);

            object[][] testCases =
            {
                new object[]
                {
                    "Date",
                    date,
                    "yyyy-MM-dd",
                    "2026-10-26"
                },
                new object[]
                {
                    "Time",
                    date,
                    "HH:mm:ss",
                    "14:35:42"
                },
                new object[]
                {
                    "Date and time",
                    date,
                    "yyyy-MM-dd HH:mm:ss",
                    "2026-10-26 14:35:42"
                },
                new object[]
                {
                    "Year",
                    date,
                    "yyyy",
                    "2026"
                },
                new object[]
                {
                    "Month",
                    date,
                    "MM",
                    "10"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                string format = (string)testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeFormat",
                    parameter,
                    format);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_Culture_ShouldReturnExpectedResult()
        {
            DateTime date = new DateTime(
                2026,
                10,
                26,
                14,
                35,
                42);

            object[][] testCases =
            {
                new object[]
                {
                    "German",
                    "de-DE",
                    "dd.MM.yyyy",
                    "26.10.2026"
                },
                new object[]
                {
                    "US",
                    "en-US",
                    "MM/dd/yyyy",
                    "10/26/2026"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string culture = (string)testCase[1];
                string format = (string)testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeFormat",
                    date,
                    format,
                    culture);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_NullDate_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeFormat",
                null,
                "yyyy-MM-dd");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_NullFormat_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "DateTimeFormat",
                        new DateTime(2026, 10, 26),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "2026-10-26",
                42,
                42L,
                true,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFormat",
                            parameter,
                            "yyyy-MM-dd");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_InvalidCulture_ShouldThrowInvalidArgumentException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "DateTimeFormat",
                        new DateTime(2026, 10, 26),
                        "yyyy-MM-dd",
                        "invalid-culture");
                },
                typeof(InvalidArgumentException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    new DateTime(2026, 10, 26),
                    "yyyy-MM-dd"
                },
                new object[]
                {
                    null,
                    "yyyy-MM-dd"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "DateTimeFormat",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_ParseModeWithNullFormat_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeFormat",
                        new DateTime(2026, 10, 26),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeFormat",
                        "2026-10-26",
                        "yyyy-MM-dd");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFormat_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new DateTime(2026, 10, 26)
                },

                new object[]
                {
                    new DateTime(2026, 10, 26),
                    "yyyy-MM-dd",
                    "de-DE",
                    "Extra"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFormat",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}