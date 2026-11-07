using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class NormalizeStringTests
    {
        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_ValidParameter_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Plain ASCII",
                    "Hello World",
                    "Hello World"
                },
                new object[]
                {
                    "German umlauts",
                    "Müller",
                    "Muller"
                },
                new object[]
                {
                    "Accented characters",
                    "Café",
                    "Cafe"
                },
                new object[]
                {
                    "Multiple accents",
                    "àéîõü",
                    "aeiou"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty,
                    string.Empty
                },
                new object[]
                {
                    "Null",
                    null,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object parameter = testCase[1];
                string expected = (string)testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "NormalizeString",
                    parameter);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_NonAsciiCharacterWithoutSubstitutionParameter_ShouldReturnCharacter()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "NormalizeString",
                "Ж");

            // Assert
            Assert.AreEqual(
                "Ж",
                result,
                "Failed test case: NonAsciiCharacter");
        }
        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_NonAsciiCharacterWithSubstitutionParameter_ShouldReturnSubstitution()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "NormalizeString",
                "Ж",
                "Ж:Zh");

            // Assert
            Assert.AreEqual(
                "Zh",
                result,
                "Failed test case: NonAsciiCharacter");
        }



        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_Substitutions_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single substitution",
                    "Hello World",
                    "World:Test",
                    "Hello Test"
                },
                new object[]
                {
                    "Multiple substitutions",
                    "Hello World",
                    "Hello:Hi|World:Test",
                    "Hi Test"
                },
                new object[]
                {
                    "Remove value",
                    "Hello-World",
                    "-:",
                    "HelloWorld"
                },
                new object[]
                {
                    "Substitution before normalization",
                    "Müller",
                    "Müller:Mueller",
                    "Mueller"
                },
                new object[]
                {
                    "Substitution before normalization for multiple characters",
                    "von Müller Straße",
                    "ü:ue|ß:ss",
                    "von Mueller Strasse"
                },
                new object[]
                {
                    "Whitespace around substitution",
                    "Hello World",
                    " World : Test ",
                    "Hello Test"
                },
                new object[]
                {
                    "Non ASCII Char with substitution",
                    "Ж",
                    "Ж:Zh",
                    "Zh"
                },
                new object[]
                {
                    "Non ASCII Char without substitution",
                    "Ж",
                    "Ä:Ae",
                    "Ж"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string input = (string)testCase[1];
                string substitutions = (string)testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "NormalizeString",
                    input,
                    substitutions);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_EmptyOrNullSubstitutions_ShouldNormalizeOnly()
        {
            object[] testCases =
            {
                null,
                string.Empty
            };

            foreach (object substitutions in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "NormalizeString",
                    "Müller",
                    substitutions);

                // Assert
                Assert.AreEqual(
                    "Muller",
                    result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_InvalidSubstitutionPair_ShouldIgnoreInvalidPair()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Missing separator",
                    "Hello World",
                    "Invalid",
                    "Hello World"
                },
                new object[]
                {
                    "Too many separators",
                    "Hello World",
                    "Hello:Hi:Extra",
                    "Hello World"
                },
                new object[]
                {
                    "Valid and invalid pairs",
                    "Hello World",
                    "Invalid|World:Test",
                    "Hello Test"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                string input = (string)testCase[1];
                string substitutions = (string)testCase[2];
                string expected = (string)testCase[3];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "NormalizeString",
                    input,
                    substitutions);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_NonStringFirstParameter_ShouldReturnNull()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object()
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "NormalizeString",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "NormalizeString",
                "Müller");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_NormalizeString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "Test",
                    "a:b",
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
                            "NormalizeString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}