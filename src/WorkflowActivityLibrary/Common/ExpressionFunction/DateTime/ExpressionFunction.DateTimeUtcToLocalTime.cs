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
        /// This function is used to convert a date to the local time or specified time zone.
        /// Function Syntax: DateTimeUtcToLocalTime(date:DateTime [, TimeZoneId])
        /// </summary>
        /// <returns>The value of the specified UTC date expressed in the local time or specified time zone.</returns>
        private object DateTimeUtcToLocalTime()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeUtcToLocalTime, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountBetween(1, 2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeUtcToLocalTimeInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError2, this.function, 1, 2, this.parameters.Count));
                
                Type parameterType = typeof(DateTime);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeToFileTimeUtcInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try
                {
                    object result = null;

                    if (parameter != null)
                        result = TryGetTimeFromUTC();

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeUtcToLocalTime, "DateTimeUtcToLocalTime({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (TimeZoneNotFoundException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeUtcToLocalTimeInvalidSecondFunctionParameterTypeError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_DateTimeUtcToLocalTime_InvalidFunctionParameterTimeZoneNotFoundError, this.function, this.parameters[0], this.parameters[1], e.Message));
                    
                }
                catch (InvalidTimeZoneException e)
                {
                    // cannot be tested unless manipulating/corrupting the machine's time-zone configuration, but is never the less a posssible scenario
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeUtcToLocalTimeInvalidSecondFunctionParameterTypeError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_DateTimeUtcToLocalTime_InvalidFunctionParameterInvalidTimeZoneError, this.function, this.parameters[0], this.parameters[1], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeUtcToLocalTime, EvaluationModeTemplate, this.mode);
            }
        }

        private object TryGetTimeFromUTC()
        {
            if(this.parameters.Count == 1)
                return ((DateTime)this.parameters[0]).ToLocalTime();
            
            var timeZoneId = this.parameters[1] as string;
            if (string.IsNullOrEmpty(timeZoneId))
                return ((DateTime)this.parameters[0]).ToLocalTime();            
           
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            return TimeZoneInfo.ConvertTimeFromUtc((DateTime)this.parameters[0], timeZone);
        }
    }
}
