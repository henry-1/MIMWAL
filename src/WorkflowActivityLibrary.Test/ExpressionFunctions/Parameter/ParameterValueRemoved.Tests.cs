using System;
using System.Collections.Generic;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Parameter
{
    [TestClass]
    public class ParameterValueRemovedTests
    {
        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_StringParameter_ShouldReturnValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Remove);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueRemoved",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.AreEqual(
                "Max",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_SingleMatchingRemove_ShouldReturnValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Remove);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueRemoved",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.AreEqual(
                "Max",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_RequestParameterList_ShouldReturnExpectedResult()
        {
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Remove),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Remove),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "AccountName",
                    "max.mustermann",
                    UpdateMode.Remove)
            };

            object[][] testCases =
            {
                new object[]
                {
                    "First parameter matches",
                    "MiddleName",
                    "Max"
                },
                new object[]
                {
                    "Middle parameter matches",
                    "DisplayName",
                    "Max Mustermann"
                },
                new object[]
                {
                    "Last parameter matches",
                    "AccountName",
                    "max.mustermann"
                },
                new object[]
                {
                    "No parameter matches",
                    "Email",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string attributeName = (string)testCase[1];
                object expected = testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParameterValueRemoved",
                    requestParameters,
                    attributeName);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_MultipleMatchingRemoves_ShouldReturnValueList()
        {
            // Arrange
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Remove),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Remove),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Alexander",
                    UpdateMode.Remove)
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueRemoved",
                requestParameters,
                "MiddleName");

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<object>));

            List<object> resultList =
                (List<object>)result;

            CollectionAssert.AreEqual(
                new object[]
                {
                    "Max",
                    "Alexander"
                },
                resultList.ToArray());
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_ModifyParameter_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Modify);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueRemoved",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_InsertParameter_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Insert);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueRemoved",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_NonMatchingParameter_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Remove);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueRemoved",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_NullOrEmptyRequestParameters_ShouldReturnNull()
        {
            object[] testCases =
            {
                null,
                new List<string>()
            };

            foreach (object requestParameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParameterValueRemoved",
                    requestParameters,
                    "MiddleName");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_NullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ParameterValueRemoved",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "ParameterValueRemoved",
                            parameter,
                            "MiddleName");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max",
                        UpdateMode.Remove),
                    "MiddleName"
                },

                new object[]
                {
                    new List<string>
                    {
                        ExpressionFunctionTestHelper.CreateRequestParameter(
                            "MiddleName",
                            "Max",
                            UpdateMode.Remove)
                    },
                    "MiddleName"
                },

                new object[]
                {
                    null,
                    "MiddleName"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ParameterValueRemoved",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_ParseModeWithNullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParameterValueRemoved",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParameterValueRemoved",
                        42,
                        "MiddleName");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueRemoved_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    new List<string>()
                },

                new object[]
                {
                    new List<string>(),
                    "MiddleName",
                    "Extra"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ParameterValueRemoved",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}