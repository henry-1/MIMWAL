using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{
    [TestClass]
    public class LessThanTests
    {
        [TestMethod]
        public void ExpressionFunction_Boolean_LessThan_TwoParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "First value smaller",              1,      2,      true },
                new object[] { "First value greater",              2,      1,      false },
                new object[] { "Values equal",                     1,      1,      false },

                new object[] { "Negative value smaller",          -2,     -1,      true },
                new object[] { "Negative value greater",          -1,     -2,      false },
                new object[] { "Equal negative values",           -1,     -1,      false },

                new object[] { "Negative less than zero",         -1,      0,      true },
                new object[] { "Zero less than positive",          0,      1,      true },
                new object[] { "Zero not less than negative",      0,     -1,      false },

                new object[] { "Long values first smaller",        1L,     2L,      true },
                new object[] { "Long values first greater",        2L,     1L,      false },

                new object[] { "Integer less than long",           1,      2L,      true },
                new object[] { "Long less than integer",           1L,     2,       true },

                new object[] { "First parameter null",             null,   1,       false },
                new object[] { "Second parameter null",            1,      null,    false },

                new object[] { "Minimum less than maximum",
                    long.MinValue, long.MaxValue, true },

                new object[] { "Maximum not less than minimum",
                    long.MaxValue, long.MinValue, false }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "LessThan",
                    left,
                    right);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_LessThan_ParseMode_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "LessThan",
                1,
                2);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_LessThan_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 1 },
                new object[] { 1, 2, 3 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LessThan",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_LessThan_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "First parameter string",   "1",          1 },
                new object[] { "Second parameter string",   1,           "1" },
                new object[] { "First parameter boolean",   true,        1 },
                new object[] { "Second parameter boolean",  1,           true },
                new object[] { "First parameter object",    new object(), 1 },
                new object[] { "Second parameter object",   1,           new object() }               
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];

                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LessThan",
                            left,
                            right);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_LessThan_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "LessThan",
                        "1",
                        2);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}