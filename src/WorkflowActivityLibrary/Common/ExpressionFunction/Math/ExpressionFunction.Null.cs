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
        /// This function is used to return a null object.
        /// It is used in conjunction with Update Resources whenever you wish to delete an existing value when the Allow Nulls option is selected.
        /// Function Syntax: Null()
        /// </summary>
        /// <returns>A Null object.</returns>
        private object Null()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionNull, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(0))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionNullInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 0, this.parameters.Count));
                
                return null;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionNull, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
