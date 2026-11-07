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
        /// This function is used to determine if the object specified is non-null.
        /// Function Syntax: IsPresent(value:object)
        /// </summary>
        /// <returns>true if the object referenced in not null. Otherwise false.</returns>
        private bool IsPresent()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionIsPresent, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionIsPresentInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                                
                bool result = this.parameters[0] != null;

                if (this.mode != EvaluationMode.Parse)
                {
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionIsPresent, "IsPresent('{0}') evaluated '{1}'.", this.parameters[0], result);
                }

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionIsPresent, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
