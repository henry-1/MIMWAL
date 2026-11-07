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
        /// This function is to convert the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).
        /// Function Syntax: TitleCase(value:string)
        /// </summary>        
        /// <returns>Returns the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).</returns>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.globalization.textinfo.totitlecase"/> for the exact behavior.
        /// </remarks>
        private string TitleCase()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionTitleCase, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionTitleCaseInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = this.parameters[0] == null ? null : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(this.parameters[0].ToString());                

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionTitleCase, "TitleCase('{0}') returned '{1}'.", this.parameters[0], result);                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionTitleCase, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
