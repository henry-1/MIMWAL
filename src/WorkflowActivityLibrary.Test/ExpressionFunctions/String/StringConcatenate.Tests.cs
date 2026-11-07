using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class ConcatenateTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Concatenate_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Two strings",
                    new object[] { "Hello", "World" },
                    "HelloWorld"
                },
                new object[]
                {
                    "Three strings",
                    new object[] { "One", "Two", "Three" },
                    "OneTwoThree"
                },
                new object[]
                {
                    "Empty strings",
                    new object[] { "", "" },
                    ""
                },
                new object[]
                {
                    "Empty string between values",
                    new object[] { "One", "", "Three" },
                    "OneThree"
                },
                new object[]
                {
                    "First parameter null",
                    new object[] { null, "World" },
                    "World"
                },
                new object[]
                {
                    "Second parameter null",
                    new object[] { "Hello", null },
                    "Hello"
                },
                new object[]
                {
                    "Both parameters null",
                    new object[] { null, null },
                    ""
                },
                new object[]
                {
                    "Null between values",
                    new object[] { "One", null, "Three" },
                    "OneThree"
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
                    "Concatenate",
                    parameters);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Concatenate_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 42, "Test" },
                new object[] { "Test", 42 },
                new object[] { true, "Test" },
                new object[] { "Test", false },
                new object[] { "One", "Two", 42 },
                new object[] { new object(), "Test" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Concatenate",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Concatenate_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[] { "Hello", "World" },
                new object[] { "One", "Two", "Three" },
                new object[] { null, "World" },
                new object[] { "Hello", null },
                new object[] { null, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Concatenate",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Concatenate_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Concatenate",
                        "Hello",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_Concatenate_TooFewParameters_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "One" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Concatenate",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}