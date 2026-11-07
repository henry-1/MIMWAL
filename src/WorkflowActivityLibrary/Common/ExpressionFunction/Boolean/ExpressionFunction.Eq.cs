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
        /// This function is used to determine the equivalence between two objects or values, returns true if they are equal.
        /// Function Syntax: Eq(value:object, value:object, boolean:CompareCase)
        /// </summary>
        /// <remarks>
        /// Providing null as third parameter is StringComparison.InvariantCultureIgnoreCase
        /// </remarks>
        /// <returns>true if the two objects or values are equal. Otherwise false.</returns>
        private bool Eq()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionEqual, EvaluationModeTemplate, this.mode);

            try
            {
                if(!this.HasParameterCount(2) && !this.HasParameterCount(3))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionEqualInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError2, this.function, 2, 3, this.parameters.Count));
                
                if (this.HasParameterCount(3))
                {
                    Type parameterType = typeof(bool);
                    object parameter = this.parameters[2];
                    if (!this.VerifyType(parameter, parameterType))
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionEqualInvalidThirdFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidThirdFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                }

                if (this.mode == EvaluationMode.Parse)
                    return false;

                bool result = false;
                object left, right;                    

                CompareParameters(out result, out left, out right);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionEqual, "Eq({0}) evaluated '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionEqual, EvaluationModeTemplate, this.mode);
            }
        }

        private void CompareParameters(out bool result, out object left, out object right)
        {

            left = this.parameters[0];
            right = this.parameters[1];

            if (left == null || right == null)
            {
                result = CompareWithNull(left, right);
            }
            else if (this.VerifyType(left, typeof(string)) &&
                this.VerifyType(right, typeof(string)))
            {
                result = CompareWithString(left, right);
            }
            else if (this.VerifyType(left, typeof(long)) && this.VerifyType(right, typeof(long)))
            {
                result = Convert.ToInt64(left, CultureInfo.InvariantCulture) == Convert.ToInt64(right, CultureInfo.InvariantCulture);
            }
            else
            {
                result = left.Equals(right);
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", Justification = "Reviewed and retained InvariantCultureIgnoreCase")]
        private bool CompareWithString(object left, object right)
        {
            bool ignoreCase = this.HasParameterCount(2) ||
                                this.parameters[2] == null ||
                                !(bool)this.parameters[2];

            return ignoreCase ? left.ToString().Equals(
                    right.ToString(), StringComparison.InvariantCultureIgnoreCase) :
                    left.ToString().Equals(right.ToString());
        }

        private bool CompareWithNull(object left, object right)
        {
            bool result = false;
            if (left == null && right == null)
            {
                result = true;
            }
            else if (left == null && this.VerifyType(right, typeof(string)))
            {
                result = string.IsNullOrEmpty(right as string);
            }
            else if (right == null && this.VerifyType(left, typeof(string)))
            {
                result = string.IsNullOrEmpty(left as string);
            }

            return result;
        }
    }
}
