using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {

        /// <summary>
        /// This function is used to determine if the request parameters of the current request contain the specified attribute name.
        /// This function is useful to verify a request.
        /// Function Syntax: ParametersContain(RequestParameter:List<RequestParameter>, attribute:string)
        /// </summary>
        /// <returns>
        /// true if the request parameters contain the specified attribute name. Otherwise false.
        /// </returns>
        private bool ParametersContain()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionParametersContain, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersContainInvalidFunctionParameterCountError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));
                
                Type parameterType = typeof(List<string>);
                Type parameterType2 = typeof(string);
                object parameter = this.parameters[0];
                if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersContainInvalidFirstFunctionParameterTypeError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError2, this.function, parameterType.Name, parameterType2.Name, parameter.GetType().Name));

                Type attributeNameType = typeof(string);
                object attributeName = this.parameters[1];

                if (attributeName == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersContainNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(attributeName, attributeNameType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionParametersContainInvalidSecondFunctionParameterTypeError, 
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, attributeNameType.Name, attributeName.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return false;

                List<string> requestParameters = GetRequestParametersAsList();
                bool result = AnyRequestParameterContainsAttribute(requestParameters);                

                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionParametersContain, "ParametersContain function for attribute '{0}' evaluated '{1}'.", this.parameters[1], result);                

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionParametersContain, EvaluationModeTemplate, this.mode);
            }
        }

        private bool AnyRequestParameterContainsAttribute(List<string> requestParameters)
        {
            if (requestParameters == null)
                return false;

            foreach (string s in requestParameters)
            {
                // Consider: ParametersContain("[//Request/RequestParameter]", "AccountName")
                bool found = s.Trim().StartsWith("<RequestParameter", StringComparison.OrdinalIgnoreCase)
                    ? ContainsRequestParameterAttribute(s)
                    : ContainsOldStyleRequestParameterAttribute();

                if (found)
                    return true;
            }

            return false;
        }

        private bool ContainsRequestParameterAttribute(string s)
        {
            RequestParameter requestParameter = DeserializeObject<RequestParameter>(s);

            // Check UpdateRequestParameter before CreateRequestParameter, as the former inherits from the latter
            UpdateRequestParameter update = requestParameter as UpdateRequestParameter;
            if (update != null)
                return IsMatchingPropertyName(update.PropertyName);

            CreateRequestParameter create = requestParameter as CreateRequestParameter;
            if (create != null)
                return IsMatchingPropertyName(create.PropertyName);

            return false;
        }

        private bool IsMatchingPropertyName(string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && propertyName.Equals(this.parameters[1]);
        }

        private bool ContainsOldStyleRequestParameterAttribute()
        {
            // Consider: ParametersContain("[//RequestParameter/AllChangesAuthorizationTable]", "AccountName")
            // We have now deprecated this usage in favour of [//Request/RequestParameter] but process the code for backward compatibility.
            Logger.Instance.WriteWarning(EventIdentifier.ExpressionFunctionParametersContainWarning, Messages.ExpressionFunction_ParametersContainDeprecatedParameterWarning, LookupParameter.RequestParameter, LookupParameter.Request);

            // AllChangesAuthorizationTable and AllChangesActionTable are not well-formed XML documents.
            try
            {
                string xpath = "//td[@class='AttributeStyle' and text() ='" + this.parameters[1] + "']/parent::node()/td[@class='NewValueStyle']";
                XElement element = XElement.Parse(this.parameters[0].ToString()).XPathSelectElement(xpath);
                return element != null;
            }
            catch (XmlException e)
            {
                Logger.Instance.WriteWarning(EventIdentifier.ExpressionFunctionParametersContainWarning, "ParametersContain function for attribute '{0}' threw exception while parsing xml. '{1}'.", this.parameters[1], e);

                // fallback to simple string search
                return this.parameters[0].ToString().Contains(">" + this.parameters[1] + "<");
            }
        }
    }
}
