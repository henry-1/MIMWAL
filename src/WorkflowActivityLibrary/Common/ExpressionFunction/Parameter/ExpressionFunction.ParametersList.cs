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
        /// This function is used to return the list of attribute names in the request parameters of an arbitrary request.
        /// Function Syntax: ParametersList(RequestParameter:List<RequestParameter>)
        /// </summary>
        /// <returns>
        /// List of the attribute names in the request parameters.
        /// </returns>
        private List<string> ParametersList()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionParametersList, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersListInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));
                
                Type parameterType = typeof(List<string>);
                Type parameterType2 = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersListInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError2, this.function, parameterType.Name, parameterType2.Name, parameter.GetType().Name));
                
                if (this.mode == EvaluationMode.Parse)
                    return new List<string>();
                                
                var requestParameters = GetRequestParametersAsList(); 
                var result = GetParameters(requestParameters);                

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionParametersList, "ParametersList function returned '{0}'.", string.Join(";", result.ToArray()));                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionParametersList, EvaluationModeTemplate, this.mode);
            }
        }

        private static List<string> GetParameters(List<string> requestParameters)
        {
            List<string> result = new List<string>();

            if (requestParameters == null)
                return result;

            foreach (string s in requestParameters)
            {
                RequestParameter requestParameter = DeserializeObject<RequestParameter>(s);

                // UpdateRequestParameter inherits from CreateRequestParameter, so one check covers both
                CreateRequestParameter createParameter = requestParameter as CreateRequestParameter;
                string attributeName = createParameter == null ? null : createParameter.PropertyName;

                if (!string.IsNullOrEmpty(attributeName))
                {
                    result.Add(attributeName);
                }                
            }

            return result;
        }
    }
}
