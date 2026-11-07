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
        /// This function is used to convert a string or a boolean to a number.
        /// Function Syntax: ConvertToNumber(value:string) or ConvertToNumber(value:boolean)
        /// </summary>
        /// <returns>A number representation of the given value.</returns>
        private long ConvertToNumber()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertToNumber, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToNumberInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type stringType = typeof(string);
                Type boolType = typeof(bool);
                Type longType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, stringType) && !this.VerifyType(parameter, boolType) && !this.VerifyType(parameter, longType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToNumberInvalidFirstFunctionParameterTypeError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError3, this.function, stringType.Name, boolType.Name, longType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return 0;
                try { 
                    long result = parameter == null ? 0 : Convert.ToInt64(parameter, CultureInfo.InvariantCulture);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertToNumber, "ConvertToNumber({0}) returned '{1}'.", arguments, result);
                
                    return result;
                }
                catch (OverflowException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToNumberInvalidFunctionParameterFormatError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_ConvertToNumber_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }
                catch (FormatException e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToNumberInvalidFunctionParameterFormatError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_ConvertToNumber_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }
        }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertToNumber, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
