using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class CRTests
    {
        [TestMethod]
        public void ExpressionFunction_String_CR_NoParameters_ShouldReturnNewLine()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "CR");

            // Assert
            Assert.AreEqual(
                "\n",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_CR_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "CR");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_CR_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { "Test" },
                new object[] { null },
                new object[] { "One", "Two" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CR",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}