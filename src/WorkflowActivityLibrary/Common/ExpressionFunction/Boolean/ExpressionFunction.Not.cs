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
        /// This function negates the value of Boolean condition.
        /// Function Syntax: Not(condition:boolean)
        /// </summary>
        /// <returns>true if the specified values if false. Otherwise true.</returns>
        private bool Not()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionNot, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionNotInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(bool);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionNotInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return false;

                bool result =  this.parameters[0] == null || !(bool)this.parameters[0];                  

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionNot, "Not('{0}') evaluated '{1}'.", this.parameters[0], result);                
               
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionNot, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
