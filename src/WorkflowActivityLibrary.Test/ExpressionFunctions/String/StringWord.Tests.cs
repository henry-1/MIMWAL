using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class WordTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Word_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "First word",
                    "One Two Three",
                    0,
                    " ",
                    "One"
                },
                new object[]
                {
                    "Middle word",
                    "One Two Three",
                    1,
                    " ",
                    "Two"
                },
                new object[]
                {
                    "Last word",
                    "One Two Three",
                    2,
                    " ",
                    "Three"
                },
                new object[]
                {
                    "Custom separator",
                    "One;Two;Three",
                    1,
                    ";",
                    "Two"
                },
                new object[]
                {
                    "Index equal to item count",
                    "One Two Three",
                    3,
                    " ",
                    null
                },
                new object[]
                {
                    "Index greater than item count",
                    "One Two Three",
                    10,
                    " ",
                    null
                },

                // Split(char) retains empty entries.
                new object[]
                {
                    "Empty item between separators",
                    "One;;Three",
                    1,
                    ";",
                    string.Empty
                },
                new object[]
                {
                    "Leading separator",
                    ";One;Two",
                    0,
                    ";",
                    string.Empty
                },
                new object[]
                {
                    "Trailing separator",
                    "One;Two;",
                    2,
                    ";",
                    string.Empty
                },

                // First parameter has no type restriction.
                new object[]
                {
                    "Integer first parameter",
                    12345,
                    0,
                    "-",
                    "12345"
                },

                // VerifyType(int) accepts compatible long values.
                new object[]
                {
                    "Long index",
                    "One Two Three",
                    1L,
                    " ",
                    "Two"
                },

                // Any parameter whose ToString() has length 1
                // can act as separator.
                new object[]
                {
                    "Integer separator",
                    "101",
                    1,
                    0,
                    "1"
                },

                // Null first parameter is valid.
                new object[]
                {
                    "Null first parameter",
                    null,
                    0,
                    " ",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object index = testCase[2];
                object separator = testCase[3];
                string expected = (string)testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Word",
                    value,
                    index,
                    separator);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_NullIndex_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Word",
                        "One Two Three",
                        null,
                        " ");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_InvalidIndexType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "1",
                true,
                1.0,
                new object()
            };

            foreach (object index in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Word",
                            "One Two Three",
                            index,
                            " ");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_NullSeparator_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Word",
                        "One Two Three",
                        1,
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_InvalidSeparator_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                string.Empty,
                "  ",
                "abc",
                10,
                true
            };

            foreach (object separator in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Word",
                            "One Two Three",
                            1,
                            separator);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_NegativeIndex_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                -1,
                -10,
                -1L
            };

            foreach (object index in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Word",
                            "One Two Three",
                            index,
                            " ");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
        {
            object[] testCases =
            {
                (long)int.MaxValue + 1L,
                (long)int.MinValue - 1L
            };

            foreach (object index in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Word",
                            "One Two Three",
                            index,
                            " ");
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Word",
                "One Two Three",
                1,
                " ");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_ParseModeWithNegativeIndex_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Word",
                "One Two Three",
                -1,
                " ");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_ParseModeWithInvalidIndexType_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Word",
                        "One Two Three",
                        "1",
                        " ");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_ParseModeWithInvalidSeparator_ShouldThrowInvalidFunctionFormatException()
        {
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Word",
                        "One Two Three",
                        1,
                        "xx");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Word_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "One Two Three" },
                new object[] { "One Two Three", 1 },
                new object[] { "One Two Three", 1, " ", "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Word",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}