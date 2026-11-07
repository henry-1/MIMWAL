using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertToStringTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToString_ValidParameter_ShouldReturnExpectedResult()
        {
            Guid guid = new Guid(
                "550e8400-e29b-41d4-a716-446655440000");

            object[][] testCases =
            {
                // String
                new object[] { "String",           "Test",  "Test" },
                new object[] { "Empty string",     "",      "" },

                // Boolean
                new object[] { "Boolean true",     true,    "True" },
                new object[] { "Boolean false",    false,   "False" },

                // Integer
                new object[] { "Positive integer", 42,      "42" },
                new object[] { "Negative integer", -42,     "-42" },
                new object[] { "Zero integer",     0,       "0" },

                // Long
                new object[] { "Positive long",    42L,     "42" },
                new object[] { "Negative long",    -42L,    "-42" },
                new object[] { "Zero long",        0L,      "0" },
                new object[] { "Long minimum",     long.MinValue, "-9223372036854775808" },
                new object[] { "Long maximum",     long.MaxValue, "9223372036854775807" },

                // Guid
                new object[]
                {
                    "Guid",
                    guid,
                    "550e8400-e29b-41d4-a716-446655440000"
                },

                // Byte array
                new object[]
                {
                    "Byte array",
                    new byte[] { 1, 2, 3 },
                    "System.Byte[]"
                },

                // Null
                new object[] { "Null",             null,    null }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                string expected = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertToString",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToString_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                1.0,
                1.0f,
                1.0m,
                DateTime.Now,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToString",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToString_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                "Test",
                true,
                42,
                42L,
                Guid.Empty,
                new byte[] { 1, 2, 3 },
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertToString",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToString_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertToString",
                        1.0);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "One", "Two" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}