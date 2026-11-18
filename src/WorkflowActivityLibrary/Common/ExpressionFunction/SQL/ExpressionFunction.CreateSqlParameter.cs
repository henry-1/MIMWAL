using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Data;
using System.Data.Common;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
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
                if (!this.HasParameterCountBetween(3, 4))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, 3, 4, this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter = this.parameters[0];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 1, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[1];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 2, parameterType.Name, parameter.GetType().Name));

                if (this.parameters.Count == 4)
                {
                    parameter = this.parameters[3];

                    if (parameter == null)
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 4));

                    if (!this.VerifyType(parameter, parameterType))
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 4, parameterType.Name, parameter.GetType().Name));
                }

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string connectionStringConfigKey = this.parameters[0] as string;

                DbProviderFactory factory;
                if (!TryGetDbProviderFactory(connectionStringConfigKey, out factory))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));


                string parameterName = this.parameters[1] as string;

                if (!parameterName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError, this.function, 2, "@" + parameterName, parameterName));

                ParameterDirection parameterDirection = ParameterDirection.Input;

                if (this.parameters.Count == 4)
                {
                    try
                    {
                        parameterDirection = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), this.parameters[3] as string, true);
                    }
                    catch (ArgumentException exception)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameterInvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, exception, this.function, 4, parameterType.Name, this.parameters[3].GetType().Name));
                    }
                }

                DbParameter result = factory.CreateParameter();
                result.ParameterName = parameterName;
                result.Direction = parameterDirection;
                result.Value = this.parameters[2] == null ? DBNull.Value : this.parameters[2];

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCreateSqlParameter, "CreateSqlParameter({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCreateSqlParameter, EvaluationModeTemplate, this.mode);
            }
        }
    }
}
