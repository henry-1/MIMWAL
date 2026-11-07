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
        /// This function is used to convert a date in the Windows file time format.
        /// Function Syntax: DateTimeToFileTimeUtc(date:DateTime)
        /// </summary>
        /// <returns>The value of the specified date expressed as a Windows file time.</returns>
        private object DateTimeToFileTimeUtc()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtc, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtcInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(DateTime);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtcInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try { 
                    object result = null;
                    if (this.parameters[0] != null)
                        result = ((DateTime)this.parameters[0]).ToFileTimeUtc();

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtc, "DateTimeToFileTimeUtc({0}) returned '{1}'.", arguments, result);
                
                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtcInvalidFirstFunctionParameterFormatError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_DateTimeToFileTimeUtc_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtc, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
