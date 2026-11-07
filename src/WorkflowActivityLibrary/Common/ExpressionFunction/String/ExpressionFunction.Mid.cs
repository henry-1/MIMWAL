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
        /// This function is used to return a specified number of characters from a specified position in a string.
        /// Function Syntax: Mid(value:string, start:integer, length:integer)
        /// </summary>
        /// <returns>A copy of the input string with the specified number of characters from the specified position in the input string.</returns>
        private string Mid()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionMid, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMidInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));

                Type parameterType = typeof(int);
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMidNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMidInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[2];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMidNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMidInvalidThirdFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidThirdFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try
                { 
                    int startIndex = Convert.ToInt32(this.parameters[1], CultureInfo.InvariantCulture);
                    int length = Convert.ToInt32(this.parameters[2], CultureInfo.InvariantCulture);
                    string result = GetSubstring(startIndex, length);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionMid, "Mid({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionMidFunctionParameterFormatError,
                       new InvalidArgumentException(Messages.ExpressionFunction_StringMid_InvalidFunctionParameterFormatError, this.function, this.parameters[0], this.parameters[1], this.parameters[2], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionMid, EvaluationModeTemplate, this.mode);
            }
        }

        private string GetSubstring(int startIndex, int length)
        {
            if (this.parameters[0] == null)                
                return null;

            string value = this.parameters[0].ToString();

            if (value.Length <= startIndex)
                return null;

            if (startIndex + length > value.Length)
                return value.Substring(startIndex);

            return value.Substring(startIndex, length);          
        }
    }
}
