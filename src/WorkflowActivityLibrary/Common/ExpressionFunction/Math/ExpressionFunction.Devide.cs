using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
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
        /// This function is used to divide the first integer value by the second integer value.
        /// Function Syntax: Divide(first:integer, second:integer)
        /// </summary>
        /// <returns>The integer quotient of the division of the first integer value by the second integer value.</returns>
        private long Divide()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDivide, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDivideInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDivideInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDivideInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                long result = 0;

                if (this.mode == EvaluationMode.Parse)
                    return result;                
                
                long first = Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture);
                long second = Convert.ToInt64(this.parameters[1], CultureInfo.InvariantCulture);

                if (second == 0)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDivideInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError2, this.function, 2));
                    
                result = first / second;

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDivide, "Divide({0}) returned '{1}'.", arguments, result);

                return result;
                
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDivide, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
