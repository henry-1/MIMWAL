using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
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
                if (!this.HasParameterCountBetween(4, 6))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterMinMaxCountError, this.function, 4, 6, this.parameters.Count));

                Type parameterType = typeof(string);
                object parameter = this.parameters[0];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 1, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[1];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 2, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[2];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 3));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 3, parameterType.Name, parameter.GetType().Name));

                parameter = this.parameters[3];

                if (parameter == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 4));

                if (!this.VerifyType(parameter, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 4, parameterType.Name, parameter.GetType().Name));

                if (this.parameters.Count > 4)
                {
                    parameter = this.parameters[4];
                    parameterType = typeof(int);

                    if (parameter == null)
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 5));

                    if (!this.VerifyType(parameter, parameterType))
                        throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, this.function, 5, parameterType.Name, parameter.GetType().Name));
                }

                if (this.mode == EvaluationMode.Parse)
                    return null;

                string connectionStringConfigKey = this.parameters[0] as string;

                DbProviderFactory factory;
                if (!TryGetDbProviderFactory(connectionStringConfigKey, out factory))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2NullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidConfigKeyConfiguration, this.function, connectionStringConfigKey));

                string parameterName = this.parameters[1] as string;

                if (!parameterName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterError, this.function, 2, "@" + parameterName, parameterName));

                ParameterDirection parameterDirection;

                try
                {
                    parameterDirection = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), this.parameters[2] as string, true);
                }
                catch (ArgumentException exception)
                {
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionCreateSqlParameter2InvalidFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterTypeError, exception, this.function, 3, typeof(string).Name, this.parameters[2].GetType().Name));
                }

                string parameterDbType = this.parameters[3] as string;

                int parameterSize = 0;
                if (this.parameters.Count > 4)
                    parameterSize = Convert.ToInt32(this.parameters[4], CultureInfo.InvariantCulture);

                object parameterValue = null;
                if (this.parameters.Count > 5)
                    parameterValue = this.parameters[5];

                DbParameter result = factory.CreateParameter();
                result.ParameterName = parameterName;
                result.Direction = parameterDirection;

                this.SetDbParameterType(parameterDbType, result);

                if (parameterSize != 0)
                    result.Size = parameterSize;

                result.Value = parameterValue == null ? DBNull.Value : parameterValue;

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionCreateSqlParameter2, "CreateSqlParameter2({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionCreateSqlParameter2, EvaluationModeTemplate, this.mode);
            }
        }

    }
}
