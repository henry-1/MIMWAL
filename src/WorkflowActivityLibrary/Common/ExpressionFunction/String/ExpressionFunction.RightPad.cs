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
        /// This function is used to return a string with characters left-aligned, padding with the specified Unicode character or spaces on the right for a specified total length.
        /// Function Syntax: RightPad(value:string, length:integer, padding:char)
        /// </summary>
        /// <returns>A new string that is equivalent to string specified in the first input parameter, but left-aligned and padded on the right with as many padding characters as needed to create the specified length.</returns>
        private string RightPad()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRightPad, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightPadInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));

                Type parameterType = typeof(int);
                object intParameter = this.parameters[1];
                if (intParameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightPadNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(intParameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightPadInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, intParameter.GetType().Name));

                object paddingParameter = this.parameters[2];

                if (paddingParameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightPadNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));

                if (this.mode == EvaluationMode.Parse)
                    return null;                 

                string result = GetRightPaddedString(intParameter, paddingParameter);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRightPad, "RightPad({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRightPad, EvaluationModeTemplate, this.mode);
            }
        }

        private string GetRightPaddedString(object intParameter, object paddingParameter)
        {
            int totalWidth = Convert.ToInt32(intParameter, CultureInfo.InvariantCulture);

            if (this.parameters[0] == null)
                return null;

            // do additional parameter validation check during execution as it may be a lookup expression than a design-time value. 
            Type parameterType = typeof(char);
            if (paddingParameter.ToString().ToCharArray().Length != 1)
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightPadInvalidThirdFunctionParameterTypeError, new InvalidArgumentException(
                    Messages.ExpressionFunction_InvalidThirdFunctionParameterTypeError2, this.function, parameterType.Name, paddingParameter.GetType().Name, paddingParameter));
            
            return this.parameters[0].ToString().PadRight(totalWidth, paddingParameter.ToString()[0]);
        }
    }
}
