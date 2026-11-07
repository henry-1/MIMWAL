using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Unsorted
{
    [TestClass]
    public class IIFTests
    {
        [TestMethod]
        public void ExpressionFunction_Unsorted_IIF_Condition_ShouldReturnExpectedValue()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "True returns second parameter",
                    true,
                    "Yes",
                    "No",
                    "Yes"
                },
                new object[]
                {
                    "False returns third parameter",
                    false,
                    "Yes",
                    "No",
                    "No"
                },
                new object[]
                {
                    "Null condition returns third parameter",
                    null,
                    "Yes",
                    "No",
                    "No"
                },
                new object[]
                {
                    "True with null second parameter",
                    true,
                    null,
                    "No",
                    null
                },
                new object[]
                {
                    "False with null third parameter",
                    false,
                    "Yes",
                    null,
                    null
                },
                new object[]
                {
                    "True returns integer",
                    true,
                    42,
                    100,
                    42
                },
                new object[]
                {
                    "False returns integer",
                    false,
                    42,
                    100,
                    100
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object condition = testCase[1];
                object trueValue = testCase[2];
                object falseValue = testCase[3];
                object expected = testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "IIF",
                    condition,
                    trueValue,
                    falseValue);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_IIF_ComplexValues_ShouldReturnOriginalObject()
        {
            // Arrange
            List<string> trueValue =
                new List<string>
                {
                    "One",
                    "Two"
                };

            List<string> falseValue =
                new List<string>
                {
                    "Three",
                    "Four"
                };

            // Act
            object trueResult =
                ExpressionFunctionTestHelper.Run(
                    "IIF",
                    true,
                    trueValue,
                    falseValue);

            object falseResult =
                ExpressionFunctionTestHelper.Run(
                    "IIF",
                    false,
                    trueValue,
                    falseValue);

            // Assert
            Assert.AreSame(
                trueValue,
                trueResult);

            Assert.AreSame(
                falseValue,
                falseResult);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_IIF_InvalidConditionType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "true",
                1,
                1L,
                1.0,
                new object()
            };

            foreach (object condition in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "IIF",
                            condition,
                            "Yes",
                            "No");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_IIF_ParseMode_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.RunParse(
                    "IIF",
                    true,
                    "Yes",
                    "No");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_IIF_ParseModeWithInvalidConditionType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "IIF",
                        "true",
                        "Yes",
                        "No");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_IIF_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    true
                },
                new object[]
                {
                    true,
                    "Yes"
                },
                new object[]
                {
                    true,
                    "Yes",
                    "No",
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
                            "IIF",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}