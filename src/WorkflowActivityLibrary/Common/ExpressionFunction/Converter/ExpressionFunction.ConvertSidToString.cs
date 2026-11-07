using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to convert a byte array containing a security identifier to a string.
        /// Function Syntax: ConvertSidToString(sid:binary)
        /// </summary>
        /// <returns>A string representation of the SID.</returns>
        private string ConvertSidToString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertSidToString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertSidToStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(byte[]);
                object parameter = this.parameters[0];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertSidToStringNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));
                
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertSidToStringInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try
                {
                    SecurityIdentifier si = new SecurityIdentifier((byte[])parameter, 0);
                    string result = si.ToString();

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertSidToString, "ConvertSidToString({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionConvertSidToStringInvalidSidError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSidError, this.function, parameters[0], e.Message));
                }                
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertSidToString, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
