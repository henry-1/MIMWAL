using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Parameter
{
    [TestClass]
    public class ParametersTableTests
    {
        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_SingleParameter_ShouldReturnExpectedTable()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Modify);

            string expected =
                "<tr>" +
                "<td class=\"AttributeStyle\">MiddleName</td>" +
                "<td class=\"AttributeModificationStyle\">Modify</td>" +
                "<td class=\"NewValueStyle\">Max</td>" +
                "</tr>";

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersTable",
                requestParameter);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_MultipleParameters_ShouldReturnExpectedTable()
        {
            // Arrange
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Modify),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Modify)
            };

            string expected =
                "<tr>" +
                "<td class=\"AttributeStyle\">MiddleName</td>" +
                "<td class=\"AttributeModificationStyle\">Modify</td>" +
                "<td class=\"NewValueStyle\">Max</td>" +
                "</tr>" +
                "<tr>" +
                "<td class=\"AttributeStyle\">DisplayName</td>" +
                "<td class=\"AttributeModificationStyle\">Modify</td>" +
                "<td class=\"NewValueStyle\">Max Mustermann</td>" +
                "</tr>";

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersTable",
                requestParameters);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_SpecialCharacters_ShouldReturnHtmlEncodedValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max & Moritz",
                    UpdateMode.Modify);

            string expected =
                "<tr>" +
                "<td class=\"AttributeStyle\">DisplayName</td>" +
                "<td class=\"AttributeModificationStyle\">Modify</td>" +
                "<td class=\"NewValueStyle\">Max &amp; Moritz</td>" +
                "</tr>";

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersTable",
                requestParameter);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_NullOrEmptyParameter_ShouldReturnEmptyString()
        {
            object[] testCases =
            {
                null,
                new List<string>()
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersTable",
                    parameter);

                // Assert
                Assert.AreEqual(
                    string.Empty,
                    result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_ParseMode_ShouldReturnEmptyString()
        {
            object[] testCases =
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max"),

                new List<string>
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max")
                },

                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ParametersTable",
                    parameter);

                // Assert
                Assert.AreEqual(
                    string.Empty,
                    result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object(),
                new List<int> { 1, 2, 3 }
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ParametersTable",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParametersTable",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max"),

                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "DisplayName",
                        "Max Mustermann")
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ParametersTable",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_CreateRequestParameter_ShouldReturnExpectedTable()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max");

            string expected =
                "<tr>" +
                "<td class=\"AttributeStyle\">MiddleName</td>" +
                "<td class=\"AttributeModificationStyle\">Create</td>" +
                "<td class=\"NewValueStyle\">Max</td>" +
                "</tr>";

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersTable",
                requestParameter);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_CreateRequestParameterWithNullValue_ShouldReturnExpectedTable()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    null);

            string expected =
                "<tr>" +
                "<td class=\"AttributeStyle\">MiddleName</td>" +
                "<td class=\"AttributeModificationStyle\">Create</td>" +
                "<td class=\"NewValueStyle\"></td>" +
                "</tr>";

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersTable",
                requestParameter);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersTable_CreateRequestParameterWithSpecialCharacters_ShouldReturnHtmlEncodedValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max & Moritz");

            string expected =
                "<tr>" +
                "<td class=\"AttributeStyle\">DisplayName</td>" +
                "<td class=\"AttributeModificationStyle\">Create</td>" +
                "<td class=\"NewValueStyle\">Max &amp; Moritz</td>" +
                "</tr>";

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersTable",
                requestParameter);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }
    }
}