using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class LengthTests
    {
        [TestMethod]
        public void ExpressionFunction_String_Length_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Normal string",
                    "Hello",
                    5
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    0
                },
                new object[]
                {
                    "String with spaces",
                    "Hello World",
                    11
                },
                new object[]
                {
                    "Whitespace only",
                    "   ",
                    3
                },
                new object[]
                {
                    "Integer",
                    12345,
                    5
                },
                new object[]
                {
                    "Boolean",
                    true,
                    4
                },
                new object[]
                {
                    "Null",
                    null,
                    0
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                int expected = (int)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Length",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_Length_ParseMode_ShouldReturnZero()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Length",
                "Hello");

            // Assert
            Assert.AreEqual(
                0,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_Length_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "Length",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}