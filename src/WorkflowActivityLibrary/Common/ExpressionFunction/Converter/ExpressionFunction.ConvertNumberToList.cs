using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to convert a a number to a list of numbers which can be used to define an Iteration.
        /// Function Syntax: ConvertNumberToList(count:integer)
        /// </summary>
        /// <returns>A list of numbers starting from 1 to the specified input number.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private List<int> ConvertNumberToList()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertNumberToList, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertNumberToListInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(int);
                object parameter = this.parameters[0];

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertNumberToListInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                List<int> result = null;
                
                int count = Convert.ToInt32(parameter, CultureInfo.InvariantCulture);
                if (count > 0)
                {
                    result = Enumerable.Range(1, count).ToList();
                }

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertNumberToList, "ConvertNumberToList('{0}') returned '{1}'.", this.parameters[0], result);                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertNumberToList, EvaluationModeTemplate, this.mode);
            }
        }

    }
}
