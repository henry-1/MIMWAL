using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class RegexMatchTests
    {
        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Exact match",
                    "Hello",
                    "^Hello$",
                    true
                },
                new object[]
                {
                    "Exact match fails",
                    "Hello World",
                    "^Hello$",
                    false
                },
                new object[]
                {
                    "Partial match",
                    "Hello World",
                    "World",
                    true
                },
                new object[]
                {
                    "No match",
                    "Hello World",
                    "Test",
                    false
                },
                new object[]
                {
                    "Digits",
                    "ABC123",
                    @"\d+",
                    true
                },
                new object[]
                {
                    "Letters only",
                    "ABC",
                    @"^[A-Za-z]+$",
                    true
                },
                new object[]
                {
                    "Case sensitive",
                    "Hello",
                    "hello",
                    false
                },
                new object[]
                {
                    "Empty string matches empty expression",
                    string.Empty,
                    "^$",
                    true
                },
                new object[]
                {
                    "Empty string does not match required character",
                    string.Empty,
                    ".+",
                    false
                },
                new object[]
                {
                    "Integer converted to string",
                    12345,
                    @"^\d+$",
                    true
                },
                new object[]
                {
                    "Boolean converted to string",
                    true,
                    "^True$",
                    true
                },
                new object[]
                {
                    "Null input",
                    null,
                    ".*",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object pattern = testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RegexMatch",
                    value,
                    pattern);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_NullPattern_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "RegexMatch",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_InvalidPattern_ShouldThrowInvalidFunctionFormatException()
        {
            string[] testCases =
            {
                "[",
                "(",
                "*"
            };

            foreach (string pattern in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RegexMatch",
                            "Hello",
                            pattern);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_NonStringPattern_ShouldUseToString()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "RegexMatch",
                "12345",
                123);

            // Assert
            Assert.AreEqual(
                true,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_ParseMode_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "RegexMatch",
                "Hello",
                "^Hello$");

            // Assert
            Assert.AreEqual(
                false,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_ParseModeWithInvalidPattern_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "RegexMatch",
                "Hello",
                "[");

            // Assert
            Assert.AreEqual(
                false,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_ParseModeWithNullPattern_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "RegexMatch",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexMatch_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    "Hello"
                },
                new object[]
                {
                    "Hello",
                    "^Hello$",
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
                            "RegexMatch",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}