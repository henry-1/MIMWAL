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
        /// This function is used to get a DateTime object that is set to the current date and time on FIM server, expressed as the Coordinated Universal Time (UTC).
        /// Function Syntax: DateTimeNow()
        /// </summary>
        /// <returns>A DateTime whose value is the current UTC date and time.</returns>
        private DateTime DateTimeNow()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeNow, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(0))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeNowInvalidFunctionParameterCountError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 0, this.parameters.Count));
                
                DateTime result = DateTime.UtcNow;

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeNow, "DateTimeNow() returned '{0}'.", result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeNow, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
