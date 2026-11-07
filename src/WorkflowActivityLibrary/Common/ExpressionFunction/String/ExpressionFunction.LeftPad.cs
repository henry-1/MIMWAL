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
        /// This function is used to return a string with characters Right-aligned, padding with the specified Unicode character or spaces on the left for a specified total length.
        /// Function Syntax: LeftPad(value:string, length:integer, padding:char)
        /// </summary>
        /// <returns>A new string that is equivalent to string specified in the first input parameter, but right-aligned and padded on the left with as many padding characters as needed to create the specified length.</returns>
        private string LeftPad()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionLeftPad, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftPadInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));
                
                Type parameterType = typeof(int);
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftPadNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));
                
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftPadInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                object padParameter = this.parameters[2];
                if (padParameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftPadNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));
                
                if (this.mode == EvaluationMode.Parse)
                    return null;

                try
                {
                    string result = null;

                    if (this.parameters[0] != null)
                    {
                        // Additional validation happens during execution, as the parameter may be a
                        // lookup expression rather than a design-time value
                        string padText = padParameter.ToString();
                        if (padText.Length != 1)
                            throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftPadInvalidThirdFunctionParameterTypeError, new InvalidArgumentException(Messages.ExpressionFunction_InvalidThirdFunctionParameterTypeError2, this.function, typeof(char).Name, padParameter.GetType().Name, padParameter));

                        int length = Convert.ToInt32(parameter, CultureInfo.InvariantCulture);
                        result = this.parameters[0].ToString().PadLeft(length, padText[0]);
                    }

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionLeftPad, "LeftPad({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (OverflowException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLeftPadInvalidThirdFunctionParameterFormatError,
                       new OverflowNotAllowedException(Messages.ExpressionFunction_StringLeft_InvalidFunctionParameterFormatError, this.function, this.parameters[0], this.parameters[1], this.parameters[2], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionLeftPad, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
