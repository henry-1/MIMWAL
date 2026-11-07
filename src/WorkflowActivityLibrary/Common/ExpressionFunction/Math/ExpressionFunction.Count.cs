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
        /// This function is used to determine the number of elements in a specified list.
        /// Function Syntax: Count(values:[list or object])
        /// </summary>
        /// <returns>The number of elements in the input list. If the input is not a list, returns 1.</returns>
        private int Count()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionCount, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCountInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return 0;

                int result = CountItems(this.parameters[0]);

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCount, "Count('{0}') returned '{1}'.", this.parameters[0], result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCount, EvaluationModeTemplate, this.mode);
            }
        }

        private int CountItems(object value)
        {
            if (value == null)
                return 0;

            return this.IsGenericList(value) ? ((IEnumerable)value).Cast<object>().Count() : 1;
        }
    }
}
