using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class DivideTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_Divide_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Exact division",                 10L,   2L,   5L },
                new object[] { "Division with remainder",        10L,   3L,   3L },
                new object[] { "Numerator smaller",               2L,   5L,   0L },
                new object[] { "Zero numerator",                  0L,   5L,   0L },

                new object[] { "Negative numerator",            -10L,   2L,  -5L },
                new object[] { "Negative denominator",           10L,  -2L,  -5L },
                new object[] { "Both negative",                 -10L,  -2L,   5L },

                new object[] { "Negative remainder",            -10L,   3L,  -3L },
                new object[] { "Negative denominator remainder", 10L,  -3L,  -3L },

                // int/long compatibility
                new object[] { "Integer parameters",              10,    2,    5L },
                new object[] { "Integer and long",                10,    2L,   5L },
                new object[] { "Long and integer",                10L,   2,    5L },

                // null is accepted by VerifyType and converts to zero
                new object[] { "Null numerator",                  null,  5L,   0L }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object first = testCase[1];
                object second = testCase[2];
                long expected = (long)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Divide",
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
        public void ExpressionFunction_Math_Divide_ZeroDenominator_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 10L, 0L },
                new object[] { 10,  0 },
                new object[] { 0L,  0L },

                // null converts to zero
                new object[] { 10L, null },
                new object[] { null, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Divide",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Divide_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "10",         2L },
                new object[] { 10L,          "2" },
                new object[] { true,         2L },
                new object[] { 10L,          false },
                new object[] { 10.0,         2L },
                new object[] { 10L,          2.0 },
                new object[] { new object(), 2L },
                new object[] { 10L,          new object() }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Divide",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Divide_ParseMode_ShouldReturnZero()
        {
            object[][] testCases =
            {
                new object[] { 10L, 2L },
                new object[] { 10, 2 },
                new object[] { -10L, 2L },
                new object[] { null, 2L },

                // Division by zero is not evaluated in Parse mode
                new object[] { 10L, 0L },
                new object[] { 10L, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Divide",
                    parameters);

                // Assert
                Assert.AreEqual(0L, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Divide_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Divide",
                        "10",
                        2L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Divide_Overflow_ShouldThrowOverflowNotAllowedException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Divide",
                        long.MinValue,
                        -1L);
                },
                typeof(OverflowNotAllowedException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Divide_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 10L },
                new object[] { 10L, 2L, 1L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Divide",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}