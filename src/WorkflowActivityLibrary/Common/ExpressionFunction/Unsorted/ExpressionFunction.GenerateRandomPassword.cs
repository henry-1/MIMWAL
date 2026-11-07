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
        /// This function is used to generates a random password of specified length.
        /// Function Syntax: GenerateRandomPassword(length:integer)
        /// </summary>
        /// <returns>A randomly generated string of specified length.</returns>
        private string GenerateRandomPassword()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionGenerateRandomPassword, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionGenerateRandomPasswordInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                if (this.parameters[0] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionGenerateRandomPasswordNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));
                
                Type parameterType = typeof(int);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionGenerateRandomPasswordInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                // Instantiate a new class of the password generator class
                // and return a randomly generated complex password of the specified length
                int length = Convert.ToInt32(this.parameters[0], CultureInfo.InvariantCulture);
                string result = PasswordGenerator.GenerateRandomPassword(length);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionGenerateRandomPassword, "GenerateRandomPassword({0}) returned '{1}'.", arguments, result);
                
                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionGenerateRandomPassword, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
