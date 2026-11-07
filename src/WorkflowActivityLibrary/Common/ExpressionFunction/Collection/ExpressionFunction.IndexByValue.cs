using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to retrieve the index for a specific value within the input list.
        /// Function Syntax: IndexByValue(values:[list or object], value:object)
        /// </summary>
        /// <returns>The index of the specified value in the input list. If the value is null or not found in the list, -1 is returned.</returns>
        private int IndexByValue()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionIndexByValue, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionIndexByValueInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                // default value if no results are found
                int result = -1;

                if (this.mode == EvaluationMode.Parse)
                    return result;

                if (this.parameters[0] != null && this.parameters[1] != null)
                {
                    Type paramType = this.parameters[0].GetType();

                    if (paramType.IsGenericType &&
                        paramType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Logger.Instance.WriteVerbose(
                            EventIdentifier.ExpressionFunctionIndexByValue,
                            "IndexByValue('{0}', '{1}') First parameter is a list. Enumerating list items.", this.parameters[0], this.parameters[1]);

                        result = this.TryGetValue();
                    }
                }

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionIndexByValue,"IndexByValue({0}) returned '{1}'.",arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionIndexByValue, EvaluationModeTemplate, this.mode);
            }
        }

        private int TryGetValue()
        {
            int index = 0;
            foreach (object item in (IEnumerable)this.parameters[0])
            {
                if (item is string && this.parameters[1] is string)
                {
                    if (item.ToString().Equals(this.parameters[1].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionIndexByValue, "IndexByValue('{0}', '{1}') Matched string item '{2}' to second param '{3}'. Result: {4}.", this.parameters[0], this.parameters[1], item.ToString(), this.parameters[1].ToString(), index);
                        return index;
                    }
                }
                else if (object.Equals(item, this.parameters[1])) // list item can be null
                {
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionIndexByValue, "IndexByValue('{0}', '{1}') Matched object item '{2}' to second param '{3}'. Result: {4}.", this.parameters[0], this.parameters[1], item.ToString(), this.parameters[1].ToString(), index);
                    return index;
                }

                index += 1;
            }

            return -1;
        }
    }
}
