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
        /// This function is used to convert all characters in a string to the upper case based on current culture.
        /// Function Syntax: UpperCase(value:string)
        /// </summary>
        /// <returns>A copy of the input string converted to uppercase using the casing rules of the current culture.</returns>
        private string UpperCase()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionUpperCase, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionUpperCaseInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = this.parameters[0] == null ? null : this.parameters[0].ToString().ToUpper(CultureInfo.CurrentCulture);

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionUpperCase, "UpperCase('{0}') returned '{1}'.", this.parameters[0], result);              

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionUpperCase, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
