using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class FormatMultivaluedListTests
    {
        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_SingleValues_ShouldReturnFormattedString()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single string",
                    new object[] { "Value: {0}", "Test" },
                    "Value: Test"
                },
                new object[]
                {
                    "Multiple strings",
                    new object[] { "{0} {1}", "Hello", "World" },
                    "Hello World"
                },
                new object[]
                {
                    "Number",
                    new object[] { "Value: {0}", 42 },
                    "Value: 42"
                },
                new object[]
                {
                    "Boolean",
                    new object[] { "Value: {0}", true },
                    "Value: True"
                },
                new object[]
                {
                    "Null value",
                    new object[] { "Value: {0}", null },
                    "Value: "
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
                    "FormatMultivaluedList",
                    parameters);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_List_ShouldReturnFormattedList()
        {
            // Arrange
            List<string> values = new List<string>
            {
                "One",
                "Two",
                "Three"
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "FormatMultivaluedList",
                "Value: {0}",
                values);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            CollectionAssert.AreEqual(
                new string[]
                {
                    "Value: One",
                    "Value: Two",
                    "Value: Three"
                },
                ((List<string>)result).ToArray());
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_ListAndScalar_ShouldReuseScalarForEachItem()
        {
            // Arrange
            List<string> values = new List<string>
            {
                "One",
                "Two",
                "Three"
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "FormatMultivaluedList",
                "{0}: {1}",
                values,
                "Test");

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            CollectionAssert.AreEqual(
                new string[]
                {
                    "One: Test",
                    "Two: Test",
                    "Three: Test"
                },
                ((List<string>)result).ToArray());
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_ListsWithDifferentLengths_ShouldUseLongestList()
        {
            // Arrange
            List<string> first = new List<string>
            {
                "A",
                "B",
                "C"
            };

            List<int> second = new List<int>
            {
                1,
                2
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "FormatMultivaluedList",
                "{0}-{1}",
                first,
                second);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            CollectionAssert.AreEqual(
                new string[]
                {
                    "A-1",
                    "B-2",
                    "C-"
                },
                ((List<string>)result).ToArray());
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_ListContainingNull_ShouldReturnExpectedList()
        {
            // Arrange
            List<string> values = new List<string>
            {
                "One",
                null,
                "Three"
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "FormatMultivaluedList",
                "Value: {0}",
                values);

            // Assert
            CollectionAssert.AreEqual(
                new string[]
                {
                    "Value: One",
                    "Value: ",
                    "Value: Three"
                },
                ((List<string>)result).ToArray());
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_EmptyList_ShouldUseSingleValueFormatting()
        {
            // Arrange
            List<string> values = new List<string>();

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "FormatMultivaluedList",
                "{0}-{1}",
                values,
                "Test");

            // Assert
            Assert.AreEqual(
                "-Test",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_InvalidFormat_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Missing argument",
                    new object[] { "{0}-{1}", "One" }
                },
                new object[]
                {
                    "Invalid opening brace",
                    new object[] { "{0", "One" }
                },
                new object[]
                {
                    "Invalid closing brace",
                    new object[] { "0}", "One" }
                },
                new object[]
                {
                    "Invalid format with list",
                    new object[]
                    {
                        "{1}",
                        new List<string> { "One", "Two" }
                    }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                object[] parameters = (object[])testCase[1];

                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "FormatMultivaluedList",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_NullFormat_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "FormatMultivaluedList",
                        null,
                        "Test");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "FormatMultivaluedList",
                "{0}",
                "Test");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_ParseModeWithNullFormat_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "FormatMultivaluedList",
                        null,
                        "Test");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "{0}" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "FormatMultivaluedList",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_FormatMultivaluedList_ListWithNullParameter_ShouldReturnExpectedList()
        {
            // Arrange
            List<string> values = new List<string>
            {
                "One",
                "Two",
                "Three"
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "FormatMultivaluedList",
                "{0}-{1}",
                values,
                null);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            CollectionAssert.AreEqual(
                new string[]
                {
                    "One-",
                    "Two-",
                    "Three-"
                },
                ((List<string>)result).ToArray());
        }
    }
}