using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class SubtractTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_Subtract_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Positive values",            5L,   3L,    2L },
                new object[] { "Result negative",            3L,   5L,   -2L },
                new object[] { "Equal values",               5L,   5L,    0L },
                new object[] { "Subtract zero",              5L,   0L,    5L },
                new object[] { "Zero minus positive",        0L,   5L,   -5L },

                new object[] { "Positive minus negative",    5L,  -3L,    8L },
                new object[] { "Negative minus positive",   -5L,   3L,   -8L },
                new object[] { "Negative minus negative",   -5L,  -3L,   -2L },

                // int/long compatibility
                new object[] { "Integer parameters",          5,    3,     2L },
                new object[] { "Integer and long",            5,    3L,    2L },
                new object[] { "Long and integer",            5L,   3,     2L },

                // null is accepted by VerifyType()
                // and Convert.ToInt64(null) returns zero
                new object[] { "Null first parameter",       null,  5L,   -5L },
                new object[] { "Null second parameter",       5L,  null,   5L },
                new object[] { "Both parameters null",       null, null,   0L },

                // Boundaries without overflow
                new object[]
                {
                    "Long maximum minus zero",
                    long.MaxValue,
                    0L,
                    long.MaxValue
                },
                new object[]
                {
                    "Long minimum minus zero",
                    long.MinValue,
                    0L,
                    long.MinValue
                }
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
                    "Subtract",
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
        public void ExpressionFunction_Math_Subtract_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "Subtract",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Subtract_ParseMode_ShouldReturnZero()
        {
            object[][] testCases =
            {
                new object[] { 5L, 3L },
                new object[] { 5, 3 },
                new object[] { -5L, 3L },
                new object[] { null, 5L },
                new object[] { 5L, null },
                new object[] { null, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Subtract",
                    parameters);

                // Assert
                Assert.AreEqual(0L, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Subtract_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Subtract",
                        "5",
                        3L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Subtract_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "Subtract",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Subtract_Overflow_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    long.MinValue,
                    1L
                },
                new object[]
                {
                    long.MaxValue,
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
                            "Subtract",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}