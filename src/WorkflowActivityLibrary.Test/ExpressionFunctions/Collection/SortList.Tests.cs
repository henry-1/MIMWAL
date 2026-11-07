using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Collection
{
    [TestClass]
    public class SortListTests
    {
        [TestMethod]
        public void ExpressionFunction_Collection_SortList_List_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Unsorted integer list",
                    new List<int> { 5, 2, 4, 1, 3 },
                    new object[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    "Already sorted integer list",
                    new List<int> { 1, 2, 3, 4, 5 },
                    new object[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    "Reverse sorted integer list",
                    new List<int> { 5, 4, 3, 2, 1 },
                    new object[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    "Integer list with duplicates",
                    new List<int> { 3, 1, 2, 1, 3 },
                    new object[] { 1, 1, 2, 3, 3 }
                },
                new object[]
                {
                    "Integer list with negative values",
                    new List<int> { 2, -2, 0, -1, 1 },
                    new object[] { -2, -1, 0, 1, 2 }
                },
                new object[]
                {
                    "String list",
                    new List<string> { "Charlie", "Alpha", "Bravo" },
                    new object[] { "Alpha", "Bravo", "Charlie" }
                },
                new object[]
                {
                    "Single value list",
                    new List<int> { 42 },
                    new object[] { 42 }
                },
                new object[]
                {
                    "Empty list",
                    new List<int>(),
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
                    "SortList",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(List<object>),
                    "Failed test case: " + description);

                List<object> resultList = (List<object>)result;

                Assert.AreEqual(
                    expected.Length,
                    resultList.Count,
                    "Failed test case: " + description);

                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.AreEqual(
                        expected[i],
                        resultList[i],
                        "Failed test case: " +
                        description +
                        ". Item index: " +
                        i);
                }
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_SortList_NonListValue_ShouldReturnOriginalValue()
        {
            // only List<T> will be sorted, everything else will be returned as is

            object marker = new object();

            object[][] testCases =
            {
                new object[] { "String",  "Test" },
                new object[] { "Integer", 42 },
                new object[] { "Long",    42L },
                new object[] { "Boolean", true },
                new object[] { "Object",  marker }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "SortList",
                    parameter);

                // Assert
                Assert.AreEqual(
                    parameter,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_SortList_Array_ShouldReturnOriginalArray()
        {
            // only List<T> will be sorted, everything else will be returned as is

            // Arrange
            int[] value = new int[] { 3, 1, 2 };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "SortList",
                (object)value);

            // Assert
            Assert.AreSame(value, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_SortList_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "SortList",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_SortList_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "SortList",
                new List<int> { 3, 1, 2 });

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_SortList_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { 1, 2 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "SortList",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}