using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class TitleCaseTests
    {
        [TestMethod]
        public void ExpressionFunction_String_TitleCase_Parameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Lowercase words",
                    "hello world",
                    "Hello World"
                },
                new object[]
                {
                    "Mixed-case words",
                    "hello WORLD",
                    "Hello WORLD"
                },
                new object[]
                {
                    "Already title case",
                    "Hello World",
                    "Hello World"
                },
                new object[]
                {
                    "Single word",
                    "hello",
                    "Hello"
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
                    "TitleCase",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_TitleCase_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                "hello world",
                string.Empty,
                42,
                true,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "TitleCase",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_TitleCase_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    "hello",
                    "world"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "TitleCase",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}