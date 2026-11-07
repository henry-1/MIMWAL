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
        /// This function is used to remove duplicate values from a list of items.
        /// Function Syntax: RemoveDuplicates(input:list of string)
        /// </summary>
        /// <returns>A list without any duplicates.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private List<string> RemoveDuplicates()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRemoveDuplicates, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRemoveDuplicatesInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                Type parameterType = typeof(List<string>);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRemoveDuplicatesInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                List<string> input = (List<string>)parameter;
                List<string> result = Deduplicate(input);

                if (result == null)
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRemoveDuplicates, "RemoveDuplicates('{0}') returned null.", input);
                else
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRemoveDuplicates, "RemoveDuplicates('{0}') returned a list with '{1}' items removed.", input, input.Count - result.Count);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRemoveDuplicates, EvaluationModeTemplate, this.mode);
            }
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private static List<string> Deduplicate(List<string> input)
        {
            if (input == null)
                return null;

            List<string> result = new List<string>(input.Count);
            result.AddRange(input.Distinct(StringComparer.OrdinalIgnoreCase));

            return result;
        }
    }
}
