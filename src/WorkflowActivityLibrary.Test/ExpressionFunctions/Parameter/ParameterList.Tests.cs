using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Parameter
{
    [TestClass]
    public class ParametersListTests
    {
        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_RequestParameters_ShouldReturnExpectedProperties()
        {
            string middleNameParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter("MiddleName", "Max");

            string displayNameParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter("DisplayName", "Test User");

            string accountNameParameter =
                ExpressionFunctionTestHelper.CreateRequestParameter("AccountName", "testuser");

            object[][] testCases =
            {
                new object[]
                {
                    "Single parameter",
                    new List<string>
                    {
                        middleNameParameter
                    },
                    new string[]
                    {
                        "MiddleName"
                    }
                },
                new object[]
                {
                    "Multiple parameters",
                    new List<string>
                    {
                        middleNameParameter,
                        displayNameParameter,
                        accountNameParameter
                    },
                    new string[]
                    {
                        "MiddleName",
                        "DisplayName",
                        "AccountName"
                    }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                string[] expected = (string[])testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersList",
                    parameter);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(List<string>),
                    "Failed test case: " + description);

                List<string> resultList =
                    (List<string>)result;

                CollectionAssert.AreEqual(
                    expected,
                    resultList.ToArray(),
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_SingleRequestParameter_ShouldReturnExpectedProperty()
        {
            // Arrange
            string parameter =
                ExpressionFunctionTestHelper.CreateRequestParameter(
                    "MiddleName",
                    "Max");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersList",
                parameter);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            List<string> resultList =
                (List<string>)result;

            CollectionAssert.AreEqual(
                new string[] { "MiddleName" },
                resultList.ToArray());
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_EmptyOrNullPropertyName_ShouldNotAddProperty()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Empty property name",
                    new List<string>
                    {
                        ExpressionFunctionTestHelper.CreateRequestParameter(
                            string.Empty,
                            "Test")
                    }
                },
                new object[]
                {
                    "Empty property mixed with valid property",
                    new List<string>
                    {
                        ExpressionFunctionTestHelper.CreateRequestParameter(
                            string.Empty,
                            "Test"),

                        ExpressionFunctionTestHelper.CreateRequestParameter(
                            "DisplayName",
                            "Test User")
                    }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                List<string> parameters =
                    (List<string>)testCase[1];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "ParametersList",
                    parameters);

                // Assert
                List<string> resultList =
                    (List<string>)result;

                if (parameters.Count == 1)
                {
                    Assert.AreEqual(
                        0,
                        resultList.Count,
                        "Failed test case: " + description);
                }
                else
                {
                    CollectionAssert.AreEqual(
                        new string[] { "DisplayName" },
                        resultList.ToArray(),
                        "Failed test case: " + description);
                }
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_Null_ShouldReturnEmptyList()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersList",
                (object)null);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            Assert.AreEqual(
                0,
                ((List<string>)result).Count);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_EmptyList_ShouldReturnEmptyList()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ParametersList",
                new List<string>());

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            Assert.AreEqual(
                0,
                ((List<string>)result).Count);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_ParseMode_ShouldReturnEmptyList()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "ParametersList",
                "Test");

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<string>));

            Assert.AreEqual(
                0,
                ((List<string>)result).Count);
        }


        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
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
                            "ParametersList",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ParametersList",
                        42);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Parameter_ParametersList_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[]
                {
                    new List<string>(),
                    new List<string>()
                }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ParametersList",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        
    }
}