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
        /// This function is used to convert a value to its string representation.
        /// Function Syntax: ConvertToString(value:integer) or ConvertToString(value:boolean) or ConvertToString(value:guid)
        /// </summary>
        /// <returns>A string representation of the given value.</returns>
        private string ConvertToString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertToString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type longType = typeof(long);
                Type boolType = typeof(bool);
                Type guidType = typeof(Guid);
                Type byteType = typeof(byte[]);
                Type stringType = typeof(string);
                object parameter = this.parameters[0];

                if (!this.VerifyType(parameter, longType) && 
                    !this.VerifyType(parameter, boolType) && 
                    !this.VerifyType(parameter, guidType) && 
                    !this.VerifyType(parameter, byteType) && 
                    !this.VerifyType(parameter, stringType)
                )
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToStringInvalidFirstFunctionParameterTypeError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError5, this.function, longType.Name, boolType.Name, guidType.Name, byteType.Name, stringType.Name, parameter.GetType().Name));
                
                if (this.mode == EvaluationMode.Parse)
                    return null;
                
                string result = parameter == null ? null : Convert.ToString(parameter, CultureInfo.InvariantCulture);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertToString, "ConvertToString({0}) returned '{1}'.", arguments, result);
                
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertToString, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
