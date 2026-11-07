using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Collection
{
    [TestClass]
    public class IndexByValueTests
    {
        [TestMethod]
        public void ExpressionFunction_Collection_IndexByValue_List_ShouldReturnExpectedIndex()
        {
            object[][] testCases =
            {
                // Strings
                new object[]
                {
                    "First string",
                    new List<string> { "One", "Two", "Three" },
                    "One",
                    0
                },
                new object[]
                {
                    "Middle string",
                    new List<string> { "One", "Two", "Three" },
                    "Two",
                    1
                },
                new object[]
                {
                    "Last string",
                    new List<string> { "One", "Two", "Three" },
                    "Three",
                    2
                },
                new object[]
                {
                    "String comparison ignores case",
                    new List<string> { "One", "Two", "Three" },
                    "TWO",
                    1
                },
                new object[]
                {
                    "String not found",
                    new List<string> { "One", "Two", "Three" },
                    "Four",
                    -1
                },

                // Integers
                new object[]
                {
                    "First integer",
                    new List<int> { 10, 20, 30 },
                    10,
                    0
                },
                new object[]
                {
                    "Middle integer",
                    new List<int> { 10, 20, 30 },
                    20,
                    1
                },
                new object[]
                {
                    "Last integer",
                    new List<int> { 10, 20, 30 },
                    30,
                    2
                },
                new object[]
                {
                    "Integer not found",
                    new List<int> { 10, 20, 30 },
                    40,
                    -1
                },

                // Duplicates
                new object[]
                {
                    "Duplicate returns first index",
                    new List<string> { "One", "Two", "Two", "Three" },
                    "Two",
                    1
                },
                new object[]
                {
                    "List contains null before matching value",
                    new List<string> { "One", null, "Two", "Three" },
                    "Two",
                    2
                },

                // Empty
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    "One",
                    -1
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object values = testCase[1];
                object value = testCase[2];
                int expected = (int)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "IndexByValue",
                    values,
                    value);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_IndexByValue_NullParameter_ShouldReturnMinusOne()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "List is null",
                    null,
                    "One"
                },
                new object[]
                {
                    "Value is null",
                    new List<string> { "One", "Two" },
                    null
                },
                new object[]
                {
                    "Both parameters are null",
                    null,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object values = testCase[1];
                object value = testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "IndexByValue",
                    values,
                    value);

                // Assert
                Assert.AreEqual(
                    -1,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_IndexByValue_FirstParameterIsNotOfTypeList_ShouldReturnMinusOne()
        {
            object[][] testCases =
            {
                new object[] { "String",  "One",  "One" },
                new object[] { "Integer", 42,     42 },
                new object[] { "Boolean", true,   true },
                new object[] { "Object",  new object(), new object() }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object values = testCase[1];
                object value = testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "IndexByValue",
                    values,
                    value);

                // Assert
                Assert.AreEqual(
                    -1,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_IndexByValue_ParseMode_ShouldReturnMinusOne()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "IndexByValue",
                new List<string> { "One", "Two", "Three" },
                "Two");

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_IndexByValue_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    new List<string> { "One", "Two" }
                },
                new object[]
                {
                    new List<string> { "One", "Two" },
                    "Two",
                    "Three"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "IndexByValue",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}