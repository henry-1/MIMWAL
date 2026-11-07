using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{

    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;


    [TestClass]
    public class AndTests
    {
        [TestMethod]
        public void ExpressionFunction_Boolean_And_TwoParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[] { "Both parameters true",        true,  true,  true },
                new object[] { "First parameter false",       false, true,  false },
                new object[] { "Second parameter false",      true,  false, false },
                new object[] { "Both parameters false",       false, false, false },
                new object[] { "First parameter null",        null,  true,  false },
                new object[] { "Second parameter null",       true,  null,  false },
                new object[] { "Both parameters null",        null,  null,  false }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];
                bool expected = (bool)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "And",
                    left,
                    right);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_And_ParseMode_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "And",
                true,
                true);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_And_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
            new object[0],
            new object[] { true },
            new object[] { true, true, false }
        };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "And",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_And_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
            new object[] { "Wrong first parameter",  "true", true },
            new object[] { "Wrong second parameter", true,   "false" },
            new object[] { "Both parameters wrong",  "true", "false" }
        };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];

                // Act / Assert
                try
                {
                    ExpressionFunctionTestHelper.AssertThrows(
                        delegate
                        {
                            ExpressionFunctionTestHelper.Run(
                                "And",
                                left,
                                right);
                        },
                        typeof(InvalidFunctionFormatException),
                        null);
                }
                catch (AssertFailedException ex)
                {
                    Assert.Fail(
                        "Failed test case: " +
                        description +
                        ". " +
                        ex.Message);
                }
            }
        }
    }
}

