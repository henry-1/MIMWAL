using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Value
{
    [TestClass]
    public class ValueTypeTests
    {
        [TestMethod]
        public void ExpressionFunction_Value_ValueType_ValidParameter_ShouldReturnExpectedType()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "String",
                    "Hello",
                    "System.String"
                },
                new object[]
                {
                    "Integer",
                    42,
                    "System.Int32"
                },
                new object[]
                {
                    "Long",
                    42L,
                    "System.Int64"
                },
                new object[]
                {
                    "Boolean",
                    true,
                    "System.Boolean"
                },
                new object[]
                {
                    "DateTime",
                    new DateTime(2026, 10, 28),
                    "System.DateTime"
                },
                new object[]
                {
                    "Guid",
                    new Guid(
                        "c302a07d-5400-43af-94dd-83aa65e5aed4"),
                    "System.Guid"
                },
                new object[]
                {
                    "List of strings",
                    new List<string>(),
                    "System.Collections.Generic.List`1[System.String]"
                },
                new object[]
                {
                    "Dictionary",
                    new Dictionary<string, object>(),
                    "System.Collections.Generic.Dictionary`2[System.String,System.Object]"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                object parameter =
                    testCase[1];

                string expected =
                    (string)testCase[2];

                // Act
                object result =
                    ExpressionFunctionTestHelper.Run(
                        "ValueType",
                        parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueType_NullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ValueType",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueType_ParseMode_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.RunParse(
                    "ValueType",
                    "Hello");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueType_ParseModeWithNullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ValueType",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Value_ValueType_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "One",
                    "Two"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ValueType",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}