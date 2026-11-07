using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertSidToStringTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_ValidSid_ShouldReturnExpectedResult()
        {
            string[] testCases =
            {
                "S-1-0-0",
                "S-1-1-0",
                "S-1-5-18",
                "S-1-5-32-544"
            };

            foreach (string expectedSid in testCases)
            {
                // Arrange
                SecurityIdentifier sid =
                    new SecurityIdentifier(expectedSid);

                byte[] sidBytes =
                    new byte[sid.BinaryLength];

                sid.GetBinaryForm(
                    sidBytes,
                    0);

                // Act
                object result =
                    ExpressionFunctionTestHelper.Run(
                        "ConvertSidToString",
                        sidBytes);

                // Assert
                Assert.AreEqual(
                    expectedSid,
                    result,
                    "Failed SID: " + expectedSid);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_Null_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "ConvertSidToString",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "S-1-5-18",
                42,
                42L,
                true,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertSidToString",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_InvalidSid_ShouldThrowInvalidFunctionFormatException()
        {
            byte[][] testCases =
            {
                new byte[0],
                new byte[] { 1 },
                new byte[] { 1, 2, 3 },
                new byte[] { 255, 255, 255, 255 }
            };

            foreach (byte[] parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertSidToString",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_ParseMode_ShouldReturnNull()
        {
            // Arrange
            SecurityIdentifier sid =
                new SecurityIdentifier("S-1-5-18");

            byte[] sidBytes =
                new byte[sid.BinaryLength];

            sid.GetBinaryForm(
                sidBytes,
                0);

            // Act
            object result =
                ExpressionFunctionTestHelper.RunParse(
                    "ConvertSidToString",
                    sidBytes);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_ParseModeWithNull_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertSidToString",
                        (object)null);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertSidToString",
                        "S-1-5-18");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertSidToString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { new byte[] { 1 }, new byte[] { 2 } }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertSidToString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}