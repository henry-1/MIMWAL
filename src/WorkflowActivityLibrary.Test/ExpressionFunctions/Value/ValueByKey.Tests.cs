using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Value
{
    [TestClass]
    public class ValueByKeyTests
    {
        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_ExistingKey_ShouldReturnExpectedValue()
        {
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            dictionary.Add("Name", "Max");
            dictionary.Add("Age", 42);
            dictionary.Add("Enabled", true);

            object[][] testCases =
            {
                new object[]
                {
                    "String value",
                    "Name",
                    "Max"
                },
                new object[]
                {
                    "Integer value",
                    "Age",
                    42
                },
                new object[]
                {
                    "Boolean value",
                    "Enabled",
                    true
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string key =
                    (string)testCase[1];

                object expected =
                    testCase[2];

                // Act
                object result =
                    ExpressionFunctionTestHelper.Run(
                        "ValueByKey",
                        dictionary,
                        key);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_MissingKey_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            dictionary.Add(
                "Name",
                "Max");

            // Act
            object result =
                ExpressionFunctionTestHelper.Run(
                    "ValueByKey",
                    dictionary,
                    "DisplayName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_KeyComparison_ShouldBeCaseSensitive()
        {
            // Arrange
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            dictionary.Add(
                "Name",
                "Max");

            // Act
            object result =
                ExpressionFunctionTestHelper.Run(
                    "ValueByKey",
                    dictionary,
                    "name");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_NullStoredValue_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            dictionary.Add(
                "Name",
                null);

            // Act
            object result =
                ExpressionFunctionTestHelper.Run(
                    "ValueByKey",
                    dictionary,
                    "Name");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_NullDictionary_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.Run(
                    "ValueByKey",
                    null,
                    "Name");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_EmptyDictionary_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            // Act
            object result =
                ExpressionFunctionTestHelper.Run(
                    "ValueByKey",
                    dictionary,
                    "Name");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_InvalidFirstParameterType_ShouldThrowInvalidArgumentException()
        {
            object[] testCases =
            {
                "Test",
                42,
                true,
                new object(),
                new List<string>()
            };

            foreach (object dictionary in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ValueByKey",
                            dictionary,
                            "Name");
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_NullKey_ShouldThrowInvalidFunctionFormatException()
        {
            // Arrange
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ValueByKey",
                        dictionary,
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_InvalidKeyType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object()
            };

            Dictionary<string, object> dictionary =
                new Dictionary<string, object>();

            foreach (object key in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ValueByKey",
                            dictionary,
                            key);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_ParseMode_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.RunParse(
                    "ValueByKey",
                    new Dictionary<string, object>(),
                    "Name");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_ParseModeWithInvalidKeyType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ValueByKey",
                        new Dictionary<string, object>(),
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByKey_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new Dictionary<string, object>()
                },

                new object[]
                {
                    new Dictionary<string, object>(),
                    "Name",
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
                            "ValueByKey",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}