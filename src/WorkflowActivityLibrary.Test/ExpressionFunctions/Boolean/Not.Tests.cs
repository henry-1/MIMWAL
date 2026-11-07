using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{
    [TestClass]
    public class NotTests
    {
        [TestMethod]
        public void ExpressionFunction_Boolean_Not_Parameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "True",  true,  false },
                new object[] { "False", false, true },
                new object[] { "Null",  null,  true }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Not",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Not_ParseMode_ShouldReturnFalse()
        {
            object[] testCases =
            {
                true,
                false,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Not",
                    parameter);

                // Assert
                Assert.AreEqual(false, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Not_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { true, false }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Not",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Not_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "true",
                1,
                1L,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Not",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Not_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Not",
                        "true");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}