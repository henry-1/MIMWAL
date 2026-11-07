using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    /// <summary>
    /// Provides syntax-only validation for XPath expressions.
    /// The expression is compiled but never evaluated.
    /// </summary>
    public static class XpathValidator
    {
        /// <summary>
        /// Determines whether the supplied value is a syntactically valid
        /// absolute XPath expression suitable for a MIM WAL form field.
        /// </summary>
        public static bool IsXPath(string expression)
        {
            string ignored;
            return IsValidXPath(expression, out ignored);
        }

        /// <summary>
        /// Determines whether the supplied value is a syntactically valid
        /// absolute XPath expression suitable for a MIM WAL form field.
        /// </summary>
        public static bool IsValidXPath(
            string expression,
            out string validationError)
        {
            validationError = null;

            if (String.IsNullOrEmpty(expression))
            {
                validationError = "The XPath expression cannot be empty.";
                return false;
            }

            string trimmedExpression = expression.Trim();

            if (trimmedExpression.Length == 0)
            {
                validationError = "The XPath expression cannot be empty.";
                return false;
            }

            /*
             * XPath allows relative location paths such as "a".
             * MIM WAL form fields should only accept clearly identifiable
             * XPath expressions, so require an absolute path.
             */
            if (!trimmedExpression.StartsWith("/", StringComparison.Ordinal))
            {
                validationError =
                    "The XPath expression must start with '/'.";
                return false;
            }

            /*
             * "/" is syntactically valid XPath, and some XPath parsers may
             * also accept "//", but neither identifies a meaningful value
             * or resource path for this validation scenario.
             */
            if (String.Equals(trimmedExpression, "/", StringComparison.Ordinal) ||
                String.Equals(trimmedExpression, "//", StringComparison.Ordinal))
            {
                validationError =
                    "The XPath expression must contain a path.";
                return false;
            }

            try
            {
                XPathExpression.Compile(trimmedExpression);
                return true;
            }
            catch (XPathException ex)
            {
                validationError = ex.Message;
                return false;
            }
        }

    }
}
