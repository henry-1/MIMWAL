using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{    
    [TestClass]
    public class ExpressionFunctionEqTests
    {
        [TestMethod]
        public void ExpressionFunction_Boolean_Eq_TwoParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
            // Strings
            new object[] { "Equal strings",                       "Test",  "Test",  true },
            new object[] { "Different strings",                   "Test",  "Other", false },
            new object[] { "Strings differ only by case",         "Test",  "TEST",  true },
            new object[] { "Empty strings",                       "",      "",      true },

            // Null
            new object[] { "Both parameters null",                null,    null,    true },
            new object[] { "Null and empty string",               null,    "",      true },
            new object[] { "Empty string and null",               "",      null,    true },
            new object[] { "Null and non-empty string",           null,    "Test",  false },
            new object[] { "Non-empty string and null",           "Test",  null,    false },
            new object[] { "Null and integer",                    null,    0,       false },
            new object[] { "Integer and null",                    0,       null,    false },

            // Numeric
            new object[] { "Equal integers",                      42,      42,      true },
            new object[] { "Different integers",                  42,      43,      false },
            new object[] { "Equal longs",                         42L,     42L,     true },
            new object[] { "Different longs",                     42L,     43L,     false },
            new object[] { "Equivalent integer and long",         42,      42L,     true },
            new object[] { "Equivalent long and integer",         42L,     42,      true },
            new object[] { "Different integer and long",          42,      43L,     false },

            // Boolean
            new object[] { "Equal booleans true",                 true,    true,    true },
            new object[] { "Equal booleans false",                false,   false,   true },
            new object[] { "Different booleans",                  true,    false,   false },

            // Different types
            new object[] { "String and integer",                  "42",    42,      false }
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
                    "Eq",
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
        public void ExpressionFunction_Boolean_Eq_WithCaseSensitiveParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
            new object[] { "Equal strings, case-sensitive",           "Test", "Test",  true,  true },
            new object[] { "Different case, case-sensitive",          "Test", "TEST",  true,  false },
            new object[] { "Different strings, case-sensitive",       "Test", "Other", true,  false },

            new object[] { "Equal strings, case-insensitive",         "Test", "Test",  false, true },
            new object[] { "Different case, case-insensitive",        "Test", "TEST",  false, true },
            new object[] { "Different strings, case-insensitive",     "Test", "Other", false, false }
        };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object left = testCase[1];
                object right = testCase[2];
                bool caseSensitive = (bool)testCase[3];
                bool expected = (bool)testCase[4];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "Eq",
                    left,
                    right,
                    caseSensitive);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Eq_ObjectComparison_ShouldReturnExpectedResult()
        {
            // Arrange
            object object1 = new object();
            object object2 = new object();

            object[][] testCases =
            {
            new object[] { "Same object instance",       object1, object1, true },
            new object[] { "Different object instances", object1, object2, false }
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
                    "Eq",
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
        public void ExpressionFunction_Boolean_Eq_ParseMode_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Eq",
                "Test",
                "Test");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Eq_ParseModeWithThirdParameter_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Eq",
                "Test",
                "Test",
                true);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Eq_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
            new object[0],
            new object[] { "Test" },
            new object[] { "One", "Two", true, "Four" }
        };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Eq",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Eq_InvalidThirdParameter_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
            "invalid",
            1,
            1L,
            new object()
        };

            foreach (object thirdParameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "Eq",
                            "Test",
                            "Test",
                            thirdParameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Boolean_Eq_ParseModeWithInvalidThirdParameter_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "Eq",
                        "Test",
                        "Test",
                        "invalid");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}