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
        /// This function is used to generate a Carriage Return/Line Feed.
        /// Function Syntax: CRLF()
        /// </summary>
        /// <returns>A CRLF is the output.</returns>
        private string CRLF()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionCrlf, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(0))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCrlfInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 0, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;
               
                string result = "\r\n";
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCrlf, "CRLF() returned '{0}'.", result);
  
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCrlf, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
