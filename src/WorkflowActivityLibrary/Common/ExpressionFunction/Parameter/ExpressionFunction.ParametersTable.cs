using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Web;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to return a table of of attribute name, modification type and modified values in the request parameters of an arbitrary request.
        /// Function Syntax: ParametersTable(RequestParameter:[string])
        /// </summary>
        /// <returns>
        /// A table of of attribute name, modification type and modified values in the request parameters.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed. Required.")]
        private string ParametersTable()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionParametersTable, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(1))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersTableInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 1, this.parameters.Count));

                Type parameterType = typeof(List<string>);
                Type parameterType2 = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersTableInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError2, this.function, parameterType.Name, parameterType2.Name, parameter.GetType().Name));
                
                if (this.mode == EvaluationMode.Parse)
                    return string.Empty;

                string result = GetParametersTable();

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionParametersTable, "ParametersTable() function returned '{0}'.", result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionParametersTable, EvaluationModeTemplate, this.mode);
            }
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Approved.")]
        private string GetParametersTable()
        {
            StringBuilder result = new StringBuilder();

            if (this.parameters[0] == null)
                return result.ToString();

            List<string> requestParameters = new List<string>();

            if (this.parameters[0] is string)
            {
                requestParameters.Add(this.parameters[0].ToString());
            }
            else
            {
                requestParameters = (List<string>)this.parameters[0];
            }

            if (requestParameters.Count != 0)
            {
                foreach (string s in requestParameters)
                {
                    RequestParameter requestParameter = DeserializeObject<RequestParameter>(s);

                    // check for UpdateRequestParameter before CreateRequestParameter as UpdateRequestParameter is CreateRequestParameter
                    if (requestParameter is UpdateRequestParameter)
                    {
                        UpdateRequestParameter updateRequestParameter = (UpdateRequestParameter)requestParameter;
                        string parameterValue = updateRequestParameter.Value == null ? null : HttpUtility.HtmlEncode(updateRequestParameter.Value.ToString());
                        result.AppendFormat("<tr><td class=\"AttributeStyle\">{0}</td><td class=\"AttributeModificationStyle\">{1}</td><td class=\"NewValueStyle\">{2}</td></tr>", updateRequestParameter.PropertyName, updateRequestParameter.Mode, parameterValue);
                    }
                    else if (requestParameter is CreateRequestParameter)
                    {
                        CreateRequestParameter createRequestParameter = (CreateRequestParameter)requestParameter;
                        string parameterValue = createRequestParameter.Value == null ? null : HttpUtility.HtmlEncode(createRequestParameter.Value.ToString());
                        result.AppendFormat("<tr><td class=\"AttributeStyle\">{0}</td><td class=\"AttributeModificationStyle\">{1}</td><td class=\"NewValueStyle\">{2}</td></tr>", createRequestParameter.PropertyName, createRequestParameter.Operation, parameterValue);
                    }
                }
            }

            return result.ToString();
        }
    }
}
