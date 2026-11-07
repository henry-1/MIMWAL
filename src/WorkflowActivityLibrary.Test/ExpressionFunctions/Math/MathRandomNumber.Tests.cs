using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class RandomNumTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_ValidRange_ShouldReturnValueWithinRange()
        {
            object[][] testCases =
            {
                new object[] { "Positive range",       1,    10 },
                new object[] { "Range starting at zero", 0,  10 },
                new object[] { "Negative range",      -10,   -1 },
                new object[] { "Mixed range",         -10,   10 },

                // long values are accepted by VerifyType()
                new object[] { "Long range",           1L,   10L },
                new object[] { "Integer and long",     1,    10L },
                new object[] { "Long and integer",     1L,   10 }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object minimum = testCase[1];
                object maximum = testCase[2];

                int expectedMinimum = Convert.ToInt32(minimum);
                int expectedMaximum = Convert.ToInt32(maximum);

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RandomNum",
                    minimum,
                    maximum);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(int),
                    "Failed test case: " + description);

                int resultValue = (int)result;

                Assert.IsTrue(
                    resultValue >= expectedMinimum,
                    "Result was smaller than the minimum. Test case: " +
                    description);

                Assert.IsTrue(
                    resultValue < expectedMaximum,
                    "Result was not smaller than the exclusive maximum. " +
                    "Test case: " +
                    description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_EqualMinimumAndMaximum_ShouldReturnThatValue()
        {
            object[][] testCases =
            {
                new object[] { "Zero",      0,    0,    0 },
                new object[] { "Positive",  5,    5,    5 },
                new object[] { "Negative", -5,   -5,   -5 },
                new object[] { "Long",      5L,   5L,   5 }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object minimum = testCase[1];
                object maximum = testCase[2];
                int expected = (int)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RandomNum",
                    minimum,
                    maximum);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_FullIntegerRange_ShouldReturnValueWithinRange()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "RandomNum",
                int.MinValue,
                int.MaxValue);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(int));

            int resultValue = (int)result;

            Assert.IsTrue(resultValue >= int.MinValue);
            Assert.IsTrue(resultValue < int.MaxValue);
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_MinimumGreaterThanMaximum_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 10, 1 },
                new object[] { 1, 0 },
                new object[] { 0, -1 },
                new object[] { 10L, 1L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RandomNum",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_NullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { null, 10 },
                new object[] { 1, null },
                new object[] { null, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RandomNum",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "1",          10 },
                new object[] { 1,            "10" },
                new object[] { true,         10 },
                new object[] { 1,            false },
                new object[] { 1.0,          10 },
                new object[] { 1,            10.0 },
                new object[] { new object(), 10 },
                new object[] { 1,            new object() }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RandomNum",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_LongOutsideIntegerRange_ShouldThrowOverflowNotAllowedException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    (long)int.MaxValue + 1L,
                    (long)int.MaxValue + 2L
                },
                new object[]
                {
                    (long)int.MinValue - 2L,
                    (long)int.MinValue - 1L
                },
                new object[]
                {
                    0L,
                    (long)int.MaxValue + 1L
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RandomNum",
                            parameters);
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_ParseMode_ShouldReturnZero()
        {
            object[][] testCases =
            {
                new object[] { 1, 10 },
                new object[] { -10, 10 },
                new object[] { 10, 1 },

                // Conversion and range validation are skipped in Parse mode.
                new object[]
                {
                    (long)int.MaxValue + 1L,
                    (long)int.MaxValue + 2L
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "RandomNum",
                    parameters);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_ParseModeWithNullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "RandomNum",
                        null,
                        10);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "RandomNum",
                        "1",
                        10);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_RandomNum_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 1 },
                new object[] { 1, 10, 20 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RandomNum",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}