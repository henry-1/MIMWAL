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
        /// This function is used to escape EXACT one DN component of a distinguished name specified in LDAP format.
        /// Function Syntax: EscapeDNComponent(dnComponent:string)
        /// </summary>
        /// <returns>The escaped DN component.</returns>
        /// <remarks>
        /// This method does not validate the complete LDAP DN syntax.
        /// Example:
        /// Split at the first '=' only. Any additional '=' characters in the value
        /// are escaped by the logic below, so input such as "CN=max=muster"
        /// is converted to "CN=max\=muster".
        /// But 'cn=max=muster,ou=IT' would be falsely escaped to 'cn=max\=muster\,ou\=IT'.
        /// </remarks>
        private string EscapeDNComponent()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionEscapeDNComponent, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionEscapeDNComponentInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter = this.parameters[0];
                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionEscapeDNComponentNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionEscapeDNComponentInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string result = GetDNComponents(parameter);

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionEscapeDNComponent, "EscapeDNComponent('{0}') returned '{1}'.", this.parameters[0], result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionEscapeDNComponent, EvaluationModeTemplate, this.mode);
            }
        }

        private string GetDNComponents(object parameter)
        {
            string[] parts = ((string)parameter).Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionEscapeDNComponentInvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError, this.function, 1, "X=y", parameter));
            
            string dnComponent = parts[0].Trim() + "=" + parts[1].Trim();

            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;

            foreach (char chr in dnComponent.Trim())
            {
                switch (chr)
                {
                    case '=':
                        if (flag)
                        {
                            stringBuilder.Append(chr);
                            flag = false;
                        }
                        else
                        {
                            stringBuilder.Append("\\");
                            stringBuilder.Append(chr);
                        }

                        break;
                    case '"':
                    case '\\':
                    case ';':
                    case '#':
                    case '>':
                    case '<':
                    case '+':
                    case ',':
                        stringBuilder.Append("\\");
                        stringBuilder.Append(chr);
                        break;
                    default:
                        stringBuilder.Append(chr);
                        break;
                }
            }

            return stringBuilder.ToString();
        }
    }
}
