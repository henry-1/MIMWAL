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
        /// This function is used to concatenate two or more strings.
        /// Function Syntax: Concatenate(string1:string, string2:string, ....)
        /// </summary>
        /// <returns>All input string parameters are concatenated with each other.</returns>
        private string Concatenate()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConcatenate, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountMinimum(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConcatenateInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinCountError, this.function, "2", this.parameters.Count));
                
                Type parameterType = typeof(string);
                for (int i = 0; i < this.parameters.Count; ++i)
                {
                    object parameter = this.parameters[i];
                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConcatenateInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i, parameterType.Name, parameter.GetType().Name));
                    }
                }
                if (this.mode == EvaluationMode.Parse)
                    return null;

                StringBuilder concatenated = new StringBuilder();

                foreach (object parameter in this.parameters)
                {
                    concatenated.Append(Convert.ToString(parameter, CultureInfo.InvariantCulture));
                }

                string result = concatenated.ToString();

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConcatenate, "Concatenate('{0}', '{1}', ...) returned '{2}'.", this.parameters[0], this.parameters[1], result);              

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConcatenate, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
