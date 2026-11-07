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
        /// This function is used to convert a base64 string to a byte array.
        /// Function Syntax: ConvertFromBase64(base64string:string)
        /// </summary>
        /// <returns>A byte array representation of the given string.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private byte[] ConvertFromBase64()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertFromBase64, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertFromBase64InvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertFromBase64InvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string input = string.Empty;

                try
                {
                    input = parameter as string;
                    byte[] result = string.IsNullOrEmpty(input) ? null : Convert.FromBase64String(input);

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertFromBase64, "ConvertFromBase64('{0}') returned '{1}'.", this.parameters[0], result);

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertFromBase64InvalidFirstFunctionParameterError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_ConvertFromBase64_InvalidFirstFunctionParameterError, this.function, input, e.Message));
                }                
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertFromBase64, EvaluationModeTemplate, this.mode);
            }
        }

    }
}
