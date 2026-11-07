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
        /// This function is used to subtract the second integer value from the first.
        /// Function Syntax: Subtract(value:integer, value:integer)
        /// </summary>
        /// <returns>The result of the subtraction of the second integer value from the first.</returns>
        private long Subtract()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionSubtract, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSubtractInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSubtractInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSubtractInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                long result = 0;

                if (this.mode == EvaluationMode.Parse)
                    return result;

                try
                {
                    result = checked(
                        Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture) - Convert.ToInt64(this.parameters[1], CultureInfo.InvariantCulture));

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionSubtract, "Subtract('{0}', '{1}') returned '{2}'.", this.parameters[0], this.parameters[1], result);

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSubtractInvalidFunctionParameterFormatError,
                       new InvalidFunctionFormatException(Messages.ExpressionFunction_Substract_InvalidFunctionParameterError, this.function, this.parameters[1], this.parameters[0], e.Message));
                }                
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionSubtract, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
