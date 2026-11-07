using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to sort the specified List.
        /// Function Syntax: SortList(values:[list or object])
        /// </summary>
        /// <returns>The sorted the input list. If the input is not a list, returns the input.</returns>
        private object SortList()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionSortList, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionSortListInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                object result = null;

                if (this.mode == EvaluationMode.Parse)
                    return result;
                                
                if (this.parameters[0] != null)
                    result = TryGetSortedList();

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionSortList, "SortList() returned '{0}'.", result);

                return result;               
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionSortList, EvaluationModeTemplate, this.mode);
            }
        }

        private object TryGetSortedList()
        {
            Type paramType = this.parameters[0].GetType();
            if (!paramType.IsGenericType || (paramType.GetGenericTypeDefinition() != typeof(List<>)))
                return this.parameters[0];
                        
            List<object> list = ((IEnumerable)this.parameters[0])
                .Cast<object>()
                .ToList();

            list.Sort();

            return list;
        }
    }
}
