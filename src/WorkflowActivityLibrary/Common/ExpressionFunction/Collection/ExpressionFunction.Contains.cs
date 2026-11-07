using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to determine if the specified list contains the specified value.
        /// Function Syntax: Contains(values:List, value:object)
        /// </summary>
        /// <returns>true if the list contains given value. Otherwise false.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", Justification = "Reviewed and retained InvariantCultureIgnoreCase")]
        private bool Contains()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionContains, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionContainsInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                bool result = false;

                if (this.mode != EvaluationMode.Parse &&
                    this.parameters[0] != null &&
                    this.parameters[1] != null)
                {
                    Type paramType = this.parameters[0].GetType();

                    if (paramType.IsGenericType &&
                        paramType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        result = TestParameters();
                    }
                    else
                    {
                        result = this.Eq();
                    }

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionContains, "Contains('{0}', '{1}') evaluated '{2}'.", this.parameters[0], this.parameters[1], result);
                }                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionContains, EvaluationModeTemplate, this.mode);
            }
        }

        private bool TestParameters()
        {
            bool result = false;

            foreach (object o in (IEnumerable)this.parameters[0])
            {
                if (o is string && this.parameters[1] is string)
                {
                    if (o.ToString().Equals(
                        this.parameters[1].ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = true;
                        break;
                    }
                }
                else if (o.Equals(this.parameters[1]))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
