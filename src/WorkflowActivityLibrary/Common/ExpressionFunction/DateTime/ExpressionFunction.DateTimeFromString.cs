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
        /// This function is used to convert string representation of a date and time to its DateTime equivalent.
        /// If the string does not contain time zone info, it's assumed to be UTC.
        /// If the optional culture name parameter is not specified, it's assumed to be invariant culture.
        /// Function Syntax: DateTimeFromString(string:dateTime [, culture:cultureName])
        /// </summary>
        /// <returns>A DateTime equivalent to specified string. If the string is null, a null is returned.</returns>
        private object DateTimeFromString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeFromString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountBetween(1, 2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFromStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError2, this.function, 1, 2, this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter0 = this.parameters[0];
                if (!this.VerifyType(parameter0, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFromStringInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter0.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string parameter1 = this.parameters.Count == 2 ? this.parameters[1] as string : string.Empty;
                object result = null;

                try { 
                    if (parameter0 != null)
                        result = TryGetDateTimeAsString(parameter0, parameter1, this.parameters.Count);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeFromString, "DateTimeFromString({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (FormatException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFromStringInvalidFunctionParameterFormatError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_DateTimeFromString_InvalidFunctionParameterFormatError, this.function, parameter0, parameter1, e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeFromString, EvaluationModeTemplate, this.mode);
            }
        }

        private static object TryGetDateTimeAsString(object parameter0, string parameter1, int parameterCount)
        {            
            var cultureInfo = CultureInfo.InvariantCulture;
             
            if (parameterCount == 2)
                
                cultureInfo = new CultureInfo(parameter1);
                                    
            return DateTime.Parse(parameter0 as string, cultureInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);            
        }
    }
}
