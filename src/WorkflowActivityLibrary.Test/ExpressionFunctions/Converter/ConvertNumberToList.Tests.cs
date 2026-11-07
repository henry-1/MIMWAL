using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertNumberToListTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_Number_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "One",
                    1,
                    new int[] { 1 }
                },
                new object[]
                {
                    "Three",
                    3,
                    new int[] { 1, 2, 3 }
                },
                new object[]
                {
                    "Five",
                    5,
                    new int[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    "Long value",
                    3L,
                    new int[] { 1, 2, 3 }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                int[] expected = (int[])testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertNumberToList",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(List<int>),
                    "Failed test case: " + description);

                CollectionAssert.AreEqual(
                    expected,
                    ((List<int>)result).ToArray(),
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_NonPositiveNumber_ShouldReturnNull()
        {
            object[] testCases =
            {
                0,
                -1,
                -10,
                0L,
                -1L
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ConvertNumberToList",
                    parameter);

                // Assert
                Assert.IsNull(
                    result,
                    "Expected null for parameter: " + parameter);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertNumberToList",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                1,
                5,
                5L,
                0,
                -1,
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertNumberToList",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "ConvertNumberToList",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "5",
                true,
                new object(),
                5.0,
                5.0f
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertNumberToList",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertNumberToList_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertNumberToList",
                        "5");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}