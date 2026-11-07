using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class RightTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Right_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Length shorter than string",
                    "Hello World",
                    5,
                    "World"
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
                    "Length greater than string",
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

                // First parameter has no type restriction.
                new object[]
                {
                    "Integer first parameter",
                    12345,
                    3,
                    "345"
                },
                new object[]
                {
                    "Boolean first parameter",
                    true,
                    2,
                    "ue"
                },

                // VerifyType(int) accepts compatible long values.
                new object[]
                {
                    "Long length",
                    "Hello",
                    3L,
                    "llo"
                },

                // Null first parameter is valid.
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
                    "Right",
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
        public void ExpressionFunction_String_Right_NullLength_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Right",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_InvalidLengthType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "3",
                true,
                3.0,
                new object()
            };

            foreach (object length in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Right",
                            "Hello",
                            length);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_NegativeLength_ShouldThrowInvalidArgumentException()
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
                            "Right",
                            "Hello",
                            length);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
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
                            "Right",
                            "Hello",
                            length);
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Right",
                "Hello",
                3);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_ParseModeWithNegativeLength_ShouldThrowInvalidArgumentException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Right",
                        "Hello",
                        -1);
                },
                typeof(InvalidArgumentException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_ParseModeWithInvalidLengthType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Right",
                        "Hello",
                        "3");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_ParseModeWithNullLength_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Right",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Right_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "Right",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}