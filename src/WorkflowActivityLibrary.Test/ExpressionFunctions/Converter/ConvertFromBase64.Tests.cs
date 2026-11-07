using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertFromBase64Tests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_ValidValue_ShouldReturnExpectedBytes()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single byte",
                    "AQ==",
                    new byte[] { 1 }
                },
                new object[]
                {
                    "Multiple bytes",
                    "AQIDBAU=",
                    new byte[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    "ASCII text",
                    "VGVzdA==",
                    new byte[] { 84, 101, 115, 116 }
                },
                new object[]
                {
                    "All byte boundaries",
                    "AP8=",
                    new byte[] { 0, 255 }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string parameter = (string)testCase[1];
                byte[] expected = (byte[])testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertFromBase64",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(byte[]),
                    "Failed test case: " + description);

                CollectionAssert.AreEqual(
                    expected,
                    (byte[])result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_NullOrEmptyValue_ShouldReturnNull()
        {
            object[] testCases =
            {
                null,
                string.Empty
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertFromBase64",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                "VGVzdA==",
                string.Empty,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertFromBase64",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "AQ==", "Ag==" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertFromBase64",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object(),
                new byte[] { 1, 2, 3 }
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertFromBase64",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_InvalidBase64_ShouldThrowFormatException()
        {
            string[] testCases =
            {
                "Not Base64!",
                "ABC",
                "===="
            };

            foreach (string parameter in testCases)
            {
                Exception exception = null;

                // Act
                try
                {
                    ExpressionFunctionTestHelper.Run(
                        "ConvertFromBase64",
                        parameter);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                // Assert
                Assert.IsNotNull(
                    exception,
                    "Expected invalid Base64 value to throw.");

                Assert.IsNotNull(
                    exception.InnerException,
                    "Expected an inner exception.");

                Assert.AreEqual(
                    typeof(InvalidFunctionFormatException),
                    exception.InnerException.GetType(),
                    "Expected exact FormatException for: " + parameter);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertFromBase64_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertFromBase64",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}