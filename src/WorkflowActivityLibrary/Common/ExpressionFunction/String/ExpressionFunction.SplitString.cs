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
        /// This function is used to split the string into substring delimited by the specified separator.
        /// Function Syntax: SplitString(input:string, separator:string, [count:integer])
        /// </summary>
        /// <returns>A list whose elements contain the substrings in this string that are delimited by the separator.</returns>
        /// <remarks>
        /// A null first parameter passes VerifyType, so input is null and the result is null.
        /// A null separator gives string.IsNullOrEmpty(separator), so it splits on white space.
        /// A null count converts to 0 through Convert.ToInt32, and Split with count 0 returns an empty array, so the result is an empty list.
        /// </remarks>
        
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private List<string> SplitString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionSplitString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountBetween(1, 3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSplitStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, "1", "3", this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSplitStringInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 1, parameterType.Name, parameter.GetType().Name));

                string input = parameter as string;
                string separator = null;
                int count = int.MaxValue;

                if (this.parameters.Count > 1)
                {
                    parameterType = typeof(string);
                    parameter = this.parameters[1];
                    if (!this.VerifyType(parameter, parameterType))
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSplitStringInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 2, parameterType.Name, parameter.GetType().Name));

                    separator = parameter as string;
                }

                if (this.parameters.Count == 3)
                {
                    parameterType = typeof(int);
                    parameter = this.parameters[2];
                    if (!this.VerifyType(parameter, parameterType))
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSplitStringInvalidThirdFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 3, parameterType.Name, parameter.GetType().Name));

                    count = Convert.ToInt32(parameter, CultureInfo.InvariantCulture);
                }

                if (this.mode == EvaluationMode.Parse)
                    return null;
                                
                List<string> result = GetSplittedString(input, separator, count);
                int itemsCount = result == null ? 0 : result.Count;

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionSplitString, "SplitString('{0}', '{1}', '{2}') returned a list with '{3}' items.", input, separator, count, itemsCount);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionSplitString, EvaluationModeTemplate, this.mode);
            }
        }

        private static List<string> GetSplittedString(string input, string separator, int count)
        {
            List<string> result = null;

            if (!string.IsNullOrEmpty(input))
            {
                if (!string.IsNullOrEmpty(separator))
                {
                    result = input.Split(
                        new string[] { separator },
                        count,
                        StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    result = input.Split(
                        default(string[]),
                        count,
                        StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }

            return result;
        }
    }
}
