using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Unsorted
{
    [TestClass]
    public class GenerateRandomPasswordTests
    {
        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_ValidLength_ShouldReturnPasswordWithExpectedLength()
        {
            // Arrange
            int length = 12;

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "GenerateRandomPassword",
                length);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(string));

            Assert.AreEqual(
                length,
                ((string)result).Length);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_NullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "GenerateRandomPassword",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "12",
                true,
                12.0,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "GenerateRandomPassword",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_ParseMode_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.RunParse(
                    "GenerateRandomPassword",
                    12);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_ParseModeWithNullParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "GenerateRandomPassword",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "GenerateRandomPassword",
                        "12");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    12,
                    16
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "GenerateRandomPassword",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_LengthBelowMinimum_ShouldThrowInvalidArgumentException()
        {
            int[] testCases =
            {
                int.MinValue,
                -1,
                0,
                1,
                2,
                3
            };

            foreach (int length in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "GenerateRandomPassword",
                            length);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
        }
        

        [TestMethod]
        public void ExpressionFunction_Unsorted_GenerateRandomPassword_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
        {
            object[] testCases =
            {
                (long)int.MaxValue + 1L,
                (long)int.MinValue - 1L
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "GenerateRandomPassword",
                            parameter);
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }
    }
}