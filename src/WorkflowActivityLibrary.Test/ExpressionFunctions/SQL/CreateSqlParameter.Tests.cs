using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System.Data.Common;


namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.SQL
{
    [TestClass]
    public class CreateSqlParameterTests
    {
        public object ParameterDirection { get; private set; }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "CreateSqlParameter",
                "DoesNotNeedToExist",
                "@Name",
                "Value");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { "TestSqlConnection" },
                new object[] { "TestSqlConnection", "@Name" },
                new object[] { "TestSqlConnection", "@Name", "Value", "Input", "Extra" }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CreateSqlParameter",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter_NullRequiredParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { null, "@Name", "Value" },
                new object[] { "TestSqlConnection", null, "Value" },
                new object[] { "TestSqlConnection", "@Name", "Value", null }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CreateSqlParameter",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 42, "@Name", "Value" },
                new object[] { "TestSqlConnection", 42, "Value" },
                new object[] { "TestSqlConnection", "@Name", "Value", 42 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "CreateSqlParameter",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter_ValidValues_ShouldReturnInputParameter()
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
                    "CreateSqlParameter",
                    "TestSqlConnection",
                    "@Name",
                    value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("@Name", result.ParameterName);
                Assert.AreEqual(value, result.Value);
                Assert.AreEqual(System.Data.ParameterDirection.Input, result.Direction);
            }
        }

        [TestMethod]
        public void ExpressionFunction_SQL_CreateSqlParameter_ValidDirection_ShouldReturnParameterWithSpecifiedDirection()
        {
            // Act
            DbParameter result = (DbParameter)ExpressionFunctionTestHelper.Run(
                "CreateSqlParameter",
                "TestSqlConnection",
                "@Name",
                "Value",
                "Output");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("@Name", result.ParameterName);
            Assert.AreEqual("Value", result.Value);
            Assert.AreEqual(System.Data.ParameterDirection.Output, result.Direction);
        }
    }
}
