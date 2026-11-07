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
        /// This function is used to convert the first character of each space-delimited word in a string to upper case and all other characters are converted to lower case.
        /// Function Syntax: ProperCase(value:string)
        /// </summary>
        /// <returns>Returns a copy of the input string with the first character of each space-delimited word in a string in upper case and all other characters in lower case.</returns>
        private string ProperCase()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionProperCase, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionProperCaseInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = GetProperCaseString();

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionProperCase, "ProperCase('{0}') returned '{1}'.", this.parameters[0], result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionProperCase, EvaluationModeTemplate, this.mode);
            }
        }

        private string GetProperCaseString()
        {
            if (string.IsNullOrEmpty(this.parameters[0] as string))
                return this.parameters[0] as string;                    // always 'null' in this case
            
            // FIM function does not behave as ToTitleCase
            ////result = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(this.parameters[0].ToString());

            // FIM implementation 
            string input = this.parameters[0].ToString();
            bool lastCharWhiteSpace = true;
            bool lastCharUpperCase = false;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    if (!char.IsLetter(input[i]) || (!lastCharWhiteSpace && lastCharUpperCase))
                    {
                        stringBuilder.Append(char.ToLower(input[i], CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        stringBuilder.Append(char.ToUpper(input[i], CultureInfo.CurrentCulture));
                        lastCharUpperCase = true;
                    }

                    lastCharWhiteSpace = false;
                }
                else
                {
                    stringBuilder.Append(input[i]);
                    lastCharWhiteSpace = true;
                    lastCharUpperCase = false;
                }
            }

            return stringBuilder.ToString();
        }
    }
}
