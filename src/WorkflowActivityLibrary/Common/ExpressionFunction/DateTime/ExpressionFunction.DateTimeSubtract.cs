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
        /// This function is used to get the timespan between the two dates.
        /// Function Syntax: DateTimeSubtract(date:DateTimeEndDate, date:DateTimeStartDate)
        /// </summary>
        /// <returns>The value of the specified date expressed as a Windows file time.</returns>
        private object DateTimeSubtract()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionDateTimeSubtract, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeSubtractInvalidFunctionParameterCountError, new InvalidFunctionFormatException(
                        Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                object minuend = this.parameters[0];
                object subtrahend = this.parameters[1];

                if (minuend == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeSubtractNullFunctionParameterError, new InvalidFunctionFormatException(
                        Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));
                
                if (subtrahend == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeSubtractNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));
                
                Type parameterType = typeof(DateTime);                
                if (!this.VerifyType(minuend, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeSubtractInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, minuend.GetType().Name));
                
                if (!this.VerifyType(subtrahend, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionDateTimeSubtractInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, subtrahend.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                object result = ((DateTime)minuend).Subtract((DateTime)subtrahend);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionDateTimeSubtract, "DateTimeSubtract({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionDateTimeSubtract, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
