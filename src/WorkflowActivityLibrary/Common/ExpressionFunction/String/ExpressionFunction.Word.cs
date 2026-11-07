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
        /// This function is used to return a word contained within a string, based on parameters describing the delimiters to use and the word number to return.
        /// Function Syntax: Word(value:string, index:integer, delimiter:char)
        /// </summary>
        /// <returns>A string containing the word at the specified position. If string contains less than number of words, or string does not contain any words identified by delimiters, a null string is returned.</returns>
        private string Word()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionWord, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWordInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 3, this.parameters.Count));
                
                Type parameterType = typeof(int);
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWordNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWordInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[2];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWordNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));

                parameterType = typeof(char);
                if (parameter.ToString().ToCharArray().Length != 1)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWordInvalidThirdFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidThirdFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                int index = Convert.ToInt32(this.parameters[1], CultureInfo.InvariantCulture);
                if (index < 0)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionWordInvalidSecondFunctionParameterValueError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NegativeNumberAsParameterNotAllowedError, this.function));
                
                string result = GetWord(index);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionWord, "Word({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionWord, EvaluationModeTemplate, this.mode);
            }
        }

        private string GetWord(int index)
        {
            if (this.parameters[0] == null)            
                return null;
            
            string[] split = this.parameters[0].ToString().Split(this.parameters[2].ToString()[0]);
            
            return index < split.Length ? split[index] : null;           
        }
    }
}
