using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "I haven't touched these methods yet.")]
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to create a <see cref="DbParameter" /> object for use in various ExecuteSql functions.
        /// Function Syntax: CreateSqlParameter(sqlConnectionStringConfigKey:string, parameterName:string, parameterValue:object [,parameterDirection:string])
        /// </summary>
        /// <returns>The <see cref="DbParameter" /> object.</returns>
        private DbParameter CreateSqlParameter()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionCreateSqlParameter, EvaluationModeTemplate, this.mode);

            try
            {
                if (this.parameters.Count < 3 || this.parameters.Count > 4)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, 3, 4, this.parameters.Count));
                }

                for (int i = 0; i < 2; ++i)
                {
                    object parameter = this.parameters[i]; // 0 = SQL Connection String Config Key, 1 = Parameter Name
                    Type parameterType = typeof(string);
                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                    }

                    if (this.mode != EvaluationMode.Parse)
                    {
                        switch (i)
                        {
                            case 0: // SQL Connection String Config Key
                                {
                                    var connectionStringConfigKey = this.parameters[0] as string;
                                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;
                                    if (string.IsNullOrEmpty(providerName))
                                    {
                                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                                    }

                                    try
                                    {
                                        DbProviderFactories.GetFactory(providerName);
                                    }
                                    catch (DbException)
                                    {
                                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                                    }
                                }

                                break;
                            case 1: // SQL Parameter Name
                                var parameterName = parameter as string;
                                if (!parameterName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError, this.function, i, "@" + parameterName, parameterName));
                                }

                                break;
                        }
                    }
                }

                if (this.parameters.Count == 4)
                {
                    var i = 3;
                    object parameter = this.parameters[i]; // 3 = Parameter Direction
                    Type parameterType = typeof(string);
                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                    }

                    if (this.mode != EvaluationMode.Parse)
                    {
                        try
                        {
                            Enum.Parse(typeof(ParameterDirection), parameter as string, true);
                        }
                        catch (ArgumentException e)
                        {
                            throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, e, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                        }
                    }
                }

                DbParameter result = null;

                if (this.mode != EvaluationMode.Parse)
                {
                    string parameterName = this.parameters[1] as string;
                    object parameterValue = this.parameters[2];
                    var parameterDirection = ParameterDirection.Input;

                    if (this.parameters.Count == 4)
                    {
                        parameterDirection = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), this.parameters[3] as string, true);
                    }

                    var connectionStringConfigKey = this.parameters[0] as string;
                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;
                    if (string.IsNullOrEmpty(providerName))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                    }

                    var factory = DbProviderFactories.GetFactory(providerName);

                    result = factory.CreateParameter();
                    result.ParameterName = parameterName;
                    result.Direction = parameterDirection;
                    result.Value = (parameterValue == null) ? DBNull.Value : parameterValue;

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCreateSqlParameter, "CreateSqlParameter('{0}', '{1}', '{2}', '{3}') returned '{4}'.", this.parameters[0], result.ParameterName, result.Value, result.Direction, result);
                }

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCreateSqlParameter, EvaluationModeTemplate, this.mode);
            }
        }


        /// <summary>
        /// This function is used to create a <see cref="DbParameter" /> object for use in various ExecuteSql functions.
        /// Function Syntax: CreateSqlParameter2(sqlConnectionStringConfigKey:string, parameterName:string, parameterDirection:string, parameterType:string [, parameterSize:integer, parameterValue:object])
        /// </summary>
        /// <returns>The <see cref="DbParameter" /> object.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed.")]
        private DbParameter CreateSqlParameter2()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionCreateSqlParameter2, EvaluationModeTemplate, this.mode);

            try
            {
                if (this.parameters.Count < 4 || this.parameters.Count > 6)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, 4, 6, this.parameters.Count));
                }

                for (int i = 0; i < 4; ++i)
                {
                    object parameter = this.parameters[i]; // 0 = SQL Connection String Config Key, 1 = Parameter Name, 2 = Parameter Direction, 3 = Parameter Type
                    Type parameterType = typeof(string);
                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                    }

                    if (this.mode != EvaluationMode.Parse)
                    {
                        switch (i)
                        {
                            case 0: // SQL Connection String Config Key
                                {
                                    var connectionStringConfigKey = this.parameters[0] as string;
                                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;
                                    if (string.IsNullOrEmpty(providerName))
                                    {
                                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                                    }

                                    try
                                    {
                                        DbProviderFactories.GetFactory(providerName);
                                    }
                                    catch (DbException)
                                    {
                                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                                    }
                                }

                                break;
                            case 1: // SQL Parameter Name
                                var parameterName = parameter as string;
                                if (!parameterName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError, this.function, i, "@" + parameterName, parameterName));
                                }

                                break;
                            case 2: // SQL Parameter Direction
                                {
                                    try
                                    {
                                        Enum.Parse(typeof(ParameterDirection), parameter as string, true);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, e, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                                    }
                                }

                                break;
                            case 3: // SQL Parameter Type
                                {
                                    var connectionStringConfigKey = this.parameters[0] as string;
                                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;
                                    var factory = DbProviderFactories.GetFactory(providerName);

                                    var dbParameter = factory.CreateParameter();
                                    if (dbParameter is SqlParameter)
                                    {
                                        try
                                        {
                                            Enum.Parse(typeof(SqlDbType), parameter as string, true);
                                        }
                                        catch (ArgumentException)
                                        {
                                            try
                                            {
                                                Enum.Parse(typeof(DbType), parameter as string, true);
                                            }
                                            catch (ArgumentException e)
                                            {
                                                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError2, e, this.function, i + 1, typeof(SqlDbType), typeof(DbType), parameter.GetType().Name));
                                            }
                                        }
                                    }
                                    else if (dbParameter is OdbcParameter)
                                    {
                                        try
                                        {
                                            Enum.Parse(typeof(OdbcType), parameter as string, true);
                                        }
                                        catch (ArgumentException)
                                        {
                                            try
                                            {
                                                Enum.Parse(typeof(DbType), parameter as string, true);
                                            }
                                            catch (ArgumentException e)
                                            {
                                                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError2, e, this.function, i + 1, typeof(OdbcType), typeof(DbType), parameter.GetType().Name));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Enum.Parse(typeof(DbType), parameter as string, true);
                                        }
                                        catch (ArgumentException e)
                                        {
                                            throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, e, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                                        }
                                    }
                                }

                                break;
                        }
                    }
                }

                if (this.parameters.Count > 4)
                {
                    var i = 4;
                    object parameter = this.parameters[i]; // SQL Parameter Size
                    Type parameterType = typeof(int);
                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                    }
                }

                DbParameter result = null;

                if (this.mode != EvaluationMode.Parse)
                {
                    var connectionStringConfigKey = this.parameters[0] as string;
                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;
                    var factory = DbProviderFactories.GetFactory(providerName);

                    var parameterName = this.parameters[1] as string;
                    var parameterDirection = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), this.parameters[2] as string, true);
                    var parameterDbType = this.parameters[3] as string;

                    var parameterSize = 0;
                    if (this.parameters.Count > 4)
                    {
                        parameterSize = Convert.ToInt32(this.parameters[4], CultureInfo.InvariantCulture); // SQL Parameter Size
                    }

                    object parameterValue = null;
                    if (this.parameters.Count > 5)
                    {
                        parameterValue = this.parameters[5]; // SQL Parameter Value
                    }

                    result = factory.CreateParameter();
                    result.ParameterName = parameterName;
                    result.Direction = parameterDirection;

                    if (result is SqlParameter)
                    {
                        try
                        {
                            ((SqlParameter)result).SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), parameterDbType, true);
                        }
                        catch (ArgumentException)
                        {
                            result.DbType = (DbType)Enum.Parse(typeof(DbType), parameterDbType, true);
                        }
                    }
                    else if (result is OdbcParameter)
                    {
                        try
                        {
                            ((OdbcParameter)result).OdbcType = (OdbcType)Enum.Parse(typeof(OdbcType), parameterDbType, true);
                        }
                        catch (ArgumentException)
                        {
                            result.DbType = (DbType)Enum.Parse(typeof(DbType), parameterDbType, true);
                        }
                    }
                    else
                    {
                        result.DbType = (DbType)Enum.Parse(typeof(DbType), parameterDbType, true);
                    }

                    if (parameterSize != 0)
                    {
                        result.Size = parameterSize;
                    }

                    result.Value = (parameterValue == null) ? DBNull.Value : parameterValue;

                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCreateSqlParameter2, "CreateSqlParameter2('{0}', '{1}', '{2}', '{3}', '{4}', '{5}') returned '{6}'.", connectionStringConfigKey, result.ParameterName, result.Direction, result.DbType, result.Size, result.Value, result);
                }

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCreateSqlParameter2, EvaluationModeTemplate, this.mode);
            }
        }
        

        /// <summary>
        /// This function is used to execute a SQL stored procedure or query against a SQL database.
        /// Function Syntax: ExecuteSqlScalar(sqlConnectionStringConfigKey:string, sqlStatement:string [, parameter1:SqlParameter, parameter2:SqlParameter, ...])
        /// </summary>
        /// <returns>The first column of the first row in the result set.</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S2077", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        private object ExecuteSqlScalar()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionExecuteSqlScalar, EvaluationModeTemplate, this.mode);

            try
            {
                if (this.parameters.Count < 2)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinCountError, this.function, 2, this.parameters.Count));
                }

                for (int i = 0; i < 2; ++i)
                {
                    object parameter = this.parameters[i];
                    Type parameterType = typeof(string);

                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                    }
                }

                for (int i = 2; i < this.parameters.Count; ++i)
                {
                    object parameter = this.parameters[i];
                    Type parameterType = typeof(DbParameter);
                    Type parameterType2 = typeof(SqlParameter);
                    Type parameterType3 = typeof(OdbcParameter);

                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2) && !this.VerifyType(parameter, parameterType3))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError3, this.function, i + 1, parameterType.Name, parameterType2.Name, parameterType3.Name, parameter.GetType().Name));
                    }
                }

                object result = null;

                if (this.mode != EvaluationMode.Parse)
                {
                    var connectionStringConfigKey = this.parameters[0] as string;
                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                    var connectionString = connectionStringConfig != null ? connectionStringConfig.ConnectionString : null;
                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;

                    if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(providerName))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                    }

                    Int32 commandTimeout;
                    commandTimeout = int.TryParse(ConfigurationManager.AppSettings["WAL_DBCommandTimeout"], out commandTimeout) ? commandTimeout : 30;

                    try
                    {
                        var factory = DbProviderFactories.GetFactory(providerName);
                        using (var dbConnection = factory.CreateConnection())
                        {
                            dbConnection.ConnectionString = connectionString;

                            if (providerName.Equals(SystemDataOdbcTemplate, StringComparison.OrdinalIgnoreCase) &&
                                connectionString.IndexOf(ConnectionTimeoutTemplate, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                try
                                {
                                    var connectionTimeout = Convert.ToInt32(connectionString.ToUpperInvariant().Split(new string[] { ConnectionTimeoutTemplate }, StringSplitOptions.RemoveEmptyEntries)[1].Split(';')[0], CultureInfo.InvariantCulture);
                                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionExecuteSqlScalar, "Provider Name: '{0}'. Connection Timeout: '{1}'.", providerName, connectionTimeout);
                                    ((OdbcConnection)dbConnection).ConnectionTimeout = connectionTimeout;
                                }
                                catch (Exception e)
                                {
                                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, e, this.function, "Connection Timeout"));
                                }
                            }

                            using (var dbCommand = factory.CreateCommand())
                            {
                                dbCommand.Connection = dbConnection;
                                var dbCmdText = (this.parameters[1] as string).Trim();
                                dbCommand.CommandText = dbCmdText;

                                // If the command text has spaces inbetween, assume it's a query, else stored procedure as we won't support anything else.
                                dbCommand.CommandType = dbCmdText.Contains(" ") ? CommandType.Text : CommandType.StoredProcedure;
                                dbCommand.CommandTimeout = commandTimeout;

                                string paramString = string.Empty;
                                var sb = new StringBuilder();
                                for (var i = 2; i < this.parameters.Count; ++i)
                                {
                                    var parameter = this.parameters[i] as DbParameter;
                                    dbCommand.Parameters.Add(parameter);

                                    sb.Append(parameter.ParameterName).
                                        Append("='").
                                        Append(parameter.Value).
                                        Append("',");
                                }

                                paramString = sb.ToString().TrimEnd(',');

                                if (providerName.Equals(SystemDataOdbcTemplate, StringComparison.OrdinalIgnoreCase) && dbCommand.CommandType == CommandType.StoredProcedure)
                                {
                                    var paramCount = dbCommand.Parameters.Count;

                                    sb = new StringBuilder();

                                    var paramPlaceholder = string.Empty;
                                    for (var i = 0; i < paramCount; ++i)
                                    {
                                        if (i > 0)
                                        {
                                            sb.Append(',');
                                        }

                                        sb.Append('?');
                                    }
                                    paramPlaceholder = sb.ToString();

                                    if (!string.IsNullOrEmpty(paramPlaceholder))
                                    {
                                        dbCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "{0} CALL {1}({2}) {3}", "{", dbCmdText, paramPlaceholder, "}");
                                    }
                                }

                                dbConnection.Open();
                                result = dbCommand.ExecuteScalar();

                                if (result == DBNull.Value)
                                {
                                    result = null;
                                }

                                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionExecuteSqlScalar, "ExecuteSqlScalar('{0}', '{1}, '{2}') returned '{3}'.", connectionStringConfigKey, dbCommand.CommandText, paramString, result);
                            }
                        }
                    }
                    catch (DbException e)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, e);
                    }
                }

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionExecuteSqlScalar, EvaluationModeTemplate, this.mode);
            }
        }


        /// <summary>
        /// This function is used to execute a SQL stored procedure or insert, update, delete statements against a SQL database.
        /// Function Syntax: ExecuteSqlNonQuery(sqlConnectionStringConfigKey:string, sqlStatement:string [, parameter1:SqlParameter, parameter2:SqlParameter, ...])
        /// </summary>
        /// <returns>The dictionary of output parameter names and their values.</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S2077", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        private Dictionary<string, object> ExecuteSqlNonQuery()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionExecuteSqlNonQuery, EvaluationModeTemplate, this.mode);

            try
            {
                if (this.parameters.Count < 2)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinCountError, this.function, 2, this.parameters.Count));
                }

                for (int i = 0; i < 2; ++i)
                {
                    object parameter = this.parameters[i];
                    Type parameterType = typeof(string);

                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, i + 1, parameterType.Name, parameter.GetType().Name));
                    }
                }

                for (int i = 2; i < this.parameters.Count; ++i)
                {
                    object parameter = this.parameters[i];
                    Type parameterType = typeof(DbParameter);
                    Type parameterType2 = typeof(SqlParameter);
                    Type parameterType3 = typeof(OdbcParameter);

                    if (parameter == null)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));
                    }

                    if (!this.VerifyType(parameter, parameterType) && !this.VerifyType(parameter, parameterType2) && !this.VerifyType(parameter, parameterType3))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError3, this.function, i + 1, parameterType.Name, parameterType2.Name, parameterType3.Name, parameter.GetType().Name));
                    }
                }

                Dictionary<string, object> result = null;

                if (this.mode != EvaluationMode.Parse)
                {
                    var connectionStringConfigKey = this.parameters[0] as string;
                    var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];
                    var connectionString = connectionStringConfig != null ? connectionStringConfig.ConnectionString : null;
                    var providerName = connectionStringConfig != null ? connectionStringConfig.ProviderName : null;

                    if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(providerName))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                    }

                    Int32 commandTimeout;
                    commandTimeout = int.TryParse(ConfigurationManager.AppSettings["WAL_DBCommandTimeout"], out commandTimeout) ? commandTimeout : 30;

                    try
                    {
                        var factory = DbProviderFactories.GetFactory(providerName);
                        using (var dbConnection = factory.CreateConnection())
                        {
                            dbConnection.ConnectionString = connectionString;

                            if (providerName.Equals(SystemDataOdbcTemplate, StringComparison.OrdinalIgnoreCase) &&
                                 (connectionString.IndexOf(ConnectionTimeoutTemplate, StringComparison.OrdinalIgnoreCase) != -1))
                            {
                                try
                                {
                                    var connectionTimeout = Convert.ToInt32(connectionString.ToUpperInvariant().Split(new string[] { ConnectionTimeoutTemplate }, StringSplitOptions.RemoveEmptyEntries)[1].Split(';')[0], CultureInfo.InvariantCulture);
                                    Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionExecuteSqlNonQuery, "Provider Name: '{0}'. Connection Timeout: '{1}'.", providerName, connectionTimeout);
                                    ((OdbcConnection)dbConnection).ConnectionTimeout = connectionTimeout;
                                }
                                catch (Exception e)
                                {
                                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, e, this.function, "Connection Timeout"));
                                }
                            }

                            using (var dbCommand = factory.CreateCommand())
                            {
                                dbCommand.Connection = dbConnection;
                                var dbCmdText = (this.parameters[1] as string).Trim();
                                dbCommand.CommandText = dbCmdText;

                                // If the command text has spaces inbetween, assume it's a query, else stored procedure as we won't support anything else.
                                dbCommand.CommandType = dbCmdText.Contains(" ") ? CommandType.Text : CommandType.StoredProcedure;
                                dbCommand.CommandTimeout = commandTimeout;

                                var sb = new StringBuilder();
                                for (var i = 2; i < this.parameters.Count; ++i)
                                {
                                    var parameter = this.parameters[i] as DbParameter;
                                    if (parameter != null)
                                    {
                                        dbCommand.Parameters.Add(parameter);
                                        sb.Append(parameter.ParameterName).
                                            Append("='").
                                            Append(parameter.Value).
                                            Append("',");
                                    }
                                }

                                var paramString = sb.ToString().TrimEnd(',');

                                if (providerName.Equals(SystemDataOdbcTemplate, StringComparison.OrdinalIgnoreCase) && dbCommand.CommandType == CommandType.StoredProcedure)
                                {
                                    var paramCount = dbCommand.Parameters.Count;
                                    sb = new StringBuilder();

                                    for (var i = 0; i < paramCount; ++i)
                                    {
                                        if (i > 0)
                                            sb.Append(",");

                                        sb.Append("?");
                                    }

                                    var paramPlaceholder = sb.ToString();

                                    if (!string.IsNullOrEmpty(paramPlaceholder))
                                    {
                                        dbCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "{0} CALL {1}({2}) {3}", "{", dbCmdText, paramPlaceholder, "}");
                                    }
                                }

                                dbConnection.Open();
                                var rowsAffected = dbCommand.ExecuteNonQuery();

                                result = new Dictionary<string, object>();
                                result.Add("@RowsAffected", rowsAffected);
                                foreach (DbParameter param in dbCommand.Parameters)
                                {
                                    if (param.Direction != ParameterDirection.Input)
                                    {
                                        result.Add(param.ParameterName, param.Value);
                                    }
                                }

                                string paramOutString = string.Join(",", result.Select(kvp => string.Format(CultureInfo.InvariantCulture, "{0}='{1}'", kvp.Key, kvp.Value)).ToArray());

                                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionExecuteSqlNonQuery, "ExecuteSqlNonQuery('{0}', '{1}, '{2}') returned a dictionary of '{3}' parameter/value pairs.", connectionStringConfigKey, dbCommand.CommandText, paramString, paramOutString);
                            }
                        }
                    }
                    catch (DbException e)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError, e);
                    }
                }

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionExecuteSqlNonQuery, EvaluationModeTemplate, this.mode);
            }
        }

        
    }
}
