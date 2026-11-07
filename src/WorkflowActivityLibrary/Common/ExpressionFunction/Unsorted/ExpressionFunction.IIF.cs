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
        /// This Inline if function is used to return the second or third parameter based on the evaluation of the first parameter.
        /// Function Syntax: IIF(condition:boolean, trueReturn:object, falseReturn:object)
        /// </summary>
        /// <returns>The second input parameter if the first input condition is true. Otherwise the third input parameter.</returns>
        private object IIF()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionIIF, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionIIFInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));
                
                Type parameterType = typeof(bool);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionIIFInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter == null ? "null" : parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                object result = this.parameters[1];

                if (this.parameters[0] == null || !(bool)this.parameters[0])
                    result = this.parameters[2];
                
                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionIIF, "IIF({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionIIF, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
