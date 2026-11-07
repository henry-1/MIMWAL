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
        /// This function is used to format the value of the first DateTime parameter in the format specified in the second string parameter.
        /// Function Syntax: DateTimeFormat(date:DateTime, format:string [, culture:name])
        /// </summary>
        /// <returns>The string representation of the first DateTime parameter in the format specified in the second string parameter.</returns>
        private string DateTimeFormat()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeFormat, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountBetween(2, 3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFormatInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError2, this.function, 2, 3, this.parameters.Count));

                if (this.parameters[1] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFormatNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                Type parameterType = typeof(DateTime);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFormatInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                if (this.parameters[0] == null)
                    return null;

                try
                {
                    CultureInfo cultureInfo = GetCultureInfo();
                    string result = ((DateTime)this.parameters[0]).ToString(this.parameters[1].ToString(), cultureInfo);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeFormat, "DateTimeFormat({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (ArgumentException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeFormatFunctionParameterFormatError,
                        new InvalidArgumentException(Messages.ExpressionFunction_DateTimeFormat_InvalidFunctionParameterFormatError, this.function, this.parameters[0], this.parameters[1], this.parameters[2], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeFormat, EvaluationModeTemplate, this.mode);
            }
        }
        

        private CultureInfo GetCultureInfo()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            if (this.parameters.Count == 3)
                cultureInfo = new CultureInfo(this.parameters[2] as string);                

            return cultureInfo;
        }
    }
}
