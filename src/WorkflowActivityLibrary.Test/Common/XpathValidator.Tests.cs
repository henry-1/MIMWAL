using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;

namespace WorkflowActivityLibrary.Tests.Common
{
    [TestClass]
    public class XpathValidatorTests
    {        
        [TestMethod]
        public void IsValidXPath_ValidAbsoluteXPath_ReturnsTrue()
        {
            string[] expressions =
            {
            "/Person",
            "/Person/DisplayName",
            "/Person[AccountName='test']",
            "/Person[AccountName=\"test\"]",
            "/Person[AccountName!='test']",
            "/Person[EmployeeID='123']/DisplayName",
            "/EmailTemplate[DisplayName='Default pending approval email template']",
            "/EmailTemplate[DisplayName=Concatenate('4','2')]"
        };

            foreach (string expression in expressions)
            {
                Assert.IsTrue(
                    XpathValidator.IsXPath(expression),
                    "Expected XPath to be valid: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_ValidAbsoluteXPathWithWhitespace_ReturnsTrue()
        {
            Assert.IsTrue(
                XpathValidator.IsXPath(
                    "  /Person[AccountName='test']  "));
        }

        [TestMethod]
        public void IsValidXPath_NullOrEmpty_ReturnsFalse()
        {
            string[] expressions =
            {
            null,
            String.Empty,
            " ",
            "   "
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected XPath to be invalid: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_RelativeXPath_ReturnsFalse()
        {
            string[] expressions =
            {
            "a",
            "Person",
            "Person/DisplayName",
            "*",
            ".",
            "@id"
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected relative XPath to be invalid: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_EmptyAbsolutePath_ReturnsFalse()
        {
            string[] expressions =
            {
            "/",
            "//",
            " / ",
            " // "
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected XPath to be invalid: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_MalformedXPath_ReturnsFalse()
        {
            string[] expressions =
            {
            "/Person[",
            "/Person]",
            "/Person[AccountName=]",
            "/Person[AccountName='test'",
            "/Person[AccountName=='test']",
            "/Person/"
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected malformed XPath to be invalid: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_Number_ReturnsFalse()
        {
            string[] expressions =
            {
            "0",
            "1",
            "2",
            "-1",
            "123"
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected number to not be XPath: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_StringLiteral_ReturnsFalse()
        {
            string[] expressions =
            {
            "\"2\"",
            "'2'",
            "\"test\"",
            "'test'"
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected string literal to not be XPath: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_StandaloneWalFunction_ReturnsFalse()
        {
            string[] expressions =
            {
            "Add(2,2)",
            "Concatenate('4','2')",
            "Count('test')",
            "Null()"
        };

            foreach (string expression in expressions)
            {
                Assert.IsFalse(
                    XpathValidator.IsXPath(expression),
                    "Expected WAL function to not be XPath: " + expression);
            }
        }

        [TestMethod]
        public void IsValidXPath_WalFunctionInsideXPath_ReturnsTrue()
        {
            Assert.IsTrue(
                XpathValidator.IsXPath(
                    "/EmailTemplate[DisplayName=Concatenate('Email','Template')]"));
        }
    }
}

