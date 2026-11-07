using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Unsorted
{
    [TestClass]
    public class WrapXPathFilterTests
    {
        [TestMethod]
        public void ExpressionFunction_Unsorted_WrapXPathFilter_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "XPath filter",
                    "/Person[AccountName='Max']",
                    "/Person[AccountName='Max']"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    string.Empty
                },
                new object[]
                {
                    "Integer",
                    42,
                    "42"
                },
                new object[]
                {
                    "Boolean",
                    true,
                    "True"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                string expectedContent = (string)testCase[2];

                string expected =
                    "<Filter " +
                    "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                    "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                    "Dialect=\"http://schemas.microsoft.com/2006/11/XPathFilterDialect\" " +
                    "xmlns=\"http://schemas.xmlsoap.org/ws/2004/09/enumeration\">" +
                    expectedContent +
                    "</Filter>";

                // Act
                object result =
                    ExpressionFunctionTestHelper.Run(
                        "WrapXPathFilter",
                        parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_WrapXPathFilter_NullParameter_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.Run(
                    "WrapXPathFilter",
                    (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_WrapXPathFilter_ParseMode_ShouldReturnNull()
        {
            // Act
            object result =
                ExpressionFunctionTestHelper.RunParse(
                    "WrapXPathFilter",
                    "/Person[AccountName='Max']");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Unsorted_WrapXPathFilter_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "One",
                    "Two"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "WrapXPathFilter",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}