using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeToFileTimeUtcTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Known 2026 date",
                    new DateTime(
                        2026,
                        10,
                        26,
                        12,
                        30,
                        0,
                        DateTimeKind.Utc)
                },
                new object[]
                {
                    "Known 2000 date",
                    new DateTime(
                        2000,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc)
                },
                new object[]
                {
                    "FILETIME epoch",
                    new DateTime(
                        1601,
                        1,
                        1,
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
                DateTime parameter = (DateTime)testCase[1];

                long expected = parameter.ToFileTimeUtc();

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeToFileTimeUtc",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(long),
                    "Failed test case: " + description);

                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeToFileTimeUtc",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_InvalidDate_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "DateTimeToFileTimeUtc",
                        DateTime.MinValue);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "2026-10-26",
                42,
                42L,
                true,
                TimeSpan.Zero,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeToFileTimeUtc",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                new DateTime(
                    2026,
                    10,
                    26,
                    12,
                    30,
                    0,
                    DateTimeKind.Utc),

                DateTime.MinValue,

                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "DateTimeToFileTimeUtc",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeToFileTimeUtc",
                        "2026-10-26");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeToFileTimeUtc_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new DateTime(2026, 10, 26),
                    new DateTime(2026, 10, 27)
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeToFileTimeUtc",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}