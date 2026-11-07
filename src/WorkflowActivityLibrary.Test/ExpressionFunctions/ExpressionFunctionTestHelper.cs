using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions
{
    internal static class ExpressionFunctionTestHelper
    {
        public static string CreateRequestParameter(
            string propertyName,
            object value,
            UpdateMode? mode)
        {
            CreateRequestParameter parameter;

            if (mode.HasValue)
            {
                UpdateRequestParameter updateParameter =
                    new UpdateRequestParameter();

                updateParameter.Mode = mode.Value;

                parameter = updateParameter;
            }
            else
            {
                parameter = new CreateRequestParameter();
            }

            parameter.Target =
                new Guid("c302a07d-5400-43af-94dd-83aa65e5aed4");

            parameter.Calculated = false;
            parameter.PropertyName = propertyName;
            parameter.Value = value;
            parameter.Operation = OperationType.Create;

            XmlSerializer serializer =
                new XmlSerializer(typeof(RequestParameter));

            XmlWriterSettings settings =
                new XmlWriterSettings();

            settings.OmitXmlDeclaration = true;
            settings.Indent = false;

            StringBuilder builder =
                new StringBuilder();

            using (XmlWriter writer =
                XmlWriter.Create(builder, settings))
            {
                serializer.Serialize(writer, parameter);
            }

            return builder.ToString();
        }

        public static string CreateRequestParameter(
            string propertyName,
            object value)
        {
            return CreateRequestParameter(
                propertyName,
                value,
                null);
        }

        public static Exception AssertThrows(
        Action action,
        Type expectedExceptionType,
        string expectedExceptionMessage)
        {
            Exception exception = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception, "Expected an exception to be thrown.");

            Assert.IsInstanceOfType(
                exception,
                expectedExceptionType);            
            
            if(!string.IsNullOrEmpty(expectedExceptionMessage))
                Assert.AreEqual(
                    expectedExceptionMessage,
                    exception.InnerException.Message,
                    "The exception message did not match.");

            return exception;
        }

        public static object Run(
        string functionName,
        params object[] parameters)
        {
            var func = new ExpressionFunction(
                functionName,
                new ArrayList(parameters),
                EvaluationMode.Resolve);

            return func.Run();
        }

        public static object RunParse(
            string functionName,
            params object[] parameters)
        {
            var func = new ExpressionFunction(
                functionName,
                new ArrayList(parameters),
                EvaluationMode.Parse);

            return func.Run();
        }
    }
}
