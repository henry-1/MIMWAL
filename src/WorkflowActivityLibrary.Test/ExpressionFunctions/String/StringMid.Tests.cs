using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class MidTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Mid_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Substring from middle",
                    "Hello World",
                    6,
                    5,
                    "World"
                },
                new object[]
                {
                    "Substring from beginning",
                    "Hello World",
                    0,
                    5,
                    "Hello"
                },
                new object[]
                {
                    "Length extends beyond end",
                    "Hello World",
                    6,
                    100,
                    "World"
                },
                new object[]
                {
                    "Length exactly reaches end",
                    "Hello World",
                    6,
                    5,
                    "World"
                },
                new object[]
                {
                    "Zero length",
                    "Hello World",
                    6,
                    0,
                    string.Empty
                },
                new object[]
                {
                    "Start index equals string length",
                    "Hello",
                    5,
                    1,
                    null
                },
                new object[]
                {
                    "Start index greater than string length",
                    "Hello",
                    10,
                    1,
                    null
                },

                // First parameter has no type restriction.
                new object[]
                {
                    "Integer first parameter",
                    12345,
                    1,
                    3,
                    "234"
                },

                // int/long compatibility from VerifyType().
                new object[]
                {
                    "Long start and length",
                    "Hello World",
                    6L,
                    5L,
                    "World"
                },

                // Null first parameter is valid.
                new object[]
                {
                    "Null first parameter",
                    null,
                    0,
                    5,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object startIndex = testCase[2];
                object length = testCase[3];
                string expected = (string)testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Mid",
                    value,
                    startIndex,
                    length);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_NullStartIndex_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Mid",
                        "Hello",
                        null,
                        2);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_NullLength_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Mid",
                        "Hello",
                        1,
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_InvalidStartIndexType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "1",
                true,
                1.0,
                new object()
            };

            foreach (object startIndex in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mid",
                            "Hello",
                            startIndex,
                            2);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_InvalidLengthType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "2",
                true,
                2.0,
                new object()
            };

            foreach (object length in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mid",
                            "Hello",
                            1,
                            length);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_NegativeValue_ShouldThrowInvalidArgumentException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Negative start index",
                    -1,
                    2
                },
                new object[]
                {
                    "Negative length",
                    1,
                    -1
                }
            };

            foreach (object[] testCase in testCases)
            {
                object startIndex = testCase[1];
                object length = testCase[2];

                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mid",
                            "Hello",
                            startIndex,
                            length);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    (long)int.MaxValue + 1L,
                    1
                },
                new object[]
                {
                    0,
                    (long)int.MaxValue + 1L
                }
            };

            foreach (object[] parameters in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mid",
                            "Hello",
                            parameters[0],
                            parameters[1]);
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Mid",
                "Hello World",
                6,
                5);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_ParseModeWithInvalidParameter_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Mid",
                        "Hello",
                        "1",
                        2);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Mid_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "Hello" },
                new object[] { "Hello", 1 },
                new object[] { "Hello", 1, 2, "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Mid",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}