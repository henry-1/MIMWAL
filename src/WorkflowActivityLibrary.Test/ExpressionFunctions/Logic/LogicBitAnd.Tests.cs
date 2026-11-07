using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Logic
{
    [TestClass]
    public class BitAndTests
    {
        [TestMethod]
        public void ExpressionFunction_Logic_BitAnd_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Both zero",                  0L,   0L,   0L },
                new object[] { "Zero and value",             0L,   5L,   0L },
                new object[] { "Same value",                 5L,   5L,   5L },

                // 0101 & 0011 = 0001
                new object[] { "Partial bit overlap",        5L,   3L,   1L },

                // 1100 & 1010 = 1000
                new object[] { "Different bit masks",       12L,  10L,   8L },

                // 0101 & 1010 = 0000
                new object[] { "No bit overlap",             5L,  10L,   0L },

                // int/long compatibility from VerifyType()
                new object[] { "Integer parameters",          5,    3,    1L },
                new object[] { "Integer and long",            5,    3L,   1L },
                new object[] { "Long and integer",            5L,   3,    1L },

                // Negative values
                new object[] { "Minus one and value",        -1L,   5L,   5L },
                new object[] { "Both minus one",             -1L,  -1L,  -1L },

                // Boundaries
                new object[]
                {
                    "Maximum and minimum",
                    long.MaxValue,
                    long.MinValue,
                    0L
                },
                new object[]
                {
                    "Maximum and minus one",
                    long.MaxValue,
                    -1L,
                    long.MaxValue
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
                    "BitAnd",
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
        public void ExpressionFunction_Logic_BitAnd_NullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { null, 1L },
                new object[] { 1L, null },
                new object[] { null, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "BitAnd",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitAnd_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "5",  3L },
                new object[] { 5L,   "3" },
                new object[] { true,  3L },
                new object[] { 5L,    false },
                new object[] { 5.0,   3L },
                new object[] { 5L,    3.0 },
                new object[] { new object(), 3L },
                new object[] { 5L, new object() }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "BitAnd",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitAnd_ParseMode_ShouldReturnZero()
        {
            object[][] testCases =
            {
                new object[] { 5L, 3L },
                new object[] { 5, 3 },
                new object[] { -1L, 5L }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "BitAnd",
                    parameters);

                // Assert
                Assert.AreEqual(0L, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitAnd_ParseModeWithNullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "BitAnd",
                        null,
                        1L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitAnd_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "BitAnd",
                        "5",
                        3L);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Logic_BitAnd_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "BitAnd",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}