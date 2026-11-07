//-----------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionFunction.cs" company="Microsoft">
//      Copyright (c) Microsoft. All Rights Reserved.
//      Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>
// <summary>
// ExpressionEvaluator class
// </summary>
//-----------------------------------------------------------------------------------------------------------------------

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    #region Namespaces Declarations

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.Odbc;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using Microsoft.ResourceManagement.WebServices;
    using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
    using System.Security.Cryptography;

    #endregion

    /// <summary>
    /// Represents a function expression
    /// </summary>
    [SuppressMessage("SonarAnalyzer.CSharp", "S3247", Justification = "type-check-and-cast not available in .Net Framework 3.5")]
    public partial class ExpressionFunction
    {
        #region Declarations

        /// <summary>
        /// The function
        /// </summary>
        private readonly string function;

        /// <summary>
        /// The evaluation mode
        /// </summary>
        private readonly EvaluationMode mode;

        /// <summary>
        /// The function parameters
        /// </summary>
        private readonly ArrayList parameters;

        #endregion

        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionFunction" /> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="mode">The mode.</param>
        public ExpressionFunction(string function, ArrayList parameters, EvaluationMode mode)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionConstructor, FunctionEvaluationModeTemplate, function, mode);

            this.function = function;
            this.parameters = parameters;
            this.mode = mode;

            Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionConstructor, FunctionEvaluationModeTemplate, function, mode);
        }

        #region Methods

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>The result of the function evaluation.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Reviewed and retained as this is part of the exception message formatting.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Approved.")]
        public object Run()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRun, FunctionEvaluationModeTemplate, this.function, this.mode);

            try
            {
                // Based on the function name,
                // call the appropriate method and return the results
                switch (this.function.ToUpperInvariant())
                {
                    case "ADD":
                        return this.Add();

                    case "AFTER":
                        return this.After();

                    case "AND":
                        return this.And();

                    case "BEFORE":
                        return this.Before();

                    case "BITAND":
                        return this.BitAnd();

                    case "BITNOT":
                        return this.BitNot();

                    case "BITOR":
                        return this.BitOr();

                    case "CONCATENATE":
                        return this.Concatenate();

                    case "CONCATINATEMULTIVALUEDSTRING":
                        Logger.Instance.WriteWarning(EventIdentifier.ExpressionFunctionRunDeprecatedFunctionWarning, Messages.ExpressionFunction_DeprecatedFunctionWarning, "ConcatinateMultivaluedString", "ConcatenateMultivaluedString");
                        return this.ConcatenateMultivaluedString();

                    case "CONCATENATEMULTIVALUEDSTRING":
                        return this.ConcatenateMultivaluedString();

                    case "CONTAINS":
                        return this.Contains();

                    case "CONVERTFROMBASE64":
                        return this.ConvertFromBase64();

                    case "CONVERTNUMBERTOLIST":
                        return this.ConvertNumberToList();

                    case "CONVERTSIDTOSTRING":
                        return this.ConvertSidToString();

                    case "CONVERTSTRINGTOGUID":
                    case "CONVERTTOGUID":
                        return this.ConvertStringToGuid();

                    case "CONVERTTOBASE64":
                        return this.ConvertToBase64();

                    case "CONVERTTOBOOLEAN":
                        return this.ConvertToBoolean();

                    case "CONVERTTONUMBER":
                        return this.ConvertToNumber();

                    case "CONVERTTOSTRING":
                        return this.ConvertToString();

                    case "CONVERTTOUNIQUEIDENTIFIER":
                        return this.ConvertToUniqueIdentifier();

                    case "COUNT":
                        return this.Count();

                    case "CREATESQLPARAMETER":
                        return this.CreateSqlParameter();

                    case "CREATESQLPARAMETER2":
                        return this.CreateSqlParameter2();

                    case "CR":
                        return this.CR();

                    case "CRLF":
                        return this.CRLF();

                    case "DATETIMEADD":
                        return this.DateTimeAdd();

                    case "DATETIMEFORMAT":
                        return this.DateTimeFormat();

                    case "DATETIMEFROMFILETIMEUTC":
                        return this.DateTimeFromFileTimeUtc();

                    case "DATETIMEFROMSTRING":
                        return this.DateTimeFromString();

                    case "DATETIMENOW":
                        return this.DateTimeNow();

                    case "DATETIMETOFILETIMEUTC":
                        return this.DateTimeToFileTimeUtc();

                    case "DATETIMESUBTRACT":
                        return this.DateTimeSubtract();

                    case "DATETIMEUTCTOLOCALTIME":
                        return this.DateTimeUtcToLocalTime();

                    case "DIVIDE":
                        return this.Divide();

                    case "EQ":
                        return this.Eq();

                    case "ESCAPEDNCOMPONENT":
                        return this.EscapeDNComponent();

                    case "EXECUTESQLNONQUERY":
                        return this.ExecuteSqlNonQuery();

                    case "EXECUTESQLSCALAR":
                        return this.ExecuteSqlScalar();

                    case "FIRST":
                        return this.First();

                    case "FORMATMULTIVALUEDLIST":
                        return this.FormatMultivaluedList();

                    case "GENERATERANDOMPASSWORD":
                        return this.GenerateRandomPassword();

                    case "GREATERTHAN":
                        return this.GreaterThan();

                    case "IIF":
                        return this.IIF();

                    case "INDEXBYVALUE":
                        return this.IndexByValue();

                    case "INSERTVALUES":
                        return this.InsertValues();

                    case "ISPRESENT":
                        return this.IsPresent();

                    case "LAST":
                        return this.Last();

                    case "LEFT":
                        return this.Left();

                    case "LEFTPAD":
                        return this.LeftPad();

                    case "LENGTH":
                        return this.Length();

                    case "LESSTHAN":
                        return this.LessThan();

                    case "LOWERCASE":
                        return this.LowerCase();

                    case "LTRIM":
                        return this.LTrim();

                    case "MID":
                        return this.Mid();

                    case "MOD":
                        return this.Mod();

                    case "MULTIPLY":
                        return this.Multiply();

                    case "NORMALIZESTRING":
                        return this.NormalizeString();

                    case "NOT":
                        return this.Not();

                    case "NULL":
                        return this.Null();

                    case "OR":
                        return this.Or();

                    case "PARAMETERSCONTAIN":
                        return this.ParametersContain();

                    case "PARAMETERSLIST":
                        return this.ParametersList();

                    case "PARAMETERSTABLE":
                        return this.ParametersTable();

                    case "PARAMETERVALUE":
                        return this.ParameterValue();

                    case "PARAMETERVALUEADDED":
                        return this.ParameterValueAdded();

                    case "PARAMETERVALUEREMOVED":
                        return this.ParameterValueRemoved();

                    case "PROPERCASE":
                        return this.ProperCase();

                    case "SORTLIST":
                        return this.SortList();

                    case "SPLITSTRING":
                        return this.SplitString();

                    case "SUBTRACT":
                        return this.Subtract();

                    case "RANDOMNUM":
                        return this.RandomNum();

                    case "REGEXMATCH":
                        return this.RegexMatch();

                    case "REGEXREPLACE":
                        return this.RegexReplace();

                    case "REMOVEDUPLICATES":
                        return this.RemoveDuplicates();

                    case "REMOVEVALUES":
                        return this.RemoveValues();

                    case "REPLACESTRING":
                        return this.ReplaceString();

                    case "RIGHT":
                        return this.Right();

                    case "RIGHTPAD":
                        return this.RightPad();

                    case "RTRIM":
                        return this.RTrim();

                    case "TITLECASE":
                        return this.TitleCase();

                    case "TRIM":
                        return this.Trim();

                    case "UPPERCASE":
                        return this.UpperCase();

                    case "VALUEBYINDEX":
                        return this.ValueByIndex();

                    case "VALUEBYKEY":
                        return this.ValueByKey();

                    case "VALUETYPE":
                        return this.ValueType();

                    case "WORD":
                        return this.Word();

                    case "WRAPXPATHFILTER":
                        return this.WrapXPathFilter();

                    default:
                        throw new NotSupportedFunctionException();
                }
            }
            catch (NotSupportedFunctionException ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunUnsupportedFunctionError, new NotSupportedFunctionException(Messages.ExpressionFunction_UnsupportedFunctionError, ex, this.function, this.mode));
            }
            catch (InvalidFunctionFormatException ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunFunctionSyntaxValidationError, new InvalidFunctionFormatException(Messages.ExpressionFunction_FunctionSyntaxValidationError, ex, this.function, this.mode));
            }
            catch(InvalidArgumentException ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunFunctionInvalidArgumentError, new InvalidArgumentException(Messages.ExpressionFunction_FunctionInvalidArgumentError, ex, this.function, this.mode));
            }
            catch (ArgumentException ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunFunctionInvalidArgumentError, new InvalidArgumentException(Messages.ExpressionFunction_FunctionInvalidArgumentError, ex, this.function, this.mode));
            }
            catch (OverflowException ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunFunctionOverflowError, new OverflowNotAllowedException(Messages.ExpressionFunction_FunctionOverflowError, ex, this.function, this.mode));
            }
            catch (OverflowNotAllowedException ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunFunctionOverflowError, new OverflowNotAllowedException(Messages.ExpressionFunction_FunctionOverflowError, ex, this.function, this.mode));
            }
            catch (Exception ex)
            {
                // dump all function parameters in an error log entry
                string paramString = string.Empty;
                StringBuilder sb = new StringBuilder();
                if (this.mode != EvaluationMode.Parse)
                {
                    try
                    {
                        if (this.parameters != null && this.parameters.Count > 0)
                        {
                            for (var i = 0; i < this.parameters.Count; ++i)
                            {
                                var parameter = this.parameters[i];
                                sb.Append("'");

                                if (parameter != null)
                                {
                                    try
                                    {
                                        sb.Append(
                                                Convert.ToString(parameter, CultureInfo.InvariantCulture))
                                            .Append("',");
                                    }
                                    catch (Exception)
                                    {
                                        // let it print without the ending single quote as an indication of the conversion failure
                                    }
                                }
                                else
                                {
                                    sb.Append("',");
                                }
                            }
                        }

                        if (sb.Length > 0)
                        {
                            sb.Length--; // Remove final comma
                        }

                        sb.Insert(0, "(")
                            .Append(")");

                        paramString = sb.ToString();

                    }
                    catch (Exception)
                    {
                        // do nothing - silenty ignore
                    }
                }

                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRunUnknownFunctionExecutionError, new InvalidFunctionOperationException(Messages.ExpressionFunction_UnknownFunctionExecutionError, this.mode.ToString().ToLowerInvariant(), ex, this.function + paramString, ex.Message));
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRun, FunctionEvaluationModeTemplate, this.function, this.mode);
            }
        }

        /// <summary>
        /// Deserializes the XML document into an object of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the object to be deserialized</typeparam>
        /// <param name="xml">Serialized representation of the specified type</param>
        /// <returns>The Object being deserialized.</returns>
        internal static T DeserializeObject<T>(string xml)
        where T : class
        {
            T t;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            StringReader stringReader = null;
            XmlTextReader xmlTextReader = null;

            try
            {
                stringReader = new StringReader(xml);
                xmlTextReader = new XmlTextReader(stringReader);
                xmlTextReader.ProhibitDtd = true;

                t = xmlSerializer.Deserialize(xmlTextReader) as T;
            }
            finally
            {
                // This closes the XmlTextReader and safely cascades to close stringReader.
                if (xmlTextReader != null)
                    xmlTextReader.Close();                
            }

            return t;
        }

        #endregion Methods

                
    }
}