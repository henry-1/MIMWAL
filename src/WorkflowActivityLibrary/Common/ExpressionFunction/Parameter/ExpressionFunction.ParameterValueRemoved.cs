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
        /// This function is used to read the removed values of a multi-valued request parameter of an arbitrary request.
        /// Function Syntax: ParameterValueRemoved(RequestParameter:List<RequestParameter>, attribute:string)
        /// </summary>
        /// <returns>
        /// The removed value of the multi-valued attribute in the request parameters.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed.")]
        private object ParameterValueRemoved()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionParameterValueRemoved, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParameterValueRemovedInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                if (this.parameters[1] == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParameterValueRemovedNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                Type parameterType = typeof(List<string>);
                Type parameterType2 = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParameterValueRemovedInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError2, this.function, parameterType.Name, parameterType2.Name, parameter.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return null;

                object result = GetValueRemoved();                

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionParameterValueRemoved, "ParameterValueRemoved() for attribute '{0}' returned '{1}'.", this.parameters[1], result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionParameterValueRemoved, EvaluationModeTemplate, this.mode);
            }
        }

        private object GetValueRemoved()
        {
            object result = null;
            List<string> requestParameters = new List<string>();

            // return early to avoid NullReferenceException later on requestParameters.Count
            if (this.parameters[0] == null)
                return result;

            if (this.parameters[0] is string)
            {
                requestParameters.Add(this.parameters[0].ToString());
            }
            else
            {
                requestParameters = (List<string>)this.parameters[0];
            }

            List<object> valueList = GetRemovedValueListFromRequestParameter(requestParameters);

            if (valueList.Count == 1)
            {
                result = valueList[0];
            }
            else if (valueList.Count != 0)
            {
                result = valueList;
            }

            return result;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Approved.")]
        private List<object> GetRemovedValueListFromRequestParameter(List<string> requestParameters)
        {
            List<object> valueList = new List<object>();
            if (requestParameters.Count != 0)
            {
                foreach (string s in requestParameters)
                {
                    RequestParameter requestParameter = DeserializeObject<RequestParameter>(s);

                    // check for UpdateRequestParameter before CreateRequestParameter as UpdateRequestParameter is CreateRequestParameter
                    // though it should not really matter for our purpose here
                    if (requestParameter is UpdateRequestParameter)
                    {
                        UpdateRequestParameter updateRequestParameter = (UpdateRequestParameter)requestParameter;
                        if (updateRequestParameter.PropertyName.Equals(this.parameters[1]))
                        {
                            if (updateRequestParameter.Mode == UpdateMode.Remove)
                            {
                                if (updateRequestParameter.Value != null)
                                {
                                    valueList.Add(updateRequestParameter.Value);
                                }
                            }
                            else if (updateRequestParameter.Mode == UpdateMode.Modify)
                            {
                                Logger.Instance.WriteError(EventIdentifier.ExpressionFunctionParameterValueRemovedInvalidFunctionUseError, Messages.ExpressionFunction_InvalidParameterValueRemovedFunctionUseError, this.parameters[1]);
                            }
                        }
                    }
                }
            }

            return valueList;
        }
    }
}
