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
        /// This function is used to convert the specified Windows file time to an equivalent UTC time..
        /// Function Syntax: DateTimeFromFileTimeUtc(long:fileTime)
        /// </summary>
        /// <returns>An object that represents the UTC time equivalent of the date and time represented by the fileTime parameter.</returns>
        private object DateTimeFromFileTimeUtc()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeFromFileTimeUtc, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFromFileTimeUtcInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFromFileTimeUtcInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try { 
                    object result = null;
                
                    if (this.parameters[0] != null)
                        result = DateTime.FromFileTimeUtc(Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture));

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeFromFileTimeUtc, "DateTimeFromFileTimeUtc({0}) returned '{1}'.", arguments, result);
                
                    return result;

                }
                catch (ArgumentException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFromFileTimeUtcFunctionParameterFormatError,
                        new InvalidArgumentException(Messages.ExpressionFunction_DateTimeFromFileTimeUtc_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeFromFileTimeUtc, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
