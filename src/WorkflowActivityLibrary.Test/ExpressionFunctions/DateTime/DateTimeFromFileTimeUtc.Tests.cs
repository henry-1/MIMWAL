using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeFromFileTimeUtcTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_ValidParameter_ShouldReturnExpectedResult()
        {
            DateTime date1 = new DateTime(
                2026,
                10,
                26,
                12,
                30,
                0,
                DateTimeKind.Utc);

            DateTime date2 = new DateTime(
                2000,
                1,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            object[][] testCases =
            {
                new object[]
                {
                    "FILETIME zero",
                    0L,
                    DateTime.FromFileTimeUtc(0)
                },
                new object[]
                {
                    "Known 2026 date",
                    date1.ToFileTimeUtc(),
                    date1
                },
                new object[]
                {
                    "Known 2000 date",
                    date2.ToFileTimeUtc(),
                    date2
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                DateTime expected = (DateTime)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "DateTimeFromFileTimeUtc",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_IntegerParameter_ShouldReturnExpectedResult()
        {
            // Arrange
            int parameter = 0;

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeFromFileTimeUtc",
                parameter);

            // Assert
            Assert.AreEqual(
                DateTime.FromFileTimeUtc(0),
                result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeFromFileTimeUtc",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_InvalidFileTime_ShouldThrowInvalidArgumentException()
        {
            long[] testCases =
            {
                -1L,
                long.MaxValue,
                long.MinValue
            };

            foreach (long parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFromFileTimeUtc",
                            parameter);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "0",
                true,
                DateTime.Now,
                1.0,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFromFileTimeUtc",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                0,
                0L,
                1000L,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "DateTimeFromFileTimeUtc",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "DateTimeFromFileTimeUtc",
                        "0");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeFromFileTimeUtc_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 0L, 1L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeFromFileTimeUtc",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}