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
        /// This function is used to return a string formed with the trailing characters of the string value specified in the first parameter and the length specified in the second parameter.
        /// Function Syntax: Right(value:string, length:integer)
        /// </summary>
        /// <returns>The trailing substring of specified length. If the specified length is greater than the input string, the entire input string is returned.</returns>
        private string Right()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRight, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                Type parameterType = typeof(int);
                object parameter = this.parameters[1];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                int length;

                if (this.mode == EvaluationMode.Parse)
                {
                    // Validate only if this is a known constant,
                    // not an unresolved ParameterType placeholder.
                    // A negative number provided as Length parameter would later cause an exception                    
                    
                    if (this.TryGetLength(parameter, out length) && length < 0)
                    {
                        throw Logger.Instance.ReportError( EventIdentifier.ExpressionFunctionRightInvalidSecondFunctionParameterValueError, new InvalidArgumentException(
                                Messages.ExpressionFunction_NegativeNumberAsParameterNotAllowedError, this.function));
                    }

                    return null;
                }

                if (!this.TryGetLength(parameter, out length))
                {
                    // Report an unexpected unresolved parameter.
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightInvalidSecondFunctionParameterTypeError, new InvalidArgumentException(Messages.ExpressionFunction_Right_UnableToResolveSecondParameterError, this.function, parameterType.Name, parameter.GetType().Name));
                }

                if (length < 0)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRightInvalidSecondFunctionParameterValueError, new InvalidArgumentException(Messages.ExpressionFunction_NegativeNumberAsParameterNotAllowedError, this.function));
                
                string stringParameter = this.parameters[0] == null ? null : this.parameters[0].ToString();                

                string result = GetRightPart(stringParameter, length);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRight, "Right({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRight, EvaluationModeTemplate, this.mode);
            }
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S2325", Justification = "The method is part of an object not an abstract class.")]
        private bool TryGetLength(object parameter, out int length)
        {
            length = 0;

            if (parameter is ParameterType)
                return false;

            length = Convert.ToInt32(
                parameter,
                CultureInfo.InvariantCulture);

            return true;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S2325", Justification = "The method is part of an object not an abstract class.")]
        private string GetRightPart(string stringParameter, int length)
        {            
            if (stringParameter == null)
                return null;

            if (stringParameter.Length <= length)
                return stringParameter;
            
            return stringParameter.Substring(stringParameter.Length - length, length);            
        }
    }
}
