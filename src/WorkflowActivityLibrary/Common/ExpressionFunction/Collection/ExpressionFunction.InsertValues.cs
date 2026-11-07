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
        /// This function is used to create a new InsertedValues collection which contains all values from the specified list.
        /// This InsertedValues collection will be used by the activity so that it can generate update request parameters accordingly.
        /// Function Syntax: InsertValues(value(s):[list or object])
        /// </summary>
        /// <returns>A new InsertedValues object which contains all values from the input list. If the input is not a list, the value of the input object is added.</returns>
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private InsertedValuesCollection InsertValues()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionInsertValues, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionInsertValuesInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                InsertedValuesCollection result = new InsertedValuesCollection();

                if (this.parameters[0] != null)
                {
                    // Build a new InsertedValues collection which contains all values to be added
                    // This collection type will be caught by the activity so that it can generate update request parameters accordingly
                    // If a List<> was passed to the function, add each value
                    // For all other data types, add the value directly
                    Type paramType = this.parameters[0].GetType();
                    if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        result.AddRange(((IEnumerable)this.parameters[0]).Cast<object>());

                        Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionInsertValues, "InsertValues() inserted {0} values.", result.Count);
                    }
                    else
                    {
                        result.Add(this.parameters[0]);

                        Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionInsertValues, "InsertValues() inserted value '{0}'.", this.parameters[0]);
                    }
                }                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionInsertValues, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
