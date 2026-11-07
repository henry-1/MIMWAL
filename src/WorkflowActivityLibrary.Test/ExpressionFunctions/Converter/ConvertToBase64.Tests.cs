using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertToBase64Tests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBase64_ValidValue_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single byte",
                    new byte[] { 1 },
                    "AQ=="
                },
                new object[]
                {
                    "Multiple bytes",
                    new byte[] { 1, 2, 3, 4, 5 },
                    "AQIDBAU="
                },
                new object[]
                {
                    "ASCII Test",
                    new byte[] { 84, 101, 115, 116 },
                    "VGVzdA=="
                },
                new object[]
                {
                    "Minimum and maximum byte",
                    new byte[] { 0, 255 },
                    "AP8="
                },
                new object[]
                {
                    "Empty byte array",
                    new byte[0],
                    string.Empty
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                byte[] parameter = (byte[])testCase[1];
                string expected = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertToBase64",
                    (object)parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBase64_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertToBase64",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBase64_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                new byte[] { 1, 2, 3 },
                new byte[0],
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertToBase64",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBase64_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "Test",
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
                            "ConvertToBase64",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBase64_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertToBase64",
                        "Test");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToBase64_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    new byte[] { 1 },
                    new byte[] { 2 }
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToBase64",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}