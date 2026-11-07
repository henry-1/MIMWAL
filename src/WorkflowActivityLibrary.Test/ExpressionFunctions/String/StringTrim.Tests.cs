using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class TrimTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Trim_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Leading and trailing spaces",
                    "   Hello World   ",
                    "Hello World"
                },
                new object[]
                {
                    "Tabs",
                    "\tHello World\t",
                    "Hello World"
                },
                new object[]
                {
                    "New lines",
                    "\r\nHello World\r\n",
                    "Hello World"
                },
                new object[]
                {
                    "No whitespace",
                    "Hello World",
                    "Hello World"
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
                    "Trim",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Trim_TrimCharacters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Null trim characters",
                    new object[] { "   Hello   ", null },
                    "Hello"
                },
                new object[]
                {
                    "Empty trim characters",
                    new object[] { "   Hello   ", string.Empty },
                    "Hello"
                },
                new object[]
                {
                    "Single trim character",
                    new object[] { "xxxHelloxxx", "x" },
                    "Hello"
                },
                new object[]
                {
                    "Multiple trim characters",
                    new object[] { "xyxyHelloyxyx", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "Trim characters only",
                    new object[] { "xxxxx", "x" },
                    string.Empty
                },
                new object[]
                {
                    "No matching trim characters",
                    new object[] { "Hello", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "Internal characters remain",
                    new object[] { "xxxHelloxWorldxxx", "x" },
                    "HelloxWorld"
                },
                new object[]
                {
                    "Non-string trim parameter",
                    new object[] { "11123111", 1 },
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
                    "Trim",
                    parameters);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Trim_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Trim",
                "   Hello   ");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Trim_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "Trim",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}