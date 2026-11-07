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
        /// This function is used to a create new RemovedValues collection which contains all values removed from the specified list.
        /// This RemovedValues collection will be used by the activity so that it can generate update request parameters accordingly.
        /// Function Syntax: RemoveValues(value(s):[list or object])
        /// </summary>
        /// <returns>A new RemovedValues object which contains all values removed from the input list. If the input is not a list, the value of the input object is removed.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private RemovedValuesCollection RemoveValues()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRemoveValues, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRemoveValuesInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;
                
                RemovedValuesCollection result = GetRemovedValues();               

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRemoveValues, EvaluationModeTemplate, this.mode);
            }
        }

        private RemovedValuesCollection GetRemovedValues()
        {
            RemovedValuesCollection result = new RemovedValuesCollection();            

            // Build a new RemovedValues collection which contains all values to be removed
            // This collection type will be caught by the activity so that it can generate update request parameters accordingly
            // If a List<> was passed to the function, add each value
            // For all other data types, add the value directly
            if (this.parameters[0] != null)
            {
                Type paramType = this.parameters[0].GetType();

                if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    result.AddRange(((IEnumerable)this.parameters[0]).Cast<object>());
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRemoveValues, "RemoveValues() removed {0} values.", result.Count);
                }
                else
                {
                    result.Add(this.parameters[0]);
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRemoveValues, "RemoveValues() removed value '{0}'.", this.parameters[0]);
                }
            }

            return result;
        }
    }
}
