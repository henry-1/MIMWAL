using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class CRLFTests
    {
        [TestMethod]
        public void ExpressionFunction_String_CRLF_NoParameters_ShouldReturnCarriageReturnLineFeed()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "CRLF");

            // Assert
            Assert.AreEqual(
                "\r\n",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_String_CRLF_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "CRLF");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_CRLF_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "CRLF",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}