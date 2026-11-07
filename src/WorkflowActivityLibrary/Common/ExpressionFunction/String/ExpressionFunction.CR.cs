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
        /// This function is used to generate a Carriage Return.
        /// Function Syntax: CR()
        /// </summary>
        /// <returns>A CR is the output.</returns>
        private string CR()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionCR, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(0))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCRInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 0, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;
                                
                string result = "\n";
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCR, "CR() returned '{0}'.", result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCR, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
