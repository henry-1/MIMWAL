using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class LeftPadTests
    {
        [TestMethod]
        public void ExpressionFunction_String_LeftPad_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Pad string",
                    "123",
                    5,
                    "0",
                    "00123"
                },
                new object[]
                {
                    "Already required length",
                    "12345",
                    5,
                    "0",
                    "12345"
                },
                new object[]
                {
                    "Value longer than requested length",
                    "123456",
                    5,
                    "0",
                    "123456"
                },
                new object[]
                {
                    "Different padding character",
                    "abc",
                    6,
                    "*",
                    "***abc"
                },
                new object[]
                {
                    "Space padding",
                    "abc",
                    5,
                    " ",
                    "  abc"
                },

                // First parameter has no type restriction.
                new object[]
                {
                    "Integer value",
                    123,
                    5,
                    "0",
                    "00123"
                },
                new object[]
                {
                    "Boolean value",
                    true,
                    6,
                    "_",
                    "__True"
                },

                // VerifyType(int) accepts compatible long values.
                new object[]
                {
                    "Long length",
                    "123",
                    5L,
                    "0",
                    "00123"
                },

                // Any non-null object whose ToString() has length 1
                // is accepted as the padding character.
                new object[]
                {
                    "Integer padding character",
                    "12",
                    5,
                    0,
                    "00012"
                },

                // Null first parameter returns null.
                new object[]
                {
                    "Null value",
                    null,
                    5,
                    "0",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object length = testCase[2];
                object padding = testCase[3];
                string expected = (string)testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "LeftPad",
                    value,
                    length,
                    padding);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_NullLength_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "LeftPad",
                        "123",
                        null,
                        "0");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_NullPadding_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "LeftPad",
                        "123",
                        5,
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_InvalidLengthType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "5",
                true,
                5.0,
                new object()
            };

            foreach (object length in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LeftPad",
                            "123",
                            length,
                            "0");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_InvalidPadding_ShouldThrowInvalidInvalidArgumentException()
        {
            object[] testCases =
            {
                string.Empty,
                "00",
                "abc",
                10,
                true
            };

            foreach (object padding in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LeftPad",
                            "123",
                            5,
                            padding);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_NegativeLength_ShouldThrowInvalidArgumentException()
        {
            object[] testCases =
            {
                -1,
                -10,
                -1L
            };

            foreach (object length in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LeftPad",
                            "123",
                            length,
                            "0");
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
        {
            object[] testCases =
            {
                (long)int.MaxValue + 1L,
                (long)int.MinValue - 1L
            };

            foreach (object length in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LeftPad",
                            "123",
                            length,
                            "0");
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "LeftPad",
                "123",
                5,
                "0");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_ParseModeWithNullLength_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "LeftPad",
                        "123",
                        null,
                        "0");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_ParseModeWithNullPadding_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "LeftPad",
                        "123",
                        5,
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_ParseModeWithInvalidLengthType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "LeftPad",
                        "123",
                        "5",
                        "0");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_LeftPad_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "123" },
                new object[] { "123", 5 },
                new object[] { "123", 5, "0", "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LeftPad",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}