using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{
    [TestClass]
    public class IsPresentTests
    {
        [TestMethod]
        public void ExpressionFunction_Boolean_IsPresent_Parameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
            new object[] { "String",           "Test",       true },
            new object[] { "Empty string",     string.Empty, true },
            new object[] { "Integer zero",     0,            true },
            new object[] { "Integer",          42,           true },
            new object[] { "Boolean true",     true,         true },
            new object[] { "Boolean false",    false,        true },
            new object[] { "Object",           new object(), true },
            new object[] { "Null",             null,         false }
        };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "IsPresent",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_IsPresent_ParseMode_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
            new object[] { "Non-null parameter", "Test", true },
            new object[] { "Null parameter",     null,   false }
        };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "IsPresent",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_IsPresent_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "IsPresent",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
    }