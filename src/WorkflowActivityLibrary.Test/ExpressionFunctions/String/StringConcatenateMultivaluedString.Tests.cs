using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class ConcatenateMultivaluedStringTests
    {
        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single string",
                    "One",
                    ";",
                    "One"
                },
                new object[]
                {
                    "Multiple values",
                    new List<string> { "One", "Two", "Three" },
                    ";",
                    "One;Two;Three"
                },
                new object[]
                {
                    "Comma delimiter",
                    new List<string> { "One", "Two", "Three" },
                    ",",
                    "One,Two,Three"
                },
                new object[]
                {
                    "Multi-character delimiter",
                    new List<string> { "One", "Two", "Three" },
                    " - ",
                    "One - Two - Three"
                },
                new object[]
                {
                    "Empty delimiter",
                    new List<string> { "One", "Two", "Three" },
                    string.Empty,
                    "OneTwoThree"
                },
                new object[]
                {
                    "Single item list",
                    new List<string> { "One" },
                    ";",
                    "One"
                },
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    ";",
                    null
                },
                new object[]
                {
                    "Null values",
                    null,
                    ";",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object values = testCase[1];
                object delimiter = testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConcatenateMultivaluedString",
                    values,
                    delimiter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_ListContainingNull_ShouldReturnExpectedResult()
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
                "ConcatenateMultivaluedString",
                values,
                ";");

            // Assert
            Assert.AreEqual(
                "One;;Three",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_NullDelimiter_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "One",
                new List<string> { "One", "Two" },
                null
            };

            foreach (object values in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConcatenateMultivaluedString",
                            values,
                            null);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object(),
                new List<int> { 1, 2, 3 }
            };

            foreach (object values in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConcatenateMultivaluedString",
                            values,
                            ";");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_NonStringDelimiter_ShouldUseToString()
        {
            object[][] testCases =
            {
                new object[]
                {
                    1,
                    "One1Two1Three"
                },
                new object[]
                {
                    true,
                    "OneTrueTwoTrueThree"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                object delimiter = testCase[0];
                string expected = (string)testCase[1];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConcatenateMultivaluedString",
                    new List<string> { "One", "Two", "Three" },
                    delimiter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ConcatenateMultivaluedString",
                new List<string>(),
                ";");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_ParseModeWithNullDelimiter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConcatenateMultivaluedString",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConcatenateMultivaluedString",
                        42,
                        ";");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_ConcatenateMultivaluedString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new List<string>()
                },

                new object[]
                {
                    new List<string>(),
                    ";",
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
                            "ConcatenateMultivaluedString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}