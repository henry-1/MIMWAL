using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeSubtractTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_ValidParameters_ShouldReturnExpectedResult()
        {
            DateTime date = new DateTime(
                2026,
                10,
                26,
                12,
                30,
                0);

            object[][] testCases =
            {
                new object[]
                {
                    "One day difference",
                    date,
                    date.AddDays(-1),
                    TimeSpan.FromDays(1)
                },
                new object[]
                {
                    "Negative one day difference",
                    date,
                    date.AddDays(1),
                    TimeSpan.FromDays(-1)
                },
                new object[]
                {
                    "One hour difference",
                    date,
                    date.AddHours(-1),
                    TimeSpan.FromHours(1)
                },
                new object[]
                {
                    "One minute difference",
                    date,
                    date.AddMinutes(-1),
                    TimeSpan.FromMinutes(1)
                },
                new object[]
                {
                    "One second difference",
                    date,
                    date.AddSeconds(-1),
                    TimeSpan.FromSeconds(1)
                },
                new object[]
                {
                    "Equal dates",
                    date,
                    date,
                    TimeSpan.Zero
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                DateTime minuend = (DateTime)testCase[1];
                DateTime subtrahend = (DateTime)testCase[2];
                TimeSpan expected = (TimeSpan)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeSubtract",
                    minuend,
                    subtrahend);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(TimeSpan),
                    "Failed test case: " + description);

                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_NullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            DateTime date = new DateTime(
                2026,
                10,
                26);

            object[][] testCases =
            {
                new object[]
                {
                    null,
                    date
                },
                new object[]
                {
                    date,
                    null
                },
                new object[]
                {
                    null,
                    null
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeSubtract",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            DateTime date = new DateTime(
                2026,
                10,
                26);

            object[][] testCases =
            {
                new object[]
                {
                    "2026-10-26",
                    date
                },
                new object[]
                {
                    date,
                    "2026-10-26"
                },
                new object[]
                {
                    42,
                    date
                },
                new object[]
                {
                    date,
                    42
                },
                new object[]
                {
                    true,
                    date
                },
                new object[]
                {
                    date,
                    true
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeSubtract",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "DateTimeSubtract",
                new DateTime(2026, 10, 26),
                new DateTime(2026, 10, 25));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_ParseModeWithNullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeSubtract",
                        null,
                        new DateTime(2026, 10, 26));
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeSubtract",
                        "2026-10-26",
                        new DateTime(2026, 10, 25));
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeSubtract_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                    new DateTime(2026, 10, 25),
                    new DateTime(2026, 10, 24)
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeSubtract",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}