using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Unsorted
{
    [TestClass]
    public class EscapeDNComponentTests
    {
        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Simple component",
                    "CN=John Doe",
                    "CN=John Doe"
                },
                new object[]
                {
                    "Whitespace around component",
                    "  CN  =  John Doe  ",
                    "CN=John Doe"
                },
                new object[]
                {
                    "Equals in value",
                    "CN=John=Doe",
                    "CN=John\\=Doe"
                },
                new object[]
                {
                    "Comma",
                    "CN=Doe, John",
                    "CN=Doe\\, John"
                },
                new object[]
                {
                    "Plus",
                    "CN=John+Doe",
                    "CN=John\\+Doe"
                },
                new object[]
                {
                    "Semicolon",
                    "CN=John;Doe",
                    "CN=John\\;Doe"
                },
                new object[]
                {
                    "Double quote",
                    "CN=John\"Doe",
                    "CN=John\\\"Doe"
                },
                new object[]
                {
                    "Backslash",
                    "CN=John\\Doe",
                    "CN=John\\\\Doe"
                },
                new object[]
                {
                    "Hash",
                    "CN=John#Doe",
                    "CN=John\\#Doe"
                },
                new object[]
                {
                    "Less than",
                    "CN=John<Doe",
                    "CN=John\\<Doe"
                },
                new object[]
                {
                    "Greater than",
                    "CN=John>Doe",
                    "CN=John\\>Doe"
                },
                new object[]
                {
                    "Multiple special characters",
                    "CN=Doe, John+Test",
                    "CN=Doe\\, John\\+Test"
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
                    "EscapeDNComponent",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_InvalidFormat_ShouldThrowInvalidFunctionFormatException()
        {
            string[] testCases =
            {
                string.Empty,
                "CN",
                "John Doe",
                "=John",
                "CN="
            };

            foreach (string parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "EscapeDNComponent",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_NullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "EscapeDNComponent",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
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
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "EscapeDNComponent",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "EscapeDNComponent",
                "CN=John Doe");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "EscapeDNComponent",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_ParseModeWithNullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "EscapeDNComponent",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_EscapeDNComponent_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "CN=John",
                    "CN=Doe"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "EscapeDNComponent",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}