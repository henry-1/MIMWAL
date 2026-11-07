using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to wrap the xpath filter into a FIM filter.
        /// Function Syntax: WrapXPathFilter(filter:string)
        /// </summary>
        /// <returns>The XPath that conforms to FIM filter syntax</returns>
        private string WrapXPathFilter()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionWrapXPathFilter, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWrapXPathFilterInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                if (this.mode == EvaluationMode.Parse)
                    return null;

                object parameter = this.parameters[0];
                string result = null;

                if (parameter != null)
                {
                    result = string.Format(
                        CultureInfo.InvariantCulture,
                        "<Filter xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Dialect=\"http://schemas.microsoft.com/2006/11/XPathFilterDialect\" xmlns=\"http://schemas.xmlsoap.org/ws/2004/09/enumeration\">{0}</Filter>",
                        parameter);
                }

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionWrapXPathFilter, "WrapXPathFilter({0}) returned '{1}'.", arguments, result);              

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionWrapXPathFilter, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
