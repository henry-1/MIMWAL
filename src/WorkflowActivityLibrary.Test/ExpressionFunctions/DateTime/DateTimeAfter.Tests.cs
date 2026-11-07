using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeAfterTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_After_ValidParameters_ShouldReturnExpectedResult()
        {
            DateTime earlier = new DateTime(2026, 10, 25, 10, 0, 0);
            DateTime later = new DateTime(2026, 10, 26, 10, 0, 0);

            object[][] testCases =
            {
                new object[]
                {
                    "First date after second date",
                    later,
                    earlier,
                    true
                },
                new object[]
                {
                    "First date before second date",
                    earlier,
                    later,
                    false
                },
                new object[]
                {
                    "Dates equal",
                    earlier,
                    earlier,
                    false
                },
                new object[]
                {
                    "First parameter null",
                    null,
                    earlier,
                    false
                },
                new object[]
                {
                    "Second parameter null",
                    later,
                    null,
                    false
                },
                new object[]
                {
                    "Both parameters null",
                    null,
                    null,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object first = testCase[1];
                object second = testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "After",
                    first,
                    second);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_After_ParseMode_ShouldReturnFalse()
        {
            object[][] testCases =
            {
                new object[]
                {
                    new DateTime(2026, 10, 26),
                    new DateTime(2026, 10, 25)
                },
                new object[]
                {
                    new DateTime(2026, 10, 25),
                    new DateTime(2026, 10, 26)
                },
                new object[]
                {
                    null,
                    new DateTime(2026, 10, 26)
                },
                new object[]
                {
                    new DateTime(2026, 10, 26),
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
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "After",
                    parameters);

                // Assert
                Assert.AreEqual(false, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_After_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Invalid first parameter",
                    "2026-10-26",
                    new DateTime(2026, 10, 25)
                },
                new object[]
                {
                    "Invalid second parameter",
                    new DateTime(2026, 10, 26),
                    "2026-10-25"
                },
                new object[]
                {
                    "Both parameters invalid",
                    "2026-10-26",
                    "2026-10-25"
                },
                new object[]
                {
                    "Integer first parameter",
                    42,
                    new DateTime(2026, 10, 25)
                },
                new object[]
                {
                    "Boolean second parameter",
                    new DateTime(2026, 10, 26),
                    true
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                object first = testCase[1];
                object second = testCase[2];

                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "After",
                            first,
                            second);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_After_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "2026-10-26",
                    new DateTime(2026, 10, 25)
                },
                new object[]
                {
                    new DateTime(2026, 10, 26),
                    "2026-10-25"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.RunParse(
                            "After",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_After_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "After",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}