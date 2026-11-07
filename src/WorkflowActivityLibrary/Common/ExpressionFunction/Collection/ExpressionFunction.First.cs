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
        /// This function is used to retrieve the first member of the specified List.
        /// Function Syntax: First(values:[list or object])
        /// </summary>
        /// <returns>The first member of the input list. If the input is not a list, returns the input.</returns>
        private object First()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionFirst, EvaluationModeTemplate, this.mode);

            try
            {
                // First(values:[list or object])
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionFirstInvalidFunctionParameterCountError, new InvalidFunctionFormatException(
                        Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                object result = GetFirstValue(this.parameters[0]);

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionFirst, "First() returned '{0}'.", result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionFirst, EvaluationModeTemplate, this.mode);
            }
        }

        private object GetFirstValue(object value)
        {
            if (this.IsGenericList(value))
                return ((IEnumerable)value).Cast<object>().FirstOrDefault();
            
            return value;
        }
    }
}
