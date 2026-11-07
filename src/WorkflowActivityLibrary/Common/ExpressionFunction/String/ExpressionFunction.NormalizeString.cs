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
        /// This function is used to normalize the first string, first by replacing the character substitutions specfied in the second string 
        /// and then removing all diacritics using the .NET string normalization function.
        /// The substitution string is ':' and '|' separated in the form oldString1:newString1|oldString2:newString2...
        /// Function Syntax: NormalizeString(value:string, substitutions:string)
        /// e.g. NormalizeString("ﬀßæÁÂÃÄÅÇÈÉàáâãäåèéêëìíîïòóôõ", "Å:AA|Ä:AE|Ö:OE|å:aa|ä:ae|ö:oe|ß:ss|æ:ae")
        /// </summary>
        /// <returns>true if the first string matches the regular expression pattern specified in the second string. Otherwise false.</returns>
        private string NormalizeString()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionNormalizeString, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCountBetween(1, 2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionNormalizeStringInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, 1, 2, this.parameters.Count));
                                
                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = this.parameters[0] as string;

                if (!string.IsNullOrEmpty(result))
                {
                    // First do substitutions
                    result = ApplySubstitutions(result);

                    // Now normalize
                    result = ApplyNormalization(result);
                }  

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionNormalizeString, "NormalizeString({0}) returned '{1}'.", arguments, result);
                               
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionNormalizeString, EvaluationModeTemplate, this.mode);
            }
        }

        private string ApplyNormalization(string input)
        {
            input = input.Normalize(NormalizationForm.FormKD);

            StringBuilder normalizedString = new StringBuilder();

            foreach (char ch in input)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                normalizedString.Append(ch);

                if (ch > 128)
                {
                    if (this.parameters.Count == 1)
                    {
                        Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionNormalizeString, "NormalizeString('{0}'). Character '{1}' is not a 7-bit ASCII Character.", this.parameters[0], ch);
                    }
                    else
                    {
                        Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionNormalizeString, "NormalizeString('{0}', '{1}'). Character '{2}' is not a 7-bit ASCII Character.", this.parameters[0], this.parameters[1], ch);
                    }
                }
            }

            return normalizedString.ToString();
        }

        private string ApplySubstitutions(string input)
        {
            string substitutions = this.parameters.Count > 1 ? this.parameters[1] as string : string.Empty;

            if (!string.IsNullOrEmpty(substitutions))
            {
                string[] substituionPairs = substitutions.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                // Changed StringSplitOptions for substituionPairs from RemoveEmptyEntries to None so that soft and hard signs such as"Ь" can be simply dropped
                foreach (string[] substitution in substituionPairs.Select(substituionPair => substituionPair.Split(new string[] { ":" }, StringSplitOptions.None)).Where(substitution => substitution.Length == 2))
                {
                    input = input.Replace(substitution[0].Trim(), substitution[1].Trim());
                    input = input.Replace(substitution[0], substitution[1].Trim());
                }
            }

            return input;
        }
    }
}
