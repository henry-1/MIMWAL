using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class RTrimTests
    {
        [TestMethod]
        public void ExpressionFunction_String_RTrim_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Trailing spaces",
                    "   Hello World   ",
                    "   Hello World"
                },
                new object[]
                {
                    "Trailing tab",
                    "Hello World\t",
                    "Hello World"
                },
                new object[]
                {
                    "Trailing new line",
                    "Hello World\r\n",
                    "Hello World"
                },
                new object[]
                {
                    "No trailing whitespace",
                    "   Hello World",
                    "   Hello World"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    string.Empty
                },
                new object[]
                {
                    "Whitespace only",
                    "   ",
                    string.Empty
                },
                new object[]
                {
                    "Integer",
                    42,
                    "42"
                },
                new object[]
                {
                    "Boolean",
                    true,
                    "True"
                },
                new object[]
                {
                    "Null",
                    null,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                string expected = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RTrim",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_RTrim_TrimCharacters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Null trim characters",
                    new object[] { "   Hello   ", null },
                    "   Hello"
                },
                new object[]
                {
                    "Empty trim characters",
                    new object[] { "   Hello   ", string.Empty },
                    "   Hello"
                },
                new object[]
                {
                    "Single trim character",
                    new object[] { "Helloxxx", "x" },
                    "Hello"
                },
                new object[]
                {
                    "Multiple trim characters",
                    new object[] { "Helloxyxy", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "Characters are treated as a set",
                    new object[] { "Helloyxyx", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "Only trailing characters removed",
                    new object[] { "xxxHelloxxx", "x" },
                    "xxxHello"
                },
                new object[]
                {
                    "No matching trim character",
                    new object[] { "Hello", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "All characters trimmed",
                    new object[] { "xxxxx", "x" },
                    string.Empty
                },
                new object[]
                {
                    "Non-string trim parameter",
                    new object[] { "23111", 1 },
                    "23"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object[] parameters = (object[])testCase[1];
                string expected = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RTrim",
                    parameters);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_RTrim_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "RTrim",
                "Hello   ");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RTrim_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "Hello",
                    "x",
                    "Extra"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RTrim",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}