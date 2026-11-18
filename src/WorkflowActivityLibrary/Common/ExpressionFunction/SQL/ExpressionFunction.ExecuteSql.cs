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
        /// This function is used to execute a SQL stored procedure or query against a SQL database.
        /// Function Syntax: ExecuteSqlScalar(sqlConnectionStringConfigKey:string, sqlStatement:string [, parameter1:SqlParameter, parameter2:SqlParameter, ...])
        /// </summary>
        /// <returns>The first column of the first row in the result set.</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S2077", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        private object ExecuteSqlScalar()
        {
            Logger.Instance.WriteMethodEntry(
                EventIdentifier.ExpressionFunctionExecuteSqlScalar,
                EvaluationModeTemplate,
                this.mode);

            try
            {
                if (!this.HasParameterCountMinimum(2))
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlScalarInvalidFunctionParameterCountError,
                        new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidFunctionParameterMinCountError,
                            this.function,
                            2,
                            this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter = this.parameters[0];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 1, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[1];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError,new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 2, parameterType.Name, parameter.GetType().Name));

                for (int i = 2; i < this.parameters.Count; ++i)
                {
                    parameter = this.parameters[i];

                    if (parameter == null)
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(
                                Messages.ExpressionFunction_NullFunctionParameterError, this.function, i + 1));

                    if (!this.VerifyType(parameter, typeof(DbParameter)) &&
                        !this.VerifyType(parameter, typeof(SqlParameter)) &&
                        !this.VerifyType(parameter, typeof(OdbcParameter)))
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(
                                Messages.ExpressionFunction_InvalidFunctionParameterTypeError3, this.function, i + 1, typeof(DbParameter).Name, typeof(SqlParameter).Name, typeof(OdbcParameter).Name, parameter.GetType().Name));
                    }
                }

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string connectionStringConfigKey = this.parameters[0] as string;
                string commandText = (this.parameters[1] as string).Trim();

                bool isTextCommand = commandText.Contains(" ");
                               
                if (isTextCommand && IsSqlTextCommandDenied())
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidFunctionParameterError, this.function, 2, "SQL Command Text", commandText));

                var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringConfigKey];

                string connectionString = connectionStringConfig != null
                        ? connectionStringConfig.ConnectionString
                        : null;

                string providerName = connectionStringConfig != null
                        ? connectionStringConfig.ProviderName
                        : null;

                if (string.IsNullOrEmpty(connectionString) ||
                    string.IsNullOrEmpty(providerName))
                {
                    throw Logger.Instance.ReportError( EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                }                

                DbProviderFactory factory;

                if (!TryGetDbProviderFactory(connectionStringConfigKey, out factory))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError,new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                
                int commandTimeout;
                commandTimeout = int.TryParse(ConfigurationManager.AppSettings["WAL_DBCommandTimeout"], out commandTimeout) ? commandTimeout : 30;

                try
                {
                    using (DbConnection connection = factory.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;

                        int connectionTimeout;
                        Exception connectionTimeoutException;

                        if (!TrySetOdbcConnectionTimeout(connection, providerName, connectionString, out connectionTimeout, out connectionTimeoutException))
                            throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError, new InvalidFunctionFormatException(
                                    Messages.ExpressionFunction_InvalidConfigKeyConfiguration, connectionTimeoutException, this.function, "Connection Timeout"));
                                                
                        if (connectionTimeout != 0)
                            Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionExecuteSqlScalar, "Provider Name: '{0}'. Connection Timeout: '{1}'.", providerName, connectionTimeout);
                        
                        using (DbCommand command = factory.CreateCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = commandText;
                            command.CommandType = isTextCommand
                                ? CommandType.Text
                                : CommandType.StoredProcedure;
                            command.CommandTimeout = commandTimeout;

                            string parameterLog = this.AddDbParameters(command, 2);

                            SetOdbcStoredProcedureCommandText(command, providerName, commandText);

                            connection.Open();

                            object result = command.ExecuteScalar();

                            if (result == DBNull.Value)
                                result = null;

                            Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionExecuteSqlScalar, "ExecuteSqlScalar('{0}', '{1}', '{2}') returned '{3}'.", connectionStringConfigKey, command.CommandText, parameterLog, result);

                            return result;
                        }
                    }
                }
                catch (DbException exception)
                {
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlScalarNullFunctionParameterError,
                        exception);
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(
                    EventIdentifier.ExpressionFunctionExecuteSqlScalar,
                    EvaluationModeTemplate,
                    this.mode);
            }
        }


        /// <summary>
        /// This function is used to execute a SQL stored procedure or insert, update, delete statements against a SQL database.
        /// Function Syntax: ExecuteSqlNonQuery(sqlConnectionStringConfigKey:string, sqlStatement:string [, parameter1:SqlParameter, parameter2:SqlParameter, ...])
        /// </summary>
        /// <returns>The dictionary of output parameter names and their values.</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "SQL text commands can be disabled through WAL_DenySqlTextCommands.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S2077", Justification = "SQL text commands can be disabled through WAL_DenySqlTextCommands.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S2223", Justification = "Value is assigned in unit tests.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S1168", Justification = "ExpressionFunction expects null.")]
        private Dictionary<string, object> ExecuteSqlNonQuery()
        {
            Logger.Instance.WriteMethodEntry(
                EventIdentifier.ExpressionFunctionExecuteSqlNonQuery,
                EvaluationModeTemplate,
                this.mode);

            try
            {
                if (!this.HasParameterCountMinimum(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterCountError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinCountError,
                            this.function, 2, this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter = this.parameters[0];

                if (parameter == null)
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        new InvalidFunctionFormatException( Messages.ExpressionFunction_NullFunctionParameterError,
                            this.function, 1));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidFunctionParameterTypeError,
                            this.function, 1, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[1];

                if (parameter == null)
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        new InvalidFunctionFormatException( Messages.ExpressionFunction_NullFunctionParameterError,
                            this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError,
                            this.function, 2, parameterType.Name, parameter.GetType().Name));

                for (int i = 2; i < this.parameters.Count; ++i)
                {
                    parameter = this.parameters[i];

                    if (parameter == null)
                        throw Logger.Instance.ReportError(
                            EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterTypeError,
                            new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError,
                                this.function, i + 1));

                    if (!this.VerifyType(parameter, typeof(DbParameter)) &&
                        !this.VerifyType(parameter, typeof(SqlParameter)) &&
                        !this.VerifyType(parameter, typeof(OdbcParameter)))
                    {
                        throw Logger.Instance.ReportError(
                            EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterTypeError,
                            new InvalidFunctionFormatException(
                                Messages.ExpressionFunction_InvalidFunctionParameterTypeError3,
                                this.function, i + 1,
                                typeof(DbParameter).Name,
                                typeof(SqlParameter).Name,
                                typeof(OdbcParameter).Name,
                                parameter.GetType().Name));
                    }
                }

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string connectionStringConfigKey = this.parameters[0] as string;
                string commandText = (this.parameters[1] as string).Trim();

                bool isTextCommand = commandText.Contains(" ");

                if (isTextCommand && IsSqlTextCommandDenied())
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionExecuteSqlNonQueryInvalidFunctionParameterTypeError,
                        new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError,
                            this.function, 2, "SQL Command Text", commandText));

                var connectionStringConfig =
                    ConfigurationManager.ConnectionStrings[connectionStringConfigKey];

                string connectionString = connectionStringConfig != null
                    ? connectionStringConfig.ConnectionString
                    : null;

                string providerName = connectionStringConfig != null
                    ? connectionStringConfig.ProviderName
                    : null;

                if (string.IsNullOrEmpty(connectionString) ||
                    string.IsNullOrEmpty(providerName))
                {
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));
                }

                DbProviderFactory factory;

                if (!TryGetDbProviderFactory(
                        connectionStringConfigKey,
                        out factory))
                {
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        new InvalidFunctionFormatException(
                            Messages.ExpressionFunction_InvalidConfigKeyConfiguration,
                            this.function, connectionStringConfigKey));
                }

                int commandTimeout;
                commandTimeout = int.TryParse(
                    ConfigurationManager.AppSettings["WAL_DBCommandTimeout"],
                    out commandTimeout)
                        ? commandTimeout
                        : 30;

                try
                {
                    using (DbConnection connection = factory.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;

                        if (providerName.Equals(
                                SystemDataOdbcTemplate,
                                StringComparison.OrdinalIgnoreCase) &&
                            connectionString.IndexOf(
                                ConnectionTimeoutTemplate,
                                StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            try
                            {
                                int connectionTimeout = Convert.ToInt32(connectionString
                                        .ToUpperInvariant()
                                        .Split(new string[] { ConnectionTimeoutTemplate }, StringSplitOptions.RemoveEmptyEntries)[1]
                                        .Split(';')[0], CultureInfo.InvariantCulture);

                                Logger.Instance.WriteVerbose(
                                    EventIdentifier.ExpressionFunctionExecuteSqlNonQuery, "Provider Name: '{0}'. Connection Timeout: '{1}'.",
                                    providerName, connectionTimeout);

                                ((OdbcConnection)connection).ConnectionTimeout =
                                    connectionTimeout;
                            }
                            catch (Exception exception)
                            {
                                throw Logger.Instance.ReportError(
                                    EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                                    new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration,
                                        exception, this.function, "Connection Timeout"));
                            }
                        }

                        using (DbCommand command = factory.CreateCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = commandText;
                            command.CommandType = isTextCommand
                                ? CommandType.Text
                                : CommandType.StoredProcedure;
                            command.CommandTimeout = commandTimeout;

                            string parameterLog = this.AddDbParameters(command, 2);

                            SetOdbcStoredProcedureCommandText(command, providerName, commandText);

                            connection.Open();

                            int rowsAffected = command.ExecuteNonQuery();

                            Dictionary<string, object> result =
                                new Dictionary<string, object>();

                            result.Add("@RowsAffected", rowsAffected);

                            foreach (DbParameter outputParameter in command.Parameters)
                            {
                                if (outputParameter.Direction != ParameterDirection.Input)
                                {
                                    result.Add(
                                        outputParameter.ParameterName,
                                        outputParameter.Value);
                                }
                            }

                            string outputParameterLog = string.Join(
                                ",",
                                result.Select(
                                    delegate (KeyValuePair<string, object> item)
                                    {
                                        return string.Format(
                                            CultureInfo.InvariantCulture,
                                            "{0}='{1}'",
                                            item.Key,
                                            item.Value);
                                    }).ToArray());

                            Logger.Instance.WriteVerbose(
                                EventIdentifier.ExpressionFunctionExecuteSqlNonQuery,
                                "ExecuteSqlNonQuery('{0}', '{1}', '{2}') returned '{3}'.",
                                connectionStringConfigKey,
                                command.CommandText,
                                parameterLog,
                                outputParameterLog);

                            return result;
                        }
                    }
                }
                catch (DbException exception)
                {
                    throw Logger.Instance.ReportError(
                        EventIdentifier.ExpressionFunctionExecuteSqlNonQueryNullFunctionParameterError,
                        exception);
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(
                    EventIdentifier.ExpressionFunctionExecuteSqlNonQuery,
                    EvaluationModeTemplate,
                    this.mode);
            }
        }


