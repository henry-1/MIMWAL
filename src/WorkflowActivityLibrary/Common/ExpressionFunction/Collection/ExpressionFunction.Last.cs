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
        /// This function is used to retrieve the last member of the specified List.
        /// Function Syntax: Last(values:[list or object])
        /// </summary>
        /// <returns>The last member of the input list. If the input is not a list, returns the input.</returns>
        private object Last()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionLast, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionLastInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                object result = GetLastValue(this.parameters[0]);

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionLast, "Last() returned '{0}'.", result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionLast, EvaluationModeTemplate, this.mode);
            }
        }

        private object GetLastValue(object value)
        {
            if (this.IsGenericList(value))
                return ((IEnumerable)value).Cast<object>().LastOrDefault();               
            
             return value;
        }
    }
}
