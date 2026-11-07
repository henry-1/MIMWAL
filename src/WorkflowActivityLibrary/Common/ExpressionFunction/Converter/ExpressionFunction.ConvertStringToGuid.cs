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
        /// This function is used to convert the string representation of a GUID to a binary representation of the GUID.
        /// Function Syntax: ConvertStringToGuid(value:string)
        /// </summary>
        /// <returns>The string GUID converted to its binary representation.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private byte[] ConvertStringToGuid()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertStringToGuid, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertStringToGuidInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertStringToGuidInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                try
                {
                    var input = parameter as string;
                    byte[] result = string.IsNullOrEmpty(input) ? Guid.Empty.ToByteArray() : (new Guid(input)).ToByteArray();

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertStringToGuid, "ConvertStringToGuid('{0}') returned '{1}'.", this.parameters[0], result);

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertStringToGuidInvalidFirstFunctionParameterFormatError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_ConvertStringToGuid_InvalidFirstFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertStringToGuid, EvaluationModeTemplate, this.mode);
            }
        }

    }
}
