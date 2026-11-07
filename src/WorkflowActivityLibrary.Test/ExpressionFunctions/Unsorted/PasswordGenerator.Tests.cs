using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using WorkflowActivityLibrary.Tests.ExpressionFunctions;

namespace WorkflowActivityLibrary.Tests.Common
{

    [TestClass]
    public class PasswordGeneratorTests
    {
        private const string AllowedCharacters =
            "ABCDEFGHJKLMNPQRSTUVWXYZ" +
            "abcdefghjkmnpqrstuvwxyz" +
            "23456789" +
            "!@#$&";

        [TestMethod]
        public void PasswordGenerator_GenerateRandomPassword_ValidLength_ShouldReturnExpectedLength()
        {
            int[] testCases =
            {
                4,
                5,
                8,
                12,
                32,
                100
            };

           
            foreach (int length in testCases)
            {
                // Act
                string result =
                    PasswordGenerator.GenerateRandomPassword(length);

                // Assert
                Assert.IsNotNull(result);

                Assert.AreEqual(
                    length,
                    result.Length,
                    "Failed for password length: " + length);
            }
        }

        [TestMethod]
        public void PasswordGenerator_GenerateRandomPassword_ShouldMeetComplexityRequirements()
        {
            for (int i = 0; i < 100; i++)
            {
                // Act
                string result =
                    PasswordGenerator.GenerateRandomPassword(12);

                // Assert
                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[A-Z]"),
                    "Password contains no uppercase character: " + result);

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[a-z]"),
                    "Password contains no lowercase character: " + result);

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[23456789]"),
                    "Password contains no number: " + result);

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[!@#$&]"),
                    "Password contains no special character: " + result);
            }
        }

        [TestMethod]
        public void PasswordGenerator_GenerateRandomPassword_ShouldOnlyContainAllowedCharacters()
        {
            for (int i = 0; i < 100; i++)
            {
                // Act
                string result =
                    PasswordGenerator.GenerateRandomPassword(32);

                // Assert
                foreach (char character in result)
                {
                    Assert.IsTrue(
                        AllowedCharacters.IndexOf(character) >= 0,
                        "Unexpected password character: " + character);
                }
            }
        }

        [TestMethod]
        public void PasswordGenerator_GenerateRandomPassword_MinimumLength_ShouldMeetComplexityRequirements()
        {
            for (int i = 0; i < 100; i++)
            {
                // Act
                string result =
                    PasswordGenerator.GenerateRandomPassword(4);

                // Assert
                Assert.AreEqual(
                    4,
                    result.Length);

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[A-Z]"));

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[a-z]"));

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[23456789]"));

                Assert.IsTrue(
                    Regex.IsMatch(
                        result,
                        "[!@#$&]"));
            }
        }

        [TestMethod]
        public void PasswordGenerator_GenerateRandomPassword_BelowMinimumLength_ShouldThrowArgumentOutOfRangeException()
        {
            int[] testCases =
            {
                int.MinValue,
                -1,
                0,
                1,
                2,
                3
            };

            foreach (int length in testCases)
            {
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        PasswordGenerator.GenerateRandomPassword(
                            length);
                    },
                    typeof(ArgumentOutOfRangeException),
                    null);
            }
        }

        [TestMethod]
        public void PasswordGenerator_GenerateRandomPassword_SameInstance_ShouldSupportMultipleCalls()
        {
            for (int i = 0; i < 100; i++)
            {
                // Act
                string result =
                    PasswordGenerator.GenerateRandomPassword(12);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(12, result.Length);
            }
        }
    }
}