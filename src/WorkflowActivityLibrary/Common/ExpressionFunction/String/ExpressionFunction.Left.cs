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
        /// This function is used to return a string formed with the leading characters of the string value specified in the first parameter and the length specified in the second parameter.
        /// Function Syntax: Left(value:string, length:integer)
        /// </summary>
        /// <returns>The leading substring of specified length. If the specified length is greater than the input string, the entire input string is returned.</returns>
        private string Left()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionLeft, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                Type parameterType = typeof(int);
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));
                
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;
                try
                {
                    int length = Convert.ToInt32(this.parameters[1], CultureInfo.InvariantCulture);
                    string result = this.parameters[0] == null ? null : this.parameters[0].ToString().PadRight(length).Substring(0, length).Trim();

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionLeft, "Left({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (OverflowException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionStringLeftFunctionParameterFormatError,
                       new OverflowNotAllowedException(Messages.ExpressionFunction_StringLeft_InvalidFunctionParameterFormatError, this.function, this.parameters[1], this.parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionLeft, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
