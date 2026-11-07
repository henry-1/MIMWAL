using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class RegexReplaceTests
    {
        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Replace single match",
                    "Hello World",
                    "World",
                    "Test",
                    "Hello Test"
                },
                new object[]
                {
                    "Replace multiple matches",
                    "one two one",
                    "one",
                    "three",
                    "three two three"
                },
                new object[]
                {
                    "Remove matches",
                    "ABC123DEF456",
                    @"\d+",
                    string.Empty,
                    "ABCDEF"
                },
                new object[]
                {
                    "No match",
                    "Hello World",
                    "Test",
                    "Replacement",
                    "Hello World"
                },
                new object[]
                {
                    "Replace entire value",
                    "Hello",
                    "^.*$",
                    "World",
                    "World"
                },
                new object[]
                {
                    "Case sensitive",
                    "Hello hello",
                    "hello",
                    "Test",
                    "Hello Test"
                },
                new object[]
                {
                    "Empty input",
                    string.Empty,
                    ".+",
                    "Test",
                    string.Empty
                },
                new object[]
                {
                    "Integer input uses ToString",
                    12345,
                    @"\d+",
                    "Number",
                    "Number"
                },
                new object[]
                {
                    "Boolean input uses ToString",
                    true,
                    "True",
                    "Yes",
                    "Yes"
                },
                new object[]
                {
                    "Null input",
                    null,
                    ".*",
                    "Test",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object pattern = testCase[2];
                object replacement = testCase[3];
                string expected = (string)testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RegexReplace",
                    value,
                    pattern,
                    replacement);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_CaptureGroups_ShouldReturnExpectedResult()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "RegexReplace",
                "Smith, John",
                @"^([^,]+),\s*(.+)$",
                "$2 $1");

            // Assert
            Assert.AreEqual(
                "John Smith",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_NullPattern_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "RegexReplace",
                        "Hello",
                        null,
                        "Test");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_NullReplacement_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "RegexReplace",
                        "Hello",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_InvalidPattern_ShouldThrowInvalidArgumentException()
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
                            "RegexReplace",
                            "Hello",
                            pattern,
                            "Test");
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_NonStringPatternAndReplacement_ShouldUseToString()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "RegexReplace",
                "123-123",
                123,
                456);

            // Assert
            Assert.AreEqual(
                "456-456",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "RegexReplace",
                "Hello",
                "Hello",
                "World");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_ParseModeWithInvalidPattern_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "RegexReplace",
                "Hello",
                "[",
                "Test");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_ParseModeWithNullPattern_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "RegexReplace",
                        "Hello",
                        null,
                        "Test");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_ParseModeWithNullReplacement_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "RegexReplace",
                        "Hello",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_RegexReplace_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                    "Hello"
                },
                new object[]
                {
                    "Hello",
                    "Hello",
                    "World",
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
                            "RegexReplace",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}