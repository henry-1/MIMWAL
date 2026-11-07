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
        /// This function is used to multiply two integer values.
        /// Function Syntax: Multiply(first:integer, second:integer)
        /// </summary>
        /// <returns>The result of the multiplication of the two integer values.</returns>
        /// <remarks>
        /// Null operands are treated as 0 for backward compatibility: <c>Multiply(5, null)</c> and
        /// <c>Multiply(null, 5)</c> both return 0, because <c>Convert.ToInt64((object)null, ...)</c> returns 0.
        /// The previous version of MIM WAL behaved the same way, and existing workflows may rely on it,
        /// so this is intentionally not treated as an error.
        /// But I also introduced the checked multiplication to not silently produce a false result.
        /// </remarks>
        private long Multiply()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionMultiply, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMultiplyInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMultiplyInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMultiplyInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return 0;

                long first = Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture);
                long second = Convert.ToInt64(this.parameters[1], CultureInfo.InvariantCulture);

                long result;

                try
                {
                    result = checked(first * second);
                }
                catch (OverflowException)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMultiplyOverflowError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_MultiplyOverflowError, this.function, first, second));
                }

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionMultiply, "Multiply({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionMultiply, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
