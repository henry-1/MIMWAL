using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Logic
{
    [TestClass]
    public class BitNotTests
    {
        [TestMethod]
        public void ExpressionFunction_Logic_BitNot_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Zero",                 0L,             -1L },
                new object[] { "One",                  1L,             -2L },
                new object[] { "Five",                 5L,             -6L },
                new object[] { "Minus one",           -1L,              0L },
                new object[] { "Minus two",           -2L,              1L },

                // int is accepted by VerifyType(..., typeof(long))
                new object[] { "Integer zero",          0,              -1L },
                new object[] { "Integer five",          5,              -6L },

                // Boundaries
                new object[] { "Long maximum", long.MaxValue, long.MinValue },
                new object[] { "Long minimum", long.MinValue, long.MaxValue },

                // VerifyType accepts null and Convert.ToInt64(null) is 0
                new object[] { "Null",                 null,            -1L }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                long expected = (long)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "BitNot",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitNot_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "1",
                true,
                1.0,
                1.0f,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "BitNot",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitNot_ParseMode_ShouldReturnZero()
        {
            object[] testCases =
            {
                0L,
                1L,
                -1L,
                5,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "BitNot",
                    parameter);

                // Assert
                Assert.AreEqual(
                    0L,
                    result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitNot_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "BitNot",
                        "1");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitNot_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 1L, 2L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "BitNot",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}