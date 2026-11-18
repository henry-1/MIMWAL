using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.SQL
{
    [TestClass]
    public class ExecuteSqlNonQueryTests
    {
        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ExecuteSqlNonQuery",
                "DoesNotNeedToExist",
                "UPDATE dbo.WalExpressionFunctionTest SET NumberValue = 42");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "TestSqlConnection" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlNonQuery",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_NullRequiredParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { null, "UPDATE dbo.WalExpressionFunctionTest SET NumberValue = 42" },
                new object[] { "TestSqlConnection", null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlNonQuery",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_InvalidRequiredParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 42, "UPDATE dbo.WalExpressionFunctionTest SET NumberValue = 42" },
                new object[] { "TestSqlConnection", 42 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlNonQuery",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_NullDbParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ExecuteSqlNonQuery",
                        "TestSqlConnection",
                        "UPDATE dbo.WalExpressionFunctionTest SET NumberValue = 42",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_InvalidDbParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ExecuteSqlNonQuery",
                        "TestSqlConnection",
                        "UPDATE dbo.WalExpressionFunctionTest SET NumberValue = 42",
                        "NotAParameter");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_ParseModeWithDbParameter_ShouldReturnNull()
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@NumberValue";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Value = 42;

            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ExecuteSqlNonQuery",
                "DoesNotNeedToExist",
                "UPDATE dbo.WalExpressionFunctionTest SET NumberValue = @NumberValue",
                parameter);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_DenySqlTextCommands_ShouldThrowInvalidFunctionFormatException()
        {
            try
            {
                ExpressionFunctionTestHelper.SetSqlTextCommandDeniedSettingOverride("true");

                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlNonQuery",
                            "TestSqlConnection",
                            "DELETE FROM dbo.WalExpressionFunctionTest WHERE Id = 999");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
            finally
            {
                ExpressionFunctionTestHelper.SetSqlTextCommandDeniedSettingOverride(null);
            }
        }

        #region real data tests

        /*
        
        // a database and a table is required
        
            CREATE TABLE dbo.WalExpressionFunctionTest
            (
                Id INT NOT NULL PRIMARY KEY,
                TextValue NVARCHAR(100) NULL,
                NumberValue INT NULL,
                BooleanValue BIT NULL
            );

            INSERT INTO dbo.WalExpressionFunctionTest
                (Id, TextValue, NumberValue, BooleanValue)
            VALUES
                (1, N'Test Value', 42, 1);

        // connection string must be set in app.config

            <connectionStrings>
              <add name="TestSqlConnection"
                   connectionString="Data Source=YOURSERVER;Initial Catalog=YOURDATABASE;Integrated Security=True"
                   providerName="System.Data.SqlClient" />
            </connectionStrings>
        */

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlNonQuery_Insert_ShouldReturnOneRowAffected()
        {
            try
            {
                // Arrange
                ExpressionFunctionTestHelper.Run(
                    "ExecuteSqlNonQuery",
                    "TestSqlConnection",
                    "DELETE FROM dbo.WalExpressionFunctionTest WHERE Id = @Id",
                    CreateParameter("@Id", SqlDbType.Int, 9001, 0));

                // Act
                Dictionary<string, object> result =
                (Dictionary<string, object>)ExpressionFunctionTestHelper.Run(
                    "ExecuteSqlNonQuery",
                    new object[]
                    {
                        "TestSqlConnection",
                        "INSERT INTO dbo.WalExpressionFunctionTest (Id, TextValue, NumberValue, BooleanValue) VALUES (@Id, @TextValue, @NumberValue, @BooleanValue)",
                        CreateParameter("@Id", SqlDbType.Int, 9001, 0),
                        CreateParameter("@TextValue", SqlDbType.NVarChar, "Inserted Value", 100),
                        CreateParameter("@NumberValue", SqlDbType.Int, 100, 0),
                        CreateParameter("@BooleanValue", SqlDbType.Bit, true, 0)
                    });

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(1, result["@RowsAffected"]);
            }
            finally
            {
                ExpressionFunctionTestHelper.Run(
                    "ExecuteSqlNonQuery",
                    new object[]
                    {
                        "TestSqlConnection",
                        "DELETE FROM dbo.WalExpressionFunctionTest WHERE Id = @Id",
                        CreateParameter("@Id", SqlDbType.Int, 9001, 0)
                    });
            }
        }

        #endregion real data tests

        private static SqlParameter CreateParameter(
            string name,
            SqlDbType type,
            object value,
            int size)
        {
            SqlParameter parameter = size > 0
                ? new SqlParameter(name, type, size)
                : new SqlParameter(name, type);

            parameter.Value = value;
            return parameter;
        }


    }
}
