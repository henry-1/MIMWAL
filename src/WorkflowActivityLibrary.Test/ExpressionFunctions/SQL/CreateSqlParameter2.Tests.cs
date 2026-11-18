using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.SQL
{
    [TestClass]
    public class CreateSqlParameter2Tests
    {
        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_ValidParameters_ShouldReturnParameter()
        {
            // Act
            DbParameter result = (DbParameter)ExpressionFunctionTestHelper.Run(
                "CreateSqlParameter2",
                "TestSqlConnection",
                "@Name",
                "Input",
                "NVarChar");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SqlParameter));
            Assert.AreEqual("@Name", result.ParameterName);
            Assert.AreEqual(System.Data.ParameterDirection.Input, result.Direction);
            Assert.AreEqual(SqlDbType.NVarChar, ((SqlParameter)result).SqlDbType);
            Assert.AreEqual(DBNull.Value, result.Value);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_ValidParametersWithSizeAndValue_ShouldReturnParameter()
        {
            // Act
            DbParameter result = (DbParameter)ExpressionFunctionTestHelper.Run(
                "CreateSqlParameter2",
                "TestSqlConnection",
                "@Name",
                "Input",
                "NVarChar",
                100,
                "Test");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("@Name", result.ParameterName);
            Assert.AreEqual(ParameterDirection.Input, result.Direction);
            Assert.AreEqual(SqlDbType.NVarChar, ((SqlParameter)result).SqlDbType);
            Assert.AreEqual(100, result.Size);
            Assert.AreEqual("Test", result.Value);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_ValidValues_ShouldSetValue()
        {
            object[] testCases =
            {
                "Value",
                42,
                true,
                123.45
            };

            foreach (object value in testCases)
            {
                // Act
                DbParameter result = (DbParameter)ExpressionFunctionTestHelper.Run(
                    "CreateSqlParameter2",
                    "TestSqlConnection",
                    "@Value",
                    "Input",
                    "Variant",
                    0,
                    value);

                // Assert
                Assert.AreEqual(value, result.Value);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "CreateSqlParameter2",
                "DoesNotNeedToExist",
                "@Name",
                "Input",
                "NVarChar");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "TestSqlConnection" },
                new object[] { "TestSqlConnection", "@Name" },
                new object[] { "TestSqlConnection", "@Name", "Input" },
                new object[] { "TestSqlConnection", "@Name", "Input", "NVarChar", 100, "Value", "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CreateSqlParameter2",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_NullRequiredParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { null, "@Name", "Input", "NVarChar" },
                new object[] { "TestSqlConnection", null, "Input", "NVarChar" },
                new object[] { "TestSqlConnection", "@Name", null, "NVarChar" },
                new object[] { "TestSqlConnection", "@Name", "Input", null },
                new object[] { "TestSqlConnection", "@Name", "Input", "NVarChar", null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CreateSqlParameter2",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_NullValue_ShouldSetDBNullValue()
        {
            // Act
            DbParameter result = (DbParameter)ExpressionFunctionTestHelper.Run(
                "CreateSqlParameter2",
                "TestSqlConnection",
                "@Name",
                "Input",
                "NVarChar",
                100,
                null);

            // Assert
            Assert.AreEqual(DBNull.Value, result.Value);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 42, "@Name", "Input", "NVarChar" },
                new object[] { "TestSqlConnection", 42, "Input", "NVarChar" },
                new object[] { "TestSqlConnection", "@Name", 42, "NVarChar" },
                new object[] { "TestSqlConnection", "@Name", "Input", 42 },
                new object[] { "TestSqlConnection", "@Name", "Input", "NVarChar", "100" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CreateSqlParameter2",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_InvalidDirection_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "CreateSqlParameter2",
                        "TestSqlConnection",
                        "@Name",
                        "InvalidDirection",
                        "NVarChar");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_InvalidDbType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "CreateSqlParameter2",
                        "TestSqlConnection",
                        "@Name",
                        "Input",
                        "DoesNotExist");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter2_DbType_ShouldFallbackToDbType()
        {
            // Act
            DbParameter result = (DbParameter)ExpressionFunctionTestHelper.Run(
                "CreateSqlParameter2",
                "TestSqlConnection",
                "@Name",
                "Input",
                "AnsiString");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(DbType.AnsiString, result.DbType);
        }
    }
}
