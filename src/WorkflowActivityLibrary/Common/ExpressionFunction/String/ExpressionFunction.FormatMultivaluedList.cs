using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// Formats the multivalued list as per the specified format.
        /// Function Syntax: FormatMultivaluedList(format:string, value1:list or object [, value2:list or object, ...] )
        /// If more than one value lists are supplied as parameters, they will be treated as if they are of the length equal to that of the largest list with `null` as additional items. 
        /// If one of the parameters supplied is a single / non-list object, e.g. a string, that parameter will be treated as if it is a list of length equal to that of the largest list filled with each item being the same object.
        /// </summary>
        /// <returns>The formatted list.</returns>
        private object FormatMultivaluedList()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionFormatMultivaluedList, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountMinimum(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionFormatMultivaluedListInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinCountError, this.function, 2, this.parameters.Count));

                object format = this.parameters[0];
                if (format == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionFormatMultivaluedListNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                int maxListItemCount = this.GetMaxListItemCount();
                try
                {
                    object result = maxListItemCount == 0
                        ? this.FormatSingleValue()
                        : this.FormatMultipleValues(maxListItemCount);

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConcatenateMultivaluedString, "FormatMultivaluedList() returned '{0}'.", result);

                    return result;
                }
                catch(Exception ex)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionFormatMultivaluedListInvalidParameterFormatError, new InvalidFunctionFormatException(Messages.ExpressionFunction_FormatMultivaluedList_InvalidParameterFormatError, this.function, ex));
                }                
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionFormatMultivaluedList, EvaluationModeTemplate, this.mode);
            }
        }

        private object FormatMultipleValues(int count)
        {
            List<string> result = new List<string>(count);
            object[] arguments = new object[this.parameters.Count - 1];

            for (int n = 0; n < count; n++)
            {
                for (int i = 1; i < this.parameters.Count; i++)
                {
                    object parameter = this.parameters[i];
                    arguments[i - 1] = parameter;

                    if (parameter == null)
                        continue;

                    Type parameterType = parameter.GetType();

                    if (parameterType.IsGenericType &&
                        parameterType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        List<object> list = ((IEnumerable)parameter)
                            .Cast<object>()
                            .ToList();

                        arguments[i - 1] = list.Count > n
                            ? list[n]
                            : null;
                    }
                }

                result.Add(string.Format(
                    CultureInfo.InvariantCulture,
                    this.parameters[0].ToString(),
                    arguments));
            }

            return result;
        }

        private object FormatSingleValue()
        {
            object[] arguments = new object[this.parameters.Count - 1];

            for (int i = 1; i < this.parameters.Count; i++)
            {
                object parameter = this.parameters[i];

                if (parameter != null)
                {
                    Type parameterType = parameter.GetType();

                    if (parameterType.IsGenericType &&
                        parameterType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        parameter = null;
                    }
                }

                arguments[i - 1] = parameter;
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                this.parameters[0].ToString(),
                arguments);
        }

        private int GetMaxListItemCount()
        {
            int maxListItemCount = 0;

            for (int i = 1; i < this.parameters.Count; i++)
            {
                object parameter = this.parameters[i];

                if (parameter == null)
                    continue;

                Type parameterType = parameter.GetType();

                if (parameterType.IsGenericType &&
                    parameterType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    int count = ((IEnumerable)parameter)
                        .Cast<object>()
                        .Count();

                    if (count > maxListItemCount)
                        maxListItemCount = count;
                }
            }

            return maxListItemCount;
        }

    }
}
