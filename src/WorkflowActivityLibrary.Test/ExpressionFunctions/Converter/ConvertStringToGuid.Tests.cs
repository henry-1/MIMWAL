using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertStringToGuidTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertStringToGuid_ValidGuid_ShouldReturnExpectedBytes()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Normal Guid",
                    "550e8400-e29b-41d4-a716-446655440000"
                },
                new object[]
                {
                    "Uppercase Guid",
                    "550E8400-E29B-41D4-A716-446655440000"
                },
                new object[]
                {
                    "Guid with braces",
                    "{550e8400-e29b-41d4-a716-446655440000}"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string parameter = (string)testCase[1];
                byte[] expected = new Guid(parameter).ToByteArray();

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertStringToGuid",
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
        public void ExpressionFunction_Converter_ConvertStringToGuid_NullOrEmpty_ShouldReturnEmptyGuid()
        {
            object[] testCases =
            {
                null,
                string.Empty
            };

            byte[] expected = Guid.Empty.ToByteArray();

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertStringToGuid",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(byte[]));

                CollectionAssert.AreEqual(
                    expected,
                    (byte[])result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertStringToGuid_InvalidGuid_ShouldThrowFormatException()
        {
            string[] testCases =
            {
                "invalid",
                "not-a-guid",
                "550e8400-e29b-41d4-a716"
            };

            foreach (string parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertStringToGuid",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertStringToGuid_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object(),
                Guid.Empty
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertStringToGuid",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertStringToGuid_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                "550e8400-e29b-41d4-a716-446655440000",
                string.Empty,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertStringToGuid",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertStringToGuid_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertStringToGuid",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertStringToGuid_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    "550e8400-e29b-41d4-a716-446655440000",
                    "550e8400-e29b-41d4-a716-446655440001"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertStringToGuid",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}