#pragma warning disable 0649
        // required only for unit tests
        [SuppressMessage("SonarAnalyzer.CSharp", "S2223", Justification = "Required only for unit tests.")]
        internal static string SqlTextCommandDeniedSettingOverride;
#pragma warning restore 0649

        private static bool IsSqlTextCommandDenied()
        {
            string value = SqlTextCommandDeniedSettingOverride ??
                ConfigurationManager.AppSettings["WAL_DenySqlTextCommands"];

            bool denySqlTextCommands;
            bool.TryParse(value, out denySqlTextCommands);

            return denySqlTextCommands;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S2077", Justification = "The query does not contain end-user input as well as it's parameterised.")]
        private static void SetOdbcStoredProcedureCommandText(
            DbCommand command,
            string providerName,
            string storedProcedureName)
        {
            if (!providerName.Equals(
                    SystemDataOdbcTemplate,
                    StringComparison.OrdinalIgnoreCase) ||
                command.CommandType != CommandType.StoredProcedure)
            {
                return;
            }

            StringBuilder parameterPlaceholders = new StringBuilder();

            for (int i = 0; i < command.Parameters.Count; ++i)
            {
                if (i > 0)
                    parameterPlaceholders.Append(",");

                parameterPlaceholders.Append("?");
            }

            if (parameterPlaceholders.Length > 0)
            {
                command.CommandText = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} CALL {1}({2}) {3}",
                    "{",
                    storedProcedureName,
                    parameterPlaceholders,
                    "}");
            }
        }

        private static bool TrySetOdbcConnectionTimeout(
            DbConnection connection,
            string providerName,
            string connectionString,
            out int connectionTimeout,
            out Exception exception)
        {
            connectionTimeout = 0;
            exception = null;

            if (!providerName.Equals(
                    SystemDataOdbcTemplate,
                    StringComparison.OrdinalIgnoreCase) ||
                connectionString.IndexOf(
                    ConnectionTimeoutTemplate,
                    StringComparison.OrdinalIgnoreCase) == -1)
            {
                return true;
            }

            try
            {
                connectionTimeout = Convert.ToInt32(
                    connectionString
                        .ToUpperInvariant()
                        .Split(
                            new string[] { ConnectionTimeoutTemplate },
                            StringSplitOptions.RemoveEmptyEntries)[1]
                        .Split(';')[0],
                    CultureInfo.InvariantCulture);

                ((OdbcConnection)connection).ConnectionTimeout =
                    connectionTimeout;

                return true;
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
                return false;
            }
        }


        private string AddDbParameters(
            DbCommand command,
            int startIndex)
        {
            StringBuilder parameterLog = new StringBuilder();

            for (int i = startIndex; i < this.parameters.Count; ++i)
            {
                DbParameter parameter = this.parameters[i] as DbParameter;

                command.Parameters.Add(parameter);

                if (parameterLog.Length > 0)
                    parameterLog.Append(",");

                parameterLog.Append(parameter.ParameterName)
                    .Append("='")
                    .Append(parameter.Value)
                    .Append("'");
            }

            return parameterLog.ToString();
        }

        private static bool TryGetDbProviderFactory(
            string connectionStringConfigKey,
            out DbProviderFactory factory)
        {
            factory = null;

            var connectionStringConfig =
                ConfigurationManager.ConnectionStrings[connectionStringConfigKey];

            string providerName =
                connectionStringConfig != null
                    ? connectionStringConfig.ProviderName
                    : null;

            if (string.IsNullOrEmpty(providerName))
                return false;

            try
            {
                factory = DbProviderFactories.GetFactory(providerName);
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }


        private void SetDbParameterType(string parameterDbType, DbParameter result)
        {
            if (result is SqlParameter)
            {
                try
                {
                    ((SqlParameter)result).SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), parameterDbType, true);
                }
                catch (ArgumentException)
                {
                    try
                    {
                        result.DbType = (DbType)Enum.Parse(typeof(DbType), parameterDbType, true);
                    }
                    catch (ArgumentException exception)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError2, exception, this.function, 4, typeof(SqlDbType), typeof(DbType), typeof(string).Name));
                    }
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
                    try
                    {
                        result.DbType = (DbType)Enum.Parse(typeof(DbType), parameterDbType, true);
                    }
                    catch (ArgumentException exception)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError2, exception, this.function, 4, typeof(OdbcType), typeof(DbType), typeof(string).Name));
                    }
                }
            }
            else
            {
                try
                {
                    result.DbType = (DbType)Enum.Parse(typeof(DbType), parameterDbType, true);
                }
                catch (ArgumentException exception)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, exception, this.function, 4, typeof(string).Name, this.parameters[3].GetType().Name));
                }
            }
        }
    }
}
