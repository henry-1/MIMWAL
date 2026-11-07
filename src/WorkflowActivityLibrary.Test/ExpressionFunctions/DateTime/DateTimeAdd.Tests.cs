using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeAddTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_ResultOutsideDateTimeRange_ShouldThrowInvalidArgumentException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    DateTime.MaxValue,
                    TimeSpan.FromTicks(1)
                },
                new object[]
                {
                    DateTime.MinValue,
                    TimeSpan.FromTicks(-1)
                }
            };

            foreach (object[] parameters in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeAdd",
                            parameters);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_ValidParameters_ShouldReturnExpectedResult()
        {
            DateTime date = new DateTime(
                2026,
                10,
                26,
                10,
                30,
                0);

            object[][] testCases =
            {
                new object[]
                {
                    "Positive TimeSpan",
                    date,
                    new TimeSpan(1, 2, 30, 0),
                    new DateTime(2026, 10, 27, 13, 0, 0)
                },
                new object[]
                {
                    "Negative TimeSpan",
                    date,
                    new TimeSpan(-1, 0, 0),
                    new DateTime(2026, 10, 26, 9, 30, 0)
                },
                new object[]
                {
                    "Zero TimeSpan",
                    date,
                    TimeSpan.Zero,
                    date
                },
                new object[]
                {
                    "Positive TimeSpan string",
                    date,
                    "01:30:00",
                    new DateTime(2026, 10, 26, 12, 0, 0)
                },
                new object[]
                {
                    "Negative TimeSpan string",
                    date,
                    "-01:00:00",
                    new DateTime(2026, 10, 26, 9, 30, 0)
                },
                new object[]
                {
                    "Zero TimeSpan string",
                    date,
                    "00:00:00",
                    date
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                DateTime inputDate = (DateTime)testCase[1];
                object interval = testCase[2];
                DateTime expected = (DateTime)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeAdd",
                    inputDate,
                    interval);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_NullDate_ShouldReturnNull()
        {
            object[] intervals =
            {
                TimeSpan.FromDays(1),
                "01:00:00",
                null
            };

            foreach (object interval in intervals)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeAdd",
                    null,
                    interval);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_InvalidTimeSpanString_ShouldThrowInvalidFunctionFormatException()
        {
            string[] testCases =
            {
                string.Empty,
                "invalid",
                "one day"
            };

            foreach (string interval in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeAdd",
                            new DateTime(2026, 10, 26),
                            interval);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "2026-10-26",
                42,
                42L,
                true,
                new object()
            };

            foreach (object date in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeAdd",
                            date,
                            TimeSpan.FromDays(1));
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_InvalidSecondParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                1,
                1L,
                true,
                new object(),
                DateTime.Now
            };

            foreach (object interval in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeAdd",
                            new DateTime(2026, 10, 26),
                            interval);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    new DateTime(2026, 10, 26),
                    TimeSpan.FromDays(1)
                },
                new object[]
                {
                    new DateTime(2026, 10, 26),
                    "01:00:00"
                },
                new object[]
                {
                    null,
                    TimeSpan.FromDays(1)
                },
                new object[]
                {
                    null,
                    null
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "DateTimeAdd",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "2026-10-26",
                    TimeSpan.FromDays(1)
                },
                new object[]
                {
                    new DateTime(2026, 10, 26),
                    1
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.RunParse(
                            "DateTimeAdd",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }        

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeAdd_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                    TimeSpan.FromDays(1),
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
                            "DateTimeAdd",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}