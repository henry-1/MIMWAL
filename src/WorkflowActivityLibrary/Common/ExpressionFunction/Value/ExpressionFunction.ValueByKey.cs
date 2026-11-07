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
        /// This function is used to retrieve the value at the specified key in the input Dictionary of string/object key/pair.
        /// Function Syntax: ValueByKey(dictionary:[Dictionary], key:string)
        /// </summary>
        /// <returns>The object associated with the specified key. If the specified key does not exist, null is returned.</returns>
        private object ValueByKey()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionValueByKey, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByKeyInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                Type stringParameterType = typeof(string);
                object stringParameter = this.parameters[1];
                if (stringParameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByKeyNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(stringParameter, stringParameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByKeyInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, stringParameterType.Name, stringParameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;
                
                object dictionaryParameter = this.parameters[0];
                object result = GetValue(stringParameter, dictionaryParameter);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionValueByKey, "ValueByKey({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionValueByKey, EvaluationModeTemplate, this.mode);
            }
        }

        private object GetValue(object stringParameter, object dictionaryParameter)
        {
            if (dictionaryParameter == null)
                return null;
                        
            string key = stringParameter as string;

            Type dictionaryParameterType = typeof(Dictionary<string, object>);

            if (dictionaryParameter.GetType() != dictionaryParameterType)
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionValueByKeyInvalidFirstFunctionParameterTypeError, new InvalidArgumentException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, dictionaryParameterType.Name, dictionaryParameter.GetType().Name));
                        
            var dictionary = dictionaryParameter as Dictionary<string, object>;
            if (dictionary != null && dictionary.ContainsKey(key))
                return dictionary[key];            

            return null;
        }
    }
}
