using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class LeftTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Left_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Length shorter than string",
                    "Hello World",
                    5,
                    "Hello"
                },
                new object[]
                {
                    "Length equal to string",
                    "Hello",
                    5,
                    "Hello"
                },
                new object[]
                {
                    "Length longer than string",
                    "Hello",
                    10,
                    "Hello"
                },
                new object[]
                {
                    "Zero length",
                    "Hello",
                    0,
                    string.Empty
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    5,
                    string.Empty
                },
                new object[]
                {
                    "Trailing whitespace is trimmed",
                    "Hello   World",
                    8,
                    "Hello"
                },

                // First parameter has no type restriction.
                new object[]
                {
                    "Integer first parameter",
                    12345,
                    3,
                    "123"
                },
                new object[]
                {
                    "Boolean first parameter",
                    true,
                    4,
                    "True"
                },

                // VerifyType(int) accepts compatible long values.
                new object[]
                {
                    "Long length",
                    "Hello",
                    3L,
                    "Hel"
                },

                // Null first parameter returns null.
                new object[]
                {
                    "Null first parameter",
                    null,
                    5,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object length = testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Left",
                    value,
                    length);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_NullLength_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Left",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_InvalidLengthType_ShouldThrowInvalidFunctionFormatException()
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
                            "Left",
                            "Hello",
                            length);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_NegativeLength_ShouldThrowInvalidArgumentException()
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
                            "Left",
                            "Hello",
                            length);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
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
                            "Left",
                            "Hello",
                            length);
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Left",
                "Hello",
                3);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_ParseModeWithNullLength_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Left",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_ParseModeWithInvalidLengthType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Left",
                        "Hello",
                        "3");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Left_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "Hello" },
                new object[] { "Hello", 3, "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Left",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}