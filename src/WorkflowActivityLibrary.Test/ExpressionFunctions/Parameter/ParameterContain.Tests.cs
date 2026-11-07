using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Parameter
{
    [TestClass]
    public class ParametersContainTests
    {
        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_NullOrEmptyParameters_ShouldReturnFalse()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Null parameters",
                    null,
                    "AccountName"
                },
                new object[]
                {
                    "Empty list",
                    new List<string>(),
                    "AccountName"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    "AccountName"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object requestParameters = testCase[1];
                string attributeName = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
                    requestParameters,
                    attributeName);

                // Assert
                Assert.AreEqual(
                    false,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_OldStyleXml_ShouldReturnExpectedResult()
        {
            string xml =
                "<table>" +
                    "<tr>" +
                        "<td class='AttributeStyle'>AccountName</td>" +
                        "<td class='NewValueStyle'>TestUser</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td class='AttributeStyle'>DisplayName</td>" +
                        "<td class='NewValueStyle'>Test User</td>" +
                    "</tr>" +
                "</table>";

            object[][] testCases =
            {
                new object[]
                {
                    "First attribute exists",
                    xml,
                    "AccountName",
                    true
                },
                new object[]
                {
                    "Second attribute exists",
                    xml,
                    "DisplayName",
                    true
                },
                new object[]
                {
                    "Attribute does not exist",
                    xml,
                    "Email",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string requestParameters = (string)testCase[1];
                string attributeName = (string)testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
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
        public void ExpressionFunction_Parameter_ParametersContain_InvalidOldStyleXml_ShouldUseStringFallback()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Attribute exists",
                    "<invalid>AccountName</invalid",
                    "AccountName",
                    true
                },
                new object[]
                {
                    "Attribute does not exist",
                    "<invalid>DisplayName</invalid",
                    "AccountName",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string requestParameters = (string)testCase[1];
                string attributeName = (string)testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
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
        public void ExpressionFunction_Parameter_ParametersContain_NullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ParametersContain",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "ParametersContain",
                            parameter,
                            "AccountName");
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_InvalidSecondParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object()
            };

            foreach (object attributeName in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ParametersContain",
                            new List<string>(),
                            attributeName);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_ParseMode_ShouldReturnFalse()
        {
            object[][] testCases =
            {
                new object[]
                {
                    new List<string>(),
                    "AccountName"
                },
                new object[]
                {
                    null,
                    "AccountName"
                },
                new object[]
                {
                    string.Empty,
                    "AccountName"
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ParametersContain",
                    parameters);

                // Assert
                Assert.AreEqual(false, result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_ParseModeWithNullAttributeName_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParametersContain",
                        new List<string>(),
                        null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParametersContain",
                        42,
                        "AccountName");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
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
                    "AccountName",
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
                            "ParametersContain",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
        
        /*
        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_UpdateRequestParameter_ShouldReturnExpectedResult()
        {
            // Arrange
            string requestParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max");

            object[][] testCases =
            {
                new object[]
                {
                    "Matching property",
                    "MiddleName",
                    true
                },
                new object[]
                {
                    "Different property",
                    "DisplayName",
                    false
                },
                new object[]
                {
                    "Empty property",
                    string.Empty,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string attributeName = (string)testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
                    requestParameter,
                    attributeName);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

    */

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_RequestParameterList_ShouldReturnExpectedResult()
        {
            // Arrange
            string middleNameParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max");

            string displayNameParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "DisplayName",
                    "Test User");

            List<string> requestParameters = new List<string>
            {
                middleNameParameter,
                displayNameParameter
            };

            object[][] testCases =
            {
                new object[]
                {
                    "First item matches",
                    "MiddleName",
                    true
                },
                new object[]
                {
                    "Second item matches",
                    "DisplayName",
                    true
                },
                new object[]
                {
                    "No item matches",
                    "AccountName",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string attributeName = (string)testCase[1];
                bool expected = (bool)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
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
        public void ExpressionFunction_Parameter_ParametersContain_CreateRequestParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Matching property",
                    "MiddleName",
                    "Max",
                    "MiddleName",
                    true
                },
                new object[]
                {
                    "Different property",
                    "MiddleName",
                    "Max",
                    "DisplayName",
                    false
                },
                new object[]
                {
                    "Empty property name",
                    string.Empty,
                    "Max",
                    "MiddleName",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string propertyName = (string)testCase[1];
                object value = testCase[2];
                string searchProperty = (string)testCase[3];
                bool expected = (bool)testCase[4];

                // Two arguments creates a real CreateRequestParameter.
                string requestParameter =
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        propertyName,
                        value);

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
                    requestParameter,
                    searchProperty);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersContain_UpdateRequestParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Matching property",
                    "MiddleName",
                    "Max",
                    "MiddleName",
                    true
                },
                new object[]
                {
                    "Different property",
                    "MiddleName",
                    "Max",
                    "DisplayName",
                    false
                },
                new object[]
                {
                    "Empty property name",
                    string.Empty,
                    "Max",
                    "MiddleName",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string propertyName = (string)testCase[1];
                object value = testCase[2];
                string searchProperty = (string)testCase[3];
                bool expected = (bool)testCase[4];

                // Three arguments creates an UpdateRequestParameter.
                string requestParameter =
                    ExpressionFunctionTestHelper.CreateRequestParameter(
                        propertyName,
                        value,
                        UpdateMode.Modify);

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersContain",
                    requestParameter,
                    searchProperty);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

    }
}