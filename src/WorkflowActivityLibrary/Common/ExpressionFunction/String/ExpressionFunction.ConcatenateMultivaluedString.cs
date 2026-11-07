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
        /// Concatenates  the multivalued string.
        /// Function Syntax: ConcatenateMultivaluedString(values:List of string, delimiter:string)
        /// </summary>
        /// <returns>The concatenated string.</returns>
        private string ConcatenateMultivaluedString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConcatenateMultivaluedString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConcatenateMultivaluedStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type listOfStringType = typeof(List<string>);
                Type stringType = typeof(string);
                object values = this.parameters[0];

                if (!this.VerifyType(values, listOfStringType) && !this.VerifyType(values, stringType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConcatenateMultivaluedStringInvalidFirstFunctionParameterTypeError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError2, this.function, listOfStringType.Name, stringType.Name, values.GetType().Name));

                object delimiter = this.parameters[1];
                if (delimiter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConcatenateMultivaluedStringNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = Concatenate(values, delimiter.ToString());

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConcatenateMultivaluedString, "ConcatenateMultivaluedString() returned '{0}'.", result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConcatenateMultivaluedString, EvaluationModeTemplate, this.mode);
            }
        }

        private static string Concatenate(object values, string delimiter)
        {
            if (values == null)
                return null;

            string single = values as string;
            if (single != null)
                return single;

            List<string> list = (List<string>)values;
            if (list.Count == 0)
                return null;

            return string.Join(delimiter, list.ToArray());
        }
    }
}
