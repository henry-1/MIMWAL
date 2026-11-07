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
        /// This function is used to determine if the first string matches the regular expression pattern specified in the second string.
        /// Function Syntax: RegexMatch(value:string, pattern:string)
        /// </summary>
        /// <returns>true if the first string matches the regular expression pattern specified in the second string. Otherwise false.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S6444", Justification = "Regex is constrained to a simple, bounded pattern.")]
        private bool RegexMatch()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRegexMatch, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRegexMatchInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                if (this.parameters[1] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRegexMatchNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (this.mode == EvaluationMode.Parse)
                    return false;

                try
                { 
                    bool result = this.parameters[0] != null && 
                        Regex.Match(this.parameters[0].ToString(), this.parameters[1].ToString()).Success;

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRegexMatch, "RegexMatch({0}) returned '{1}'.", arguments, result);                             

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionStringregexMatchFunctionParameterFormatError,
                       new InvalidFunctionFormatException(Messages.ExpressionFunction_StringRegexMatch_InvalidFunctionParameterFormatError, this.function, this.parameters[1], this.parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRegexMatch, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
