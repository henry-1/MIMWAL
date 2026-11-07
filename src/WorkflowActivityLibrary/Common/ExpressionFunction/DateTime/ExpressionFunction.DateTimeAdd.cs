using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to add the value of the specified second TimeSpan string to first DateTime or string value.
        /// Function Syntax: DateTimeAdd(date:DateTime, timespan:string)
        /// </summary>
        /// <returns>A DateTime whose value is the sum of the date and time represented by first parameter and the time interval represented by second parameter.</returns>
        private object DateTimeAdd()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeAdd, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeAddInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                Type parameterType = typeof(DateTime);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeAddInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                parameterType = typeof(string);
                Type parameterType2 = typeof(TimeSpan);
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeAddInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError3, this.function, parameterType.Name, parameterType2.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try
                {
                    object result = AddDateTime(this.parameters[1]);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeAdd, "DateTimeAdd({0}) returned '{1}'.", arguments, result);
                
                    return result;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeAddInvalidSecondFunctionParameterFormatError,
                       new InvalidArgumentException(Messages.ExpressionFunction_DateTimeAdd_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }

            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeAdd, EvaluationModeTemplate, this.mode);
            }
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S6580", Justification = "IFormatProvider overload of TimeSpan.TryParse not available in .NET 3.5")]
        private object AddDateTime(object timeSpan)
        {

            if (this.parameters[0] == null)
                return null;

            DateTime date = (DateTime)this.parameters[0];

            if (timeSpan is TimeSpan)
                return date.Add((TimeSpan)timeSpan);

            // Additional validation during execution, as the value may be a lookup expression rather than a design-time value
            TimeSpan parsed;
            if (TimeSpan.TryParse(timeSpan as string, out parsed))
                return date.Add(parsed);

            throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeAddInvalidSecondFunctionParameterTypeError,
                new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError2, this.function, typeof(TimeSpan).Name, timeSpan.GetType().Name, timeSpan));            
        }
    }
}
