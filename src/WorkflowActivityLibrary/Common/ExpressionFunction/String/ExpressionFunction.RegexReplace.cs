using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to replace all occurrences of a regular expression pattern in a string to another string.
        /// Function Syntax: RegexReplace(value:string, pattern:string, with:string)
        /// </summary>
        /// <returns>A copy of the input string with all occurrences of the specified second input regular expression pattern in the first input string replaced with the third input string.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S6444", Justification = "Regex is constrained to a simple, bounded pattern.")]
        private string RegexReplace()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRegexReplace, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRegexReplaceInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));
                
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRegexReplaceNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));
                
                parameter = this.parameters[2];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRegexReplaceNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = this.parameters[0] == null ? null : Regex.Replace(this.parameters[0].ToString(), this.parameters[1].ToString(), this.parameters[2].ToString());

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRegexReplace, "RegexReplace({0}) returned '{1}'.", arguments, result);
                
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRegexReplace, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
