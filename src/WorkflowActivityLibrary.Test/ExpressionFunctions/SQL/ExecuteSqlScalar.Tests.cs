using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.SQL
{
    [TestClass]
    public class ExecuteSqlScalarTests
    {
        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ExecuteSqlScalar",
                "DoesNotNeedToExist",
                "SELECT 1");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "ExecuteSqlScalar",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_NullRequiredParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { null, "SELECT 1" },
                new object[] { "TestSqlConnection", null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlScalar",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_InvalidRequiredParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 42, "SELECT 1" },
                new object[] { "TestSqlConnection", 42 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlScalar",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_NullDbParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ExecuteSqlScalar",
                        "TestSqlConnection",
                        "SELECT 1",
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_InvalidDbParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ExecuteSqlScalar",
                        "TestSqlConnection",
                        "SELECT 1",
                        "NotAParameter");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_ParseModeWithDbParameter_ShouldReturnNull()
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Value";
            parameter.Value = 42;

            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ExecuteSqlScalar",
                "DoesNotNeedToExist",
                "SELECT @Value",
                parameter);

            // Assert
            Assert.IsNull(result);
        }


        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_DenySqlTextCommands_ShouldThrowInvalidFunctionFormatException()
        {
            try
            {
                ExpressionFunctionTestHelper.SetSqlTextCommandDeniedSettingOverride("true");

                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ExecuteSqlScalar",
                            "TestSqlConnection",
                            "SELECT NumberValue FROM dbo.WalExpressionFunctionTest WHERE Id = 1");
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
        public void ExpressionFunction_SQL_ExecuteSqlScalar_SelectCount_ShouldReturnOne()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ExecuteSqlScalar",
                "TestSqlConnection",
                "SELECT COUNT(*) FROM dbo.WalExpressionFunctionTest");

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_SelectNumberValue_ShouldReturnValue()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ExecuteSqlScalar",
                "TestSqlConnection",
                "SELECT NumberValue FROM dbo.WalExpressionFunctionTest WHERE Id = 1");

            // Assert
            Assert.AreEqual(42, result);
        }


        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_SelectTextValue_ShouldReturnValue()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ExecuteSqlScalar",
                "TestSqlConnection",
                "SELECT TextValue FROM dbo.WalExpressionFunctionTest WHERE Id = 1");

            // Assert
            Assert.AreEqual("Test Value", result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_SelectNull_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ExecuteSqlScalar",
                "TestSqlConnection",
                "SELECT NULL");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_WithParameter_ShouldReturnMatchingValue()
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Id";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Value = 1;

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ExecuteSqlScalar",
                "TestSqlConnection",
                "SELECT TextValue FROM dbo.WalExpressionFunctionTest WHERE Id = @Id",
                parameter);

            // Assert
            Assert.AreEqual("Test Value", result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_ExecuteSqlScalar_NoMatchingRow_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ExecuteSqlScalar",
                "TestSqlConnection",
                "SELECT TextValue FROM dbo.WalExpressionFunctionTest WHERE Id = -1");

            // Assert
            Assert.IsNull(result);
        }

        #endregion real data tests

    }
}
