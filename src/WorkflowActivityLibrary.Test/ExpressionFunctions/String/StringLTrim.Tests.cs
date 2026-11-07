using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class LTrimTests
    {
        [TestMethod]
        public void ExpressionFunction_String_LTrim_TrimCharacters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Default trimming",
                    new object[] { "   Hello   " },
                    "Hello   "
                },
                new object[]
                {
                    "Null trim characters",
                    new object[] { "   Hello   ", null },
                    "Hello   "
                },
                new object[]
                {
                    "Empty trim characters",
                    new object[] { "   Hello   ", string.Empty },
                    "Hello   "
                },
                new object[]
                {
                    "Single trim character",
                    new object[] { "xxxHello", "x" },
                    "Hello"
                },
                new object[]
                {
                    "Multiple trim characters",
                    new object[] { "xyxyHello", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "Only leading characters trimmed",
                    new object[] { "xxxHelloxxx", "x" },
                    "Helloxxx"
                },
                new object[]
                {
                    "No matching trim character",
                    new object[] { "Hello", "xy" },
                    "Hello"
                },
                new object[]
                {
                    "Single trim character",
                    new object[] { "xyxyHello", "x" },
                    "yxyHello"
                },
                new object[]
                {
                    "Single trim character",
                    new object[] { "xyxyHello", "y" },
                    "xyxyHello"
                },
                new object[]
                {
                    "Non-string trim parameter",
                    new object[] { "11123", 1 },
                    "23"
                }
                ,
                new object[]
                {
                    "Non-string trim parameter",
                    new object[] { null, "a" },
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                string description = (string)testCase[0];
                object[] parameters = (object[])testCase[1];
                string expected = (string)testCase[2];

                object result = ExpressionFunctionTestHelper.Run(
                    "LTrim",
                    parameters);

                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LTrim_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "LTrim",
                "   Hello World");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_LTrim_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "Hello",
                    " ",
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
                            "LTrim",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LTrim_CustomTrimCharacters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
        new object[]
        {
            "Single trim character",
            "xxxHello",
            "x",
            "Hello"
        },
        new object[]
        {
            "Multiple trim characters",
            "xyxyHello",
            "xy",
            "Hello"
        },
        new object[]
        {
            "Only leading characters trimmed",
            "xxxHelloxxx",
            "x",
            "Helloxxx"
        },
        new object[]
        {
            "No matching trim character",
            "Hello",
            "x",
            "Hello"
        },
        new object[]
        {
            "All characters trimmed",
            "xxxxx",
            "x",
            string.Empty
        },
        new object[]
        {
            "Whitespace explicitly trimmed",
            "   Hello   ",
            " ",
            "Hello   "
        }
    };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object value = testCase[1];
                object trimCharacters = testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "LTrim",
                    value,
                    trimCharacters);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_LTrim_NullTrimCharacters_ShouldUseDefaultWhitespaceTrimming()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "LTrim",
                "   Hello   ",
                null);

            // Assert
            Assert.AreEqual(
                "Hello   ",
                result);
        }      

        [TestMethod]
        public void ExpressionFunction_String_LTrim_NonStringTrimCharacters_ShouldUseToString()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "LTrim",
                "11123",
                1);

            // Assert
            Assert.AreEqual(
                "23",
                result);
        }
    }
}