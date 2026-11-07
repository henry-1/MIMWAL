using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class ModTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_Mod_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Positive values",               10L,  3L,  1L },
                new object[] { "Exactly divisible",             10L,  2L,  0L },
                new object[] { "First smaller than second",      2L,  5L,  2L },
                new object[] { "Zero first parameter",           0L,  5L,  0L },

                new object[] { "Negative first parameter",     -10L,  3L, -1L },
                new object[] { "Negative second parameter",     10L, -3L,  1L },
                new object[] { "Both parameters negative",     -10L, -3L, -1L },

                // int/long compatibility
                new object[] { "Integer parameters",             10,   3,   1L },
                new object[] { "Integer and long",               10,   3L,  1L },
                new object[] { "Long and integer",               10L,  3,   1L },

                // First parameter may be null.
                // VerifyType accepts null and Convert.ToInt64(null) returns 0.
                new object[] { "Null first parameter",           null, 5L,  0L },

                // Boundaries
                new object[] { "Maximum modulo one", long.MaxValue, 1L, 0L },
                new object[] { "Minimum modulo one", long.MinValue, 1L, 0L }
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
                    "Mod",
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
        public void ExpressionFunction_Math_Mod_ZeroSecondParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 10L, 0L },
                new object[] { 10,  0 },
                new object[] { 0L,  0L },
                new object[] { null, 0L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mod",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Mod_NullSecondParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
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
                            "Mod",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Mod_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "10",         3L },
                new object[] { 10L,          "3" },
                new object[] { true,         3L },
                new object[] { 10L,          false },
                new object[] { 10.0,         3L },
                new object[] { 10L,          3.0 },
                new object[] { new object(), 3L },
                new object[] { 10L,          new object() }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mod",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Mod_ParseMode_ShouldReturnZero()
        {
            object[][] testCases =
            {
                new object[] { 10L, 3L },
                new object[] { 10, 3 },
                new object[] { -10L, 3L },
                new object[] { null, 3L },

                // Zero is not checked until after the Parse-mode return.
                new object[] { 10L, 0L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Mod",
                    parameters);

                // Assert
                Assert.AreEqual(0L, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Mod_ParseModeWithNullSecondParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Mod",
                        10L,
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Mod_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Mod",
                        "10",
                        3L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Mod_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 10L },
                new object[] { 10L, 3L, 2L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mod",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}