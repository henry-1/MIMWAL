using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class UpperCaseTests
    {
        [TestMethod]
        public void ExpressionFunction_String_UpperCase_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Lowercase string",
                    "hello world",
                    "HELLO WORLD"
                },
                new object[]
                {
                    "Mixed-case string",
                    "Hello World",
                    "HELLO WORLD"
                },
                new object[]
                {
                    "Already uppercase",
                    "HELLO WORLD",
                    "HELLO WORLD"
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
                    "TRUE"
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
                    "UpperCase",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_UpperCase_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "UpperCase",
                "hello world");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_UpperCase_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "Hello",
                    "World"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "UpperCase",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}