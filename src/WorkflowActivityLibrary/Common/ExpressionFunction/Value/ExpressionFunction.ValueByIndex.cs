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
        /// This function is used to retrieve the value at the specified index in the input list.
        /// Function Syntax: ValueByIndex(values:[list or object], index:integer)
        /// </summary>
        /// <returns>The object at the specified index in the input list. If the specified index is greater than the count in the list, null is returned.</returns>
        private object ValueByIndex()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionValueByIndex, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByIndexInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                Type parameterType = typeof(int);
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByIndexNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByIndexInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                int index = Convert.ToInt32(this.parameters[1], CultureInfo.InvariantCulture);
                if (index < 0)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByIndexInvalidSecondFunctionParameterTypeError, new InvalidArgumentException(Messages.ExpressionFunction_NegativeNumberAsParameterNotAllowedError, this.function, parameterType.Name, parameter.GetType().Name));
                
                object result = TryGetValueByIndex(index);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionValueByIndex, "ValueByIndex({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionValueByIndex, EvaluationModeTemplate, this.mode);
            }
        }

        private object TryGetValueByIndex(int index)
        {            
            if (this.parameters[0] == null)
                return null;             

            Type paramType = this.parameters[0].GetType();
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var enumerable = (IEnumerable)this.parameters[0];

                // 1. Convert to a generic LINQ sequence of objects
                var objectSequence = enumerable.Cast<object>();

                // 2. Safely check if the index is out of bounds
                if (index >= objectSequence.Count())
                    return null;

                // 3. Retrieve the item at the specific index
                return objectSequence.ElementAt(index);                
            }

            if (index == 0)
                return this.parameters[0];
            
            return null;
        }
    }
}
