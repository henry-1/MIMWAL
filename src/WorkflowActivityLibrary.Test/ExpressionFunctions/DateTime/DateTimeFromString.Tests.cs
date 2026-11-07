using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeFromStringTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "ISO date",
                    "2026-10-26",
                    new DateTime(
                        2026,
                        10,
                        26,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc)
                },
                new object[]
                {
                    "ISO date and time",
                    "2026-10-26 14:30:00",
                    new DateTime(
                        2026,
                        10,
                        26,
                        14,
                        30,
                        0,
                        DateTimeKind.Utc)
                },
                new object[]
                {
                    "ISO date and time with Z",
                    "2026-10-26T14:30:00Z",
                    new DateTime(
                        2026,
                        10,
                        26,
                        14,
                        30,
                        0,
                        DateTimeKind.Utc)
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string parameter = (string)testCase[1];
                DateTime expected = (DateTime)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeFromString",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_Culture_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "German",
                    "26.10.2026",
                    "de-DE",
                    new DateTime(
                        2026,
                        10,
                        26,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc)
                },
                new object[]
                {
                    "US",
                    "10/26/2026",
                    "en-US",
                    new DateTime(
                        2026,
                        10,
                        26,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc)
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string parameter = (string)testCase[1];
                string culture = (string)testCase[2];
                DateTime expected = (DateTime)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeFromString",
                    parameter,
                    culture);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeFromString",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_InvalidDate_ShouldThrowInvalidFunctionFormatException()
        {
            string[] testCases =
            {
                string.Empty,
                "invalid",
                "not-a-date",
                "2026-99-99"
            };

            foreach (string parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFromString",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new DateTime(2026, 10, 26),
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFromString",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_InvalidCulture_ShouldThrowInvalidArgumentException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "DateTimeFromString",
                        "2026-10-26",
                        "invalid-culture");
                },
                typeof(InvalidArgumentException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_InvalidCultureParameterType_ShouldThrowInvalidArgumentException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "DateTimeFromString",
                        "2026-10-26",
                        42);
                },
                typeof(InvalidArgumentException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "2026-10-26"
                },
                new object[]
                {
                    "26.10.2026",
                    "de-DE"
                },
                new object[]
                {
                    null
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "DateTimeFromString",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeFromString",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    "2026-10-26",
                    "en-US",
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
                            "DateTimeFromString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}