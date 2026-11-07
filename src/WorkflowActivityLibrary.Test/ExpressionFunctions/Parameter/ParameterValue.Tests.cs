using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Parameter
{
    [TestClass]
    public class ParameterValueTests
    {
        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_SingleMatchingParameter_ShouldReturnValue()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValue",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.AreEqual(
                "Max",
                result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_RequestParameterList_ShouldReturnExpectedResult()
        {
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max"),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann"),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "AccountName",
                    "max.mustermann")
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
                    "ParameterValue",
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
        public void ExpressionFunction_Parameter_ParameterValue_MultipleMatchingParameters_ShouldReturnValueList()
        {
            // Arrange
            List<string> requestParameters = new List<string>
            {
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max"),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Max Mustermann"),

                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Alexander")
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValue",
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
        public void ExpressionFunction_Parameter_ParameterValue_EmptyRequestParameterList_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValue",
                new List<string>(),
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_NullRequestParameters_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValue",
                null,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_NullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            object[] requestParameterCases =
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

            foreach (object requestParameters in requestParameterCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ParameterValue",
                            requestParameters,
                            null);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "ParameterValue",
                            parameter,
                            "MiddleName");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_SecondParameterValue_ShouldBeConvertedToStringForComparison()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "42",
                    "Value");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValue",
                requestParameter,
                42);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_ParseMode_ShouldReturnNull()
        {
            object[][] testCases =
            {
                new object[]
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max"),
                    "MiddleName"
                },
                new object[]
                {
                    new List<string>
                    {
                        ExpressionFunctionTestHelper.CreateRequestParameter(
                            "MiddleName",
                            "Max")
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
                    "ParameterValue",
                    parameters);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_ParseModeWithNullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParameterValue",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_ParseModeWithInvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParameterValue",
                        42,
                        "MiddleName");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max")
                },

                new object[]
                {
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        "MiddleName",
                        "Max"),
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
                            "ParameterValue",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_UpdateModeInsert_ShouldReturnNull()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max",
                    UpdateMode.Insert);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParameterValue",
                requestParameter,
                "MiddleName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParameterValue_UpdateRequestParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Matching property with value",
                    "MiddleName",
                    "Max",
                    UpdateMode.Modify,
                    "MiddleName",
                    "Max"
                },
                new object[]
                {
                    "Non-matching property",
                    "DisplayName",
                    "Max",
                    UpdateMode.Modify,
                    "MiddleName",
                    null
                },
                new object[]
                {
                    "Matching property with null value",
                    "MiddleName",
                    null,
                    UpdateMode.Modify,
                    "MiddleName",
                    null
                },
                new object[]
                {
                    "Matching property with Insert mode",
                    "MiddleName",
                    "Max",
                    UpdateMode.Insert,
                    "MiddleName",
                    null
                },
                new object[]
                {
                    "Matching property with Remove mode",
                    "MiddleName",
                    "Max",
                    UpdateMode.Remove,
                    "MiddleName",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string propertyName = (string)testCase[1];
                object value = testCase[2];
                UpdateMode mode = (UpdateMode)testCase[3];
                string requestedProperty = (string)testCase[4];
                object expected = testCase[5];

                string requestParameter =
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        propertyName,
                        value,
                        mode);

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParameterValue",
                    requestParameter,
                    requestedProperty);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }
    }
}