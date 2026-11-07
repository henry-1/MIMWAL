using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Collection
{
    [TestClass]
    public class InsertValuesTests
    {
        [TestMethod]
        public void ExpressionFunction_Collection_InsertValues_Value_ShouldReturnExpectedCollection()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "String value",
                    "Test",
                    new object[] { "Test" }
                },
                new object[]
                {
                    "Integer value",
                    42,
                    new object[] { 42 }
                },
                new object[]
                {
                    "Boolean value",
                    true,
                    new object[] { true }
                },
                new object[]
                {
                    "String list",
                    new List<string> { "One", "Two", "Three" },
                    new object[] { "One", "Two", "Three" }
                },
                new object[]
                {
                    "Integer list",
                    new List<int> { 1, 2, 3 },
                    new object[] { 1, 2, 3 }
                },
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    new object[0]
                },
                new object[]
                {
                    "List containing null",
                    new List<string> { "One", null, "Three" },
                    new object[] { "One", null, "Three" }
                },
                new object[]
                {
                    "Null",
                    null,
                    new object[0]
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                object[] expected = (object[])testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "InsertValues",
                    parameter);

                // Assert
                Assert.IsNotNull(
                    result,
                    "Failed test case: " + description);

                Assert.IsInstanceOfType(
                    result,
                    typeof(InsertedValuesCollection),
                    "Failed test case: " + description);

                InsertedValuesCollection collection =
                    (InsertedValuesCollection)result;

                Assert.AreEqual(
                    expected.Length,
                    collection.Count,
                    "Failed test case: " + description);

                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.AreEqual(
                        expected[i],
                        collection[i],
                        "Failed test case: " +
                        description +
                        ". Item index: " +
                        i);
                }
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_InsertValues_Array_ShouldInsertArrayAsSingleValue()
        {
            // Arrange
            string[] value = new string[]
            {
                "One",
                "Two"
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "InsertValues",
                (object)value);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(InsertedValuesCollection));

            InsertedValuesCollection collection =
                (InsertedValuesCollection)result;

            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(value, collection[0]);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_InsertValues_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                "Test",
                42,
                null,
                new List<string> { "One", "Two" }
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "InsertValues",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_InsertValues_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "InsertValues",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}