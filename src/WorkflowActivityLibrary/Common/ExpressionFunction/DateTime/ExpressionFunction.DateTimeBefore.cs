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
        /// This function is used to compare two DateTime values, returns true if the first DateTime occurs before the second.
        /// Function Syntax: Before(value:DateTime, comparison:DateTime)
        /// </summary>
        /// <returns>true if the first DateTime occurs before the second. Otherwise false.</returns>
        private bool Before()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionBefore, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionBeforeInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(DateTime);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionBeforeInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionBeforeInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return false;

                bool result =  this.parameters[0] != null &&
                        this.parameters[1] != null &&
                        (DateTime)this.parameters[0] < (DateTime)this.parameters[1];

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionBefore, "Before('{0}', '{1}') evaluated '{2}'.", this.parameters[0], this.parameters[1], result);                                  
 
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionBefore, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
