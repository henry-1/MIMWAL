using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{
    [TestClass]
    public class OrTests
    {
        [TestMethod]
        public void ExpressionFunction_Boolean_Or_TwoParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Both parameters true",    true,  true,  true },
                new object[] { "First parameter true",    true,  false, true },
                new object[] { "Second parameter true",   false, true,  true },

                // Current implementation returns true because
                // both parameters are non-null.
                new object[] { "Both parameters false",   false, false, false },

                // Null is accepted by VerifyType().
                new object[] { "First parameter null",    null,  true,  true },
                new object[] { "Second parameter null",   true,  null,  true },

                // The non-null false value makes these true
                // with the current implementation.
                new object[] { "Null and false",          null,  false, false },
                new object[] { "False and null",          false, null,  false },

                new object[] { "Both parameters null",    null,  null,  false }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Or",
                    left,
                    right);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Or_ParseMode_ShouldReturnFalse()
        {
            object[][] testCases =
            {
                new object[] { true, true },
                new object[] { true, false },
                new object[] { false, false },
                new object[] { null, true },
                new object[] { true, null },
                new object[] { null, null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "Or",
                    parameters);

                // Assert
                Assert.AreEqual(false, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Or_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { true },
                new object[] { true, false, true }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Or",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Or_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "First parameter string",   "true",       true },
                new object[] { "Second parameter string",  true,         "false" },
                new object[] { "First parameter integer",  1,            true },
                new object[] { "Second parameter integer", true,         1 },
                new object[] { "First parameter object",   new object(), true },
                new object[] { "Second parameter object",  true,         new object() }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];

                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Or",
                            left,
                            right);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Or_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Or",
                        "true",
                        false);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}