using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to remove leading and trailing white spaces or specified characters from a string.
        /// Function Syntax: Trim(value:string, [trimChars:string])
        /// </summary>
        /// <returns>A copy of the input string with all leading and trailing white spaces or specified characters removed.</returns>
        private string Trim()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionTrim, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1) && !this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionTrimInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, 1, 2, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                char[] trimChars = this.GetTrimChars();

                string result = this.parameters[0] == null ? null : this.parameters[0].ToString().Trim(trimChars);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionTrim, "Trim({0}) returned '{1}'.", arguments, result);
                
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionTrim, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
