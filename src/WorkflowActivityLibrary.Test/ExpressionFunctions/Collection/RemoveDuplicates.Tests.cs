using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Collection
{
    [TestClass]
    public class RemoveDuplicatesTests
    {
        [TestMethod]
        public void ExpressionFunction_Collection_RemoveDuplicates_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "No duplicates",
                    new List<string>
                    {
                        "One",
                        "Two",
                        "Three"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Duplicate values",
                    new List<string>
                    {
                        "One",
                        "Two",
                        "One",
                        "Three",
                        "Two"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Case insensitive duplicates",
                    new List<string>
                    {
                        "One",
                        "ONE",
                        "one",
                        "Two",
                        "TWO"
                    },
                    new string[]
                    {
                        "One",
                        "Two"
                    }
                },
                new object[]
                {
                    "First occurrence preserved",
                    new List<string>
                    {
                        "ONE",
                        "one",
                        "Two"
                    },
                    new string[]
                    {
                        "ONE",
                        "Two"
                    }
                },
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    new string[0]
                },
                new object[]
                {
                    "Duplicate null values",
                    new List<string>
                    {
                        null,
                        "One",
                        null,
                        "Two"
                    },
                    new string[]
                    {
                        null,
                        "One",
                        "Two"
                    }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                List<string> parameter =
                    (List<string>)testCase[1];

                string[] expected =
                    (string[])testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "RemoveDuplicates",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(List<string>),
                    "Failed test case: " + description);

                CollectionAssert.AreEqual(
                    expected,
                    ((List<string>)result).ToArray(),
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_RemoveDuplicates_NullParameter_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "RemoveDuplicates",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_RemoveDuplicates_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "One",
                42,
                42L,
                true,
                new object(),
                new List<int> { 1, 2, 3 }
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RemoveDuplicates",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_RemoveDuplicates_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "RemoveDuplicates",
                new List<string>());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_RemoveDuplicates_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "RemoveDuplicates",
                        "One");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_RemoveDuplicates_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new List<string>(),
                    new List<string>()
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "RemoveDuplicates",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}