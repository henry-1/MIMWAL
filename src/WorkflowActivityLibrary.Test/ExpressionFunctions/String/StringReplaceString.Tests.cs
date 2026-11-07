using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class ReplaceStringTests
    {
        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single occurrence",
                    "Hello World",
                    "World",
                    "Test",
                    "Hello Test"
                },
                new object[]
                {
                    "Multiple occurrences",
                    "one two one",
                    "one",
                    "three",
                    "three two three"
                },
                new object[]
                {
                    "Remove occurrences",
                    "Hello World",
                    " ",
                    string.Empty,
                    "HelloWorld"
                },
                new object[]
                {
                    "Search value not found",
                    "Hello World",
                    "Test",
                    "Replacement",
                    "Hello World"
                },
                new object[]
                {
                    "Replace complete value",
                    "Hello",
                    "Hello",
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
                    "Empty source",
                    string.Empty,
                    "Hello",
                    "World",
                    string.Empty
                },
                new object[]
                {
                    "Null source",
                    null,
                    "Hello",
                    "World",
                    null
                },
                new object[]
                {
                    "Integer source",
                    12345,
                    "23",
                    "XY",
                    "1XY45"
                },
                new object[]
                {
                    "Boolean source",
                    true,
                    "True",
                    "Yes",
                    "Yes"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object source = testCase[1];
                object search = testCase[2];
                object replacement = testCase[3];
                string expected = (string)testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ReplaceString",
                    source,
                    search,
                    replacement);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_NonStringSearchAndReplacement_ShouldUseToString()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ReplaceString",
                "123-123",
                123,
                456);

            // Assert
            Assert.AreEqual(
                "456-456",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_EmptySearch_ShouldThrowInvalidArgumentException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ReplaceString",
                        "Hello",
                        string.Empty,
                        "World");
                },
                typeof(InvalidArgumentException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_NullSearch_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ReplaceString",
                        "Hello",
                        null,
                        "World");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_NullReplacement_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ReplaceString",
                        "Hello",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_NullSourceWithEmptySearch_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ReplaceString",
                null,
                string.Empty,
                "World");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ReplaceString",
                "Hello",
                "Hello",
                "World");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_ParseModeWithEmptySearch_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ReplaceString",
                "Hello",
                string.Empty,
                "World");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_ParseModeWithNullSearch_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ReplaceString",
                        "Hello",
                        null,
                        "World");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_ParseModeWithNullReplacement_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ReplaceString",
                        "Hello",
                        "Hello",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ReplaceString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "Hello" },
                new object[] { "Hello", "Hello" },
                new object[] { "Hello", "Hello", "World", "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ReplaceString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}