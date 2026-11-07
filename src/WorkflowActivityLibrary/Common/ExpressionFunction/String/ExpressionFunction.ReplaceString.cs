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
        /// This function is used to replace all occurrences of a string to another string.
        /// Function Syntax: ReplaceString(value:string, replace:string, with:string)
        /// </summary>
        /// <returns>A copy of the input string with all occurrences of a string replaced with another string.</returns>
        private string ReplaceString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionReplaceString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionReplaceStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));
                
                if (this.parameters[1] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionReplaceStringNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (this.parameters[2] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionReplaceStringNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));
                                
                if (this.mode == EvaluationMode.Parse)
                    return null;

                string source = this.parameters[0] == null ? null : this.parameters[0].ToString();
                string search = this.parameters[1].ToString();

                if (source != null && search.Length == 0)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionReplaceStringEmptyFunctionParameterError,
                        new InvalidArgumentException(Messages.ExpressionFunction_ReplaceStringEmptyFunctionParameterError, this.function));

                string result = source == null ? null : source.Replace(search, this.parameters[2].ToString());
                
                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionReplaceString, "ReplaceString({0}) returned '{1}'.", arguments, result);
        
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionReplaceString, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
