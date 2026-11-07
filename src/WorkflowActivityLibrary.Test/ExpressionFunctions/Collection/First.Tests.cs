using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Collection
{
    [TestClass]
    public class FirstTests
    {
        [TestMethod]
        public void ExpressionFunction_Collection_First_Parameter_ShouldReturnExpectedResult()
        {
            object marker = new object();

            object[][] testCases =
            {
                // Generic lists
                new object[]
                {
                    "String list", new List<string> { "First", "Second", "Third" }, "First"
                },
                new object[]
                {
                    "Integer list", new List<int> { 1, 2, 3 }, 1
                },
                new object[]
                {
                    "Boolean list", new List<bool> { true, false }, true
                },
                new object[]
                {
                    "List with null as first value", new List<string> { null, "Second" }, null
                },
                new object[]
                {
                    "Empty string list", new List<string>(), null
                },
                new object[]
                {
                    "Empty integer list", new List<int>(), null
                },

                // Non-list values are returned unchanged
                new object[] { "String",       "Test",  "Test" },
                new object[] { "Empty string", "",      "" },
                new object[] { "Integer",      42,      42 },
                new object[] { "Long",         42L,     42L },
                new object[] { "Boolean",      true,    true },
                new object[] { "Object",       marker,  marker },

                // Null
                new object[] { "Null",         null,    null }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                object expected = testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "First",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_First_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                "Test",
                42,
                true,
                new List<string> { "First", "Second" },
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "First",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Collection_First_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "First", "Second" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "First",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}