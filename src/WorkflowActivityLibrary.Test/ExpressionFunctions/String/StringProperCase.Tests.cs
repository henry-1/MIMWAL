using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class ProperCaseTests
    {
        [TestMethod]
        public void ExpressionFunction_String_ProperCase_ValidParameter_ShouldReturnExpectedResult()
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
                    "Uppercase words",
                    "HELLO WORLD",
                    "Hello World"
                },
                new object[]
                {
                    "Mixed case words",
                    "hELLo wORLd",
                    "Hello World"
                },
                new object[]
                {
                    "Already proper case",
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
                    "Multiple spaces",
                    "hello   world",
                    "Hello   World"
                },
                new object[]
                {
                    "Leading and trailing spaces",
                    "  hello world  ",
                    "  Hello World  "
                },
                new object[]
                {
                    "Tab separates words",
                    "hello\tworld",
                    "Hello\tWorld"
                },
                new object[]
                {
                    "New line separates words",
                    "hello\nworld",
                    "Hello\nWorld"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    string.Empty
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
                    "ProperCase",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ProperCase_NonLetterCharacters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Number inside word",
                    "hello123WORLD",
                    "Hello123world"
                },
                new object[]
                {
                    "Hyphen does not start new word",
                    "hello-WORLD",
                    "Hello-world"
                },
                new object[]
                {
                    "Period does not start new word",
                    "hello.WORLD",
                    "Hello.world"
                },
                new object[]
                {
                    "Numbers only",
                    "12345",
                    "12345"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string parameter = (string)testCase[1];
                string expected = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ProperCase",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ProperCase_NonStringParameter_ShouldReturnNull()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ProperCase",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ProperCase_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ProperCase",
                "hello world");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ProperCase_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "ProperCase",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}