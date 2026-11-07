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
        /// This function is used to convert all characters in a string to the lower case based on current culture.
        /// Function Syntax: LowerCase(value:string)
        /// </summary>
        /// <remarks>
        /// As the original MIM WAL uses current culture here, 
        /// the output can differ between machines or service accounts with different regional settings.
        /// </remarks>
        /// <returns>A copy of the input string converted to lowercase using the casing rules of the current culture.</returns>
        private string LowerCase()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionLowerCase, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLowerCaseInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;
                
                string result = this.parameters[0] == null ? null : this.parameters[0].ToString().ToLower(CultureInfo.CurrentCulture);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionLowerCase, "LowerCase({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionLowerCase, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
