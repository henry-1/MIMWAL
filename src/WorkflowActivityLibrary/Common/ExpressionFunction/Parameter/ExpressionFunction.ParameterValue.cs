using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to read the value of a request parameter of an arbitrary request.
        /// When the request is for an update of a multi-valued attribute, ParameterValueAdded() and ParameterValueRemoved() must be used.
        /// Function Syntax: ParameterValue(RequestParameter:List<RequestParameter>, attribute:string)
        /// </summary>
        /// <returns>
        /// The value of the attribute in the request parameters.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed.")]
        private object ParameterValue()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionParameterValue, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParameterValueInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                                
                Type parameterType = typeof(List<string>);
                Type parameterType2 = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParameterValueInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError2, this.function, parameterType.Name, parameterType2.Name, parameter.GetType().Name));

                if (this.parameters[1] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParameterValueNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));
                
                if (this.mode == EvaluationMode.Parse)
                    return null;
                                
                List<string> requestParameters = GetRequestParametersAsList();
                object  result = CollectParameterValues(requestParameters);

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionParameterValue, "ParameterValue() for attribute '{0}' returned '{1}'.", this.parameters[1], result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionParameterValue, EvaluationModeTemplate, this.mode);
            }
        }

        private object CollectParameterValues(List<string> requestParameters)
        {
            if (requestParameters == null)
                return null;

            List<object> valueList = new List<object>();
            
            foreach (string s in requestParameters)
            {
                RequestParameter requestParameter = DeserializeObject<RequestParameter>(s);

                object value;
                if (TryGetRequestParameterValue(requestParameter, out value))
                    valueList.Add(value);
            }            

            if (valueList.Count == 1)
                return valueList[0];
            
            if (valueList.Count != 0)
                return valueList;            

            return null;
        }

        private bool TryGetRequestParameterValue( RequestParameter requestParameter, out object value)
        {
            value = null;

            // Check UpdateRequestParameter first because it inherits from CreateRequestParameter.
            if (requestParameter is UpdateRequestParameter)
            {
                UpdateRequestParameter updateRequestParameter = (UpdateRequestParameter)requestParameter;

                if (!updateRequestParameter.PropertyName.Equals(this.parameters[1]))
                    return false;

                if (updateRequestParameter.Mode == UpdateMode.Modify)
                {
                    value = updateRequestParameter.Value;
                    return value != null;
                }

                Logger.Instance.WriteError(
                    EventIdentifier.ExpressionFunctionParameterValueInvalidFunctionUseError,
                    Messages.ExpressionFunction_InvalidParameterValueFunctionUseError,
                    this.parameters[1]);

                return false;
            }

            if (requestParameter is CreateRequestParameter)
            {
                CreateRequestParameter createRequestParameter = (CreateRequestParameter)requestParameter;

                if (createRequestParameter.PropertyName.Equals(this.parameters[1]))
                {
                    value = createRequestParameter.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
