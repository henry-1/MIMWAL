using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Value
{
    [TestClass]
    public class ValueByIndexTests
    {
        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_List_ShouldReturnExpectedValue()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "First item",
                    new List<string> { "One", "Two", "Three" },
                    0,
                    "One"
                },
                new object[]
                {
                    "Middle item",
                    new List<string> { "One", "Two", "Three" },
                    1,
                    "Two"
                },
                new object[]
                {
                    "Last item",
                    new List<string> { "One", "Two", "Three" },
                    2,
                    "Three"
                },
                new object[]
                {
                    "Index equal to count",
                    new List<string> { "One", "Two", "Three" },
                    3,
                    null
                },
                new object[]
                {
                    "Index greater than count",
                    new List<string> { "One", "Two", "Three" },
                    10,
                    null
                },
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    0,
                    null
                },
                new object[]
                {
                    "Null item",
                    new List<string> { "One", null, "Three" },
                    1,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object values = testCase[1];
                object index = testCase[2];
                object expected = testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ValueByIndex",
                    values,
                    index);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_GenericList_ShouldPreserveValueType()
        {
            // Arrange
            List<int> values = new List<int>
            {
                10,
                20,
                30
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ValueByIndex",
                values,
                1);

            // Assert
            Assert.AreEqual(
                20,
                result);

            Assert.AreEqual(
                typeof(int),
                result.GetType());
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_Scalar_ShouldReturnOnlyForIndexZero()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "String at index zero",
                    "Hello",
                    0,
                    "Hello"
                },
                new object[]
                {
                    "String beyond index zero",
                    "Hello",
                    1,
                    null
                },
                new object[]
                {
                    "Integer at index zero",
                    42,
                    0,
                    42
                },
                new object[]
                {
                    "Integer beyond index zero",
                    42,
                    1,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object index = testCase[2];
                object expected = testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ValueByIndex",
                    value,
                    index);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_NullValue_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ValueByIndex",
                null,
                0);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_LongIndex_ShouldReturnExpectedValue()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ValueByIndex",
                new List<string> { "One", "Two", "Three" },
                1L);

            // Assert
            Assert.AreEqual(
                "Two",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_NegativeIndex_ShouldThrowInvalidArgumentException()
        {
            object[] testCases =
            {
                -1,
                -10,
                -1L
            };

            foreach (object index in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ValueByIndex",
                            new List<string> { "One", "Two" },
                            index);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_NullIndex_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ValueByIndex",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_InvalidIndexType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "1",
                true,
                1.0,
                new object()
            };

            foreach (object index in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ValueByIndex",
                            new List<string>(),
                            index);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ValueByIndex",
                new List<string>(),
                0);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_ParseModeWithNegativeIndex_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ValueByIndex",
                new List<string>(),
                -1);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_ParseModeWithInvalidIndexType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ValueByIndex",
                        new List<string>(),
                        "1");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueByIndex_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "One" },
                new object[] { "One", 0, "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ValueByIndex",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}