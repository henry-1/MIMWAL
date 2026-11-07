using Microsoft.ResourceManagement.WebServices;
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
        /// Converts a specified value to an equivalent FIM UniqueIdentifier value.
        /// Function Syntax: ConvertToUniqueIdentifier(guid(s):[list or object])
        /// </summary>
        /// <returns>The UniqueIdentifier representation of the value.</returns>
        private object ConvertToUniqueIdentifier()
        {
            Logger.Instance.WriteMethodEntry(
                EventIdentifier.ExpressionFunctionConvertToUniqueIdentifier,
                EvaluationModeTemplate,
                this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToUniqueIdentifierInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                Type listGuidType = typeof(List<Guid>);
                Type guidType = typeof(Guid);
                Type byteArrayType = typeof(byte[]);
                object parameter = this.parameters[0];

                if (!this.VerifyType(parameter, listGuidType)
                    && !this.VerifyType(parameter, guidType)
                    && !this.VerifyType(parameter, byteArrayType)
                )
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToUniqueIdentifierInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError3, this.function, listGuidType.Name, guidType.Name, byteArrayType.Name, parameter.GetType().Name));
                
                if (this.mode == EvaluationMode.Parse)
                    return null;
                try
                {
                    object result = TryGetGUID(parameter);

                    string arguments = this.FormatArguments();
                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionConvertToUniqueIdentifier, "ConvertToUniqueIdentifier({0}) returned '{1}'.", arguments, result);

                    return result;
                }
                catch(Exception e)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionConvertToUniqueIdentifierInvalidFunctionParameterFormatError,
                       new InvalidFunctionFormatException(Messages.ExpressionFunction_ConvertToUniqueIdentifier_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConvertToUniqueIdentifier, EvaluationModeTemplate, this.mode);
            }
        }

        private static object TryGetGUID(object parameter)
        {       
            if (parameter == null)
                return null;

            Type paramType = parameter.GetType();

            if (paramType.IsGenericType
                && paramType.GetGenericTypeDefinition() == typeof(List<>))
                return (from object o in (IEnumerable)parameter
                          select new UniqueIdentifier((Guid)o)).ToList();               
            
            return parameter is byte[]
                ? new UniqueIdentifier(new Guid((byte[])parameter))
                : new UniqueIdentifier((Guid)parameter);
        }
    }
}
