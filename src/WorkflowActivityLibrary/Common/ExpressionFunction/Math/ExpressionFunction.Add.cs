using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to add two integer values.
        /// Function Syntax: Add(value:integer, value:integer)
        /// </summary>
        /// <returns>The result of the addition of the two integer values.</returns>
        private long Add()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionAdd, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionAddInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionAddInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));
                
                parameter = this.parameters[1];
                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionAddInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return 0;
                try
                {                
                    long result = checked(
                        Convert.ToInt64(this.parameters[0], CultureInfo.InvariantCulture) + 
                        Convert.ToInt64(this.parameters[1], CultureInfo.InvariantCulture));

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionAdd, "Add({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionAddInvalidFirstFunctionParameterError,
                       new InvalidFunctionFormatException(Messages.ExpressionFunction_Add_InvalidFunctionParameterError, this.function, this.parameters[0], this.parameters[1], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionAdd, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
