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
        /// This function is used to convert a string or a number to a boolean value.
        /// Function Syntax: ConvertToBoolean(value:string) or ConvertToBoolean(value:integer)
        /// </summary>
        /// <returns>true or false, which reflects the value returned by invoking the IConvertible.ToBoolean method for the underlying type of value. If value is null, the method returns false.</returns>
        /// <remarks>
        /// I introduced a type check for the first parameter.
        /// Now it only accepts string, bool, long or int.
        /// The former version failed on Convert.ToBoolean(object, IFormatProvider), now we have a managed error during the test earlier int the code.
        /// </remarks>
        private bool ConvertToBoolean()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConvertToBoolean, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToBooleanInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                Type stringType = typeof(string);
                Type boolType = typeof(bool);
                Type longType = typeof(long);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, stringType) && !this.VerifyType(parameter, boolType) && !this.VerifyType(parameter, longType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToBooleanInvalidFirstFunctionParameterTypeError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError3, this.function, stringType.Name, boolType.Name, longType.Name, parameter.GetType().Name));


                if (this.mode == EvaluationMode.Parse)
                    return false;

                try
                {
                    bool result = Convert.ToBoolean(this.parameters[0], CultureInfo.InvariantCulture);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertToBoolean, "ConvertToBoolean({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch (Exception e)
                {
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionConvertToBooleanInvalidFirstFunctionParameterFormatError,
                            new InvalidFunctionFormatException(Messages.ExpressionFunction_ConvertToBoolean_InvalidFirstFunctionParameterFormatError, this.function, parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertToBoolean, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
