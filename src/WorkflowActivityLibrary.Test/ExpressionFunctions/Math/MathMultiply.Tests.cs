using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class MultiplyTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_Multiply_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Positive values",           5L,   3L,   15L },
                new object[] { "Positive and zero",         5L,   0L,    0L },
                new object[] { "Zero and positive",         0L,   5L,    0L },
                new object[] { "Both zero",                 0L,   0L,    0L },

                new object[] { "Positive and negative",     5L,  -3L,  -15L },
                new object[] { "Negative and positive",    -5L,   3L,  -15L },
                new object[] { "Both negative",            -5L,  -3L,   15L },

                // int/long compatibility
                new object[] { "Integer parameters",         5,    3,    15L },
                new object[] { "Integer and long",           5,    3L,   15L },
                new object[] { "Long and integer",           5L,   3,    15L },

                // null is accepted by VerifyType()
                // and Convert.ToInt64(null) returns zero
                new object[] { "Null first parameter",      null,  5L,    0L },
                new object[] { "Null second parameter",      5L,  null,   0L },
                new object[] { "Both parameters null",      null, null,   0L },

                // Boundaries without overflow
                new object[] { "Long maximum times one",    long.MaxValue,  1L, long.MaxValue },
                new object[] { "Long minimum times one",    long.MinValue,  1L, long.MinValue },
                new object[] { "Long maximum times zero",   long.MaxValue,  0L, 0L }
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
                    "Multiply",
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
        public void ExpressionFunction_Math_Multiply_Overflow_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    long.MaxValue,
                    2L
                },
                new object[]
                {
                    long.MinValue,
                    2L
                },
                new object[]
                {
                    long.MaxValue,
                    long.MaxValue
                },
                new object[]
                {
                    long.MinValue,
                    -1L
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Multiply",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Multiply_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "5",          3L },
                new object[] { 5L,           "3" },
                new object[] { true,         3L },
                new object[] { 5L,           false },
                new object[] { 5.0,          3L },
                new object[] { 5L,           3.0 },
                new object[] { new object(), 3L },
                new object[] { 5L,           new object() }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Multiply",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Multiply_ParseMode_ShouldReturnZero()
        {
            object[][] testCases =
            {
                new object[] { 5L, 3L },
                new object[] { 5, 3 },
                new object[] { -5L, 3L },
                new object[] { null, 5L },
                new object[] { 5L, null },
                new object[] { null, null },

                // Would overflow during Resolve,
                // but multiplication is not executed in Parse mode.
                new object[] { long.MaxValue, 2L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Multiply",
                    parameters);

                // Assert
                Assert.AreEqual(0L, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Multiply_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Multiply",
                        "5",
                        3L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Multiply_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 5L },
                new object[] { 5L, 3L, 2L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Multiply",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}