using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class AddTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_Add_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Positive values",            5L,   3L,    8L },
                new object[] { "Positive and zero",          5L,   0L,    5L },
                new object[] { "Zero and positive",          0L,   5L,    5L },
                new object[] { "Both zero",                  0L,   0L,    0L },

                new object[] { "Positive and negative",      5L,  -3L,    2L },
                new object[] { "Negative and positive",     -5L,   3L,   -2L },
                new object[] { "Both negative",             -5L,  -3L,   -8L },

                // int is compatible with long in VerifyType()
                new object[] { "Integer parameters",          5,    3,     8L },
                new object[] { "Integer and long",            5,    3L,    8L },
                new object[] { "Long and integer",            5L,   3,     8L },

                // null is accepted by VerifyType() and converts to zero
                new object[] { "Null and positive",          null,  5L,    5L },
                new object[] { "Positive and null",           5L,  null,   5L },
                new object[] { "Both null",                  null, null,   0L }               
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
                    "Add",
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
        public void ExpressionFunction_Math_Add_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "Add",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Add_ParseMode_ShouldReturnZero()
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
                    "Add",
                    parameters);

                // Assert
                Assert.AreEqual(0L, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Add_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Add",
                        "5",
                        3L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Add_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 1L },
                new object[] { 1L, 2L, 3L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Add",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Add_Overflow_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    long.MaxValue,
                    1L
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
                            "Add",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}