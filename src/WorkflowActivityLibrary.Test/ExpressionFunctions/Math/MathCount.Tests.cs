using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Math
{
    [TestClass]
    public class CountTests
    {
        [TestMethod]
        public void ExpressionFunction_Math_Count_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                // Generic lists
                new object[]
                {
                    "Empty string list",
                    new List<string>(),
                    0
                },
                new object[]
                {
                    "Single string list",
                    new List<string> { "One" },
                    1
                },
                new object[]
                {
                    "Multiple string values",
                    new List<string> { "One", "Two", "Three" },
                    3
                },
                new object[]
                {
                    "Multiple integer values",
                    new List<int> { 1, 2, 3, 4, 5 },
                    5
                },
                new object[]
                {
                    "List containing null",
                    new List<string> { "One", null, "Three" },
                    3
                },

                // Non-list values count as one
                new object[] { "String",       "Test",       1 },
                new object[] { "Empty string", string.Empty, 1 },
                new object[] { "Integer",      42,           1 },
                new object[] { "Long",         42L,          1 },
                new object[] { "Boolean",      true,         1 },
                new object[] { "Object",       new object(), 1 },

                // Null
                new object[] { "Null",         null,         0 }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                int expected = (int)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Count",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Count_Array_ShouldReturnOne()
        {
            // Arrange
            int[] parameter = new int[]
            {
                1,
                2,
                3,
                4,
                5
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Count",
                (object)parameter);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ExpressionFunction_Math_Count_ParseMode_ShouldReturnZero()
        {
            object[] testCases =
            {
                new List<int> { 1, 2, 3 },
                new List<int>(),
                "Test",
                42,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Count",
                    parameter);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Math_Count_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "Count",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}