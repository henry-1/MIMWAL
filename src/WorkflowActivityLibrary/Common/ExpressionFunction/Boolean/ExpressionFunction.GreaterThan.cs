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
        /// This function is used to determine if the first integer value is greater than the second.
        /// Function Syntax: GreaterThan(value:integer, comparison:integer)
        /// </summary>
        /// <returns>true if the first integer value is greater than the second. Otherwise false.</returns>
        private bool GreaterThan()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionGreaterThan, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionGreaterThanInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionGreaterThanInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionGreaterThanInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                
                if (this.mode == EvaluationMode.Parse)
                    return false;

                bool result = this.parameters[0] != null && 
                    this.parameters[1] != null &&
                    Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture) > Convert.ToInt64(this.parameters[1], CultureInfo.InvariantCulture);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionGreaterThan, "GreaterThan({0}) evaluated '{1}'.", arguments, result);
                          
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionGreaterThan, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
