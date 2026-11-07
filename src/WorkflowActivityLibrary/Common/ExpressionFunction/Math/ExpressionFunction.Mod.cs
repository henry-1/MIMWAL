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
        /// This function is used to calculate the remainder after dividing the first integer with the second integer.
        /// Function Syntax: Mod(first:integer, second:integer)
        /// </summary>
        /// <returns>The remainder after dividing the first integer with the second integer.</returns>
        private long Mod()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionMod, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionModInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionModInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
               
                parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionModNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionModInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return 0;

                long first = Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture);
                long second = Convert.ToInt64(this.parameters[1], CultureInfo.InvariantCulture);

                if (second == 0)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionModInvalidSecondFunctionParameterValueError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError2, this.function, 2));
                
                long result = first % second;

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionMod, "Mod({0}) returned '{1}'.", arguments, result);                
                
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionMod, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
