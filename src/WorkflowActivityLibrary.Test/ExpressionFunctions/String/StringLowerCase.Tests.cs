using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class LowerCaseTests
    {
        [TestMethod]
        public void ExpressionFunction_String_LowerCase_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Uppercase string",
                    "HELLO WORLD",
                    "hello world"
                },
                new object[]
                {
                    "Mixed-case string",
                    "Hello World",
                    "hello world"
                },
                new object[]
                {
                    "Already lowercase",
                    "hello world",
                    "hello world"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
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
                    "true"
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
                    "LowerCase",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LowerCase_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "LowerCase",
                "HELLO WORLD");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_LowerCase_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    "One",
                    "Two"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "LowerCase",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}