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
        /// This function is used to determine the type of the supplied input object.
        /// Function Syntax: ValueType(value:object)
        /// </summary>
        /// <returns>The System.Type of the input object.</returns>
        private string ValueType()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionValueType, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueTypeInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                object parameter = this.parameters[0];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueTypeNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = this.parameters[0].GetType().ToString();

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionValueType, "ValueType('{0}') returned '{1}'.", parameter, result);                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionValueType, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
