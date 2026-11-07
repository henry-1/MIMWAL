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
        /// This function is used to determine the length of the specified input string.
        /// Function Syntax: Length(value:string)
        /// </summary>
        /// <returns>The length of the input string.</returns>
        private int Length()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionLength, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLengthInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return 0;
                
                int result = this.parameters[0] == null ? 0 : this.parameters[0].ToString().Length;

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionLength, "Length({0}) returned '{1}'.", arguments, result);               

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionLength, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
