using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeUtcToLocalTimeTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_WithoutTimeZone_ShouldReturnLocalTime()
        {
            // Arrange
            DateTime utc = new DateTime(
                2026,
                10,
                26,
                12,
                0,
                0,
                DateTimeKind.Utc);

            DateTime expected = utc.ToLocalTime();

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeUtcToLocalTime",
                utc);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_EmptyTimeZone_ShouldReturnLocalTime()
        {
            DateTime utc = new DateTime(
                2026,
                10,
                26,
                12,
                0,
                0,
                DateTimeKind.Utc);

            object[][] testCases =
            {
                new object[] { "Null time zone", null },
                new object[] { "Empty time zone", string.Empty }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object timeZone = testCase[1];
                DateTime expected = utc.ToLocalTime();

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeUtcToLocalTime",
                    utc,
                    timeZone);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_ValidTimeZone_ShouldReturnExpectedResult()
        {
            // Arrange
            DateTime utc = new DateTime(
                2026,
                10,
                26,
                12,
                0,
                0,
                DateTimeKind.Utc);

            string timeZoneId = TimeZoneInfo.Utc.Id;

            DateTime expected =
                TimeZoneInfo.ConvertTimeFromUtc(
                    utc,
                    TimeZoneInfo.Utc);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeUtcToLocalTime",
                utc,
                timeZoneId);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_NullDate_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[] { null },
                new object[] { null, TimeZoneInfo.Utc.Id }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeUtcToLocalTime",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "DateTimeUtcToLocalTime",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_InvalidTimeZone_ShouldThrowInvalidFunctionFormatException()
        {
            // Arrange
            string timeZoneId =
                "This-Time-Zone-Does-Not-Exist";

            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "DateTimeUtcToLocalTime",
                        new DateTime(
                            2026,
                            10,
                            26,
                            12,
                            0,
                            0,
                            DateTimeKind.Utc),
                        timeZoneId);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    new DateTime(
                        2026,
                        10,
                        26,
                        12,
                        0,
                        0,
                        DateTimeKind.Utc)
                },
                new object[]
                {
                    new DateTime(
                        2026,
                        10,
                        26,
                        12,
                        0,
                        0,
                        DateTimeKind.Utc),
                    TimeZoneInfo.Utc.Id
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
                    "DateTimeUtcToLocalTime",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeUtcToLocalTime",
                        "2026-10-26");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeUtcToLocalTime_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new DateTime(2026, 10, 26),
                    TimeZoneInfo.Utc.Id,
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
                            "DateTimeUtcToLocalTime",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}