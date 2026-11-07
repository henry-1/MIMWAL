using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    
    public partial class ExpressionFunction
    {

        private string FormatArguments()
        {
            string[] formatted = new string[this.parameters.Count];
            for (int i = 0; i < formatted.Length; i++)
                formatted[i] = string.Format(CultureInfo.InvariantCulture, "'{0}'", this.parameters[i]);

            return string.Join(", ", formatted);
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private char[] GetTrimChars()
        {
            return this.parameters.Count == 2 && this.parameters[1] != null
                ? this.parameters[1].ToString().ToCharArray()
                : null;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S2325", Justification = "Checked.")]
        private bool IsGenericList(object value)
        {
            if (value == null)
                return false;

            Type type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }


        private List<string> GetRequestParametersAsList()
        {
            if (this.parameters[0] is string)
                return new List<string> { this.parameters[0].ToString() };

            return (List<string>)this.parameters[0];
        }

        
        private bool HasParameterCount(int expectedCount)
        {
            IList param = this.parameters;
            return param != null &&
                   param.Count == expectedCount;
        }

        private bool HasParameterCountBetween(int min, int max)
        {
            return this.parameters.Count >= min && this.parameters.Count <= max;
        }

        private bool HasParameterCountMinimum(int min)
        {
            return this.parameters.Count >= min;
        }


        /// <summary>
        /// Verifies value is of the specified type.
        /// If the value is of the specified type,
        /// or if a validation is being performed and the type is Function ParameterType
        /// to indicate that it is a lookup pending resolution, the type is valid
        /// We also allow the value to be null to accommodate null attribute transfer
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>true if the value is of the specified type. Otherwise false.</returns>
        private bool VerifyType(object value, Type type)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionVerifyType, "Value: '{0}'. Type: '{1}'. Evaluation Mode: '{2}'.", value, type, this.mode);
            bool verified = false;
            try
            {
                verified = value == null ||
                    value.GetType() == type ||
                    (type == typeof(int) && value is long) ||
                    (type == typeof(long) && value is int) ||
                    (this.mode == EvaluationMode.Parse && value is ParameterType);

                return verified;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionVerifyType, "Value: '{0}'. Type: '{1}'. Verified: '{2}'. Evaluation Mode: '{3}'.", value, type, verified, this.mode);
            }
        }

    }
}
