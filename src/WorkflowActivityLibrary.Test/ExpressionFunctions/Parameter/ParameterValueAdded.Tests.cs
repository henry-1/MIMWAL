using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Parameter
{
    [TestClass]
    public class ParameterValueAddedTests
    {
        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_SingleMatchingInsert_ShouldReturnValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Insert);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueAdded",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.AreEqual(
                "Max",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_RequestParameterList_ShouldReturnExpectedResult()
        {
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Insert),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Insert),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "AccountName",
                    "max.mustermann",
                    UpdateMode.Insert)
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
                    "ParameterValueAdded",
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
        public void ExpressionFunction_Parameter_ParameterValueAdded_MultipleMatchingInserts_ShouldReturnValueList()
        {
            // Arrange
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Insert),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Insert),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Alexander",
                    UpdateMode.Insert)
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueAdded",
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
        public void ExpressionFunction_Parameter_ParameterValueAdded_ModifyParameter_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Modify);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueAdded",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_NonMatchingParameter_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann",
                    UpdateMode.Insert);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueAdded",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_NullOrEmptyRequestParameters_ShouldReturnNull()
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
                    "ParameterValueAdded",
                    requestParameters,
                    "MiddleName");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_NullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ParameterValueAdded",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "ParameterValueAdded",
                            parameter,
                            "MiddleName");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max",
                        UpdateMode.Insert),
                        "MiddleName"
                },
                new object[]
                {
                    new List<string>(),
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
                    "ParameterValueAdded",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_ParseModeWithNullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParameterValueAdded",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParameterValueAdded",
                        42,
                        "MiddleName");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                            "ParameterValueAdded",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_CreateRequestParameterMatchingAttribute_ShouldReturnValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueAdded",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.AreEqual(
                "Max",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValueAdded_CreateRequestParameterNonMatchingAttribute_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValueAdded",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }
    }
}