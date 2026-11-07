using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.String
{
    [TestClass]
    public class SplitStringTests
    {
        [TestMethod]
        public void ExpressionFunction_String_SplitString_ValidParameters_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Default whitespace separator",
                    new object[]
                    {
                        "One Two Three"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Multiple whitespace characters",
                    new object[]
                    {
                        "One  Two\tThree"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Custom separator",
                    new object[]
                    {
                        "One;Two;Three",
                        ";"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Multi-character separator",
                    new object[]
                    {
                        "One--Two--Three",
                        "--"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Empty entries removed",
                    new object[]
                    {
                        "One;;Two;;;Three",
                        ";"
                    },
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    }
                },
                new object[]
                {
                    "Count limits result",
                    new object[]
                    {
                        "One;Two;Three",
                        ";",
                        2
                    },
                    new string[]
                    {
                        "One",
                        "Two;Three"
                    }
                },
                new object[]
                {
                    "Long count",
                    new object[]
                    {
                        "One;Two;Three",
                        ";",
                        2L
                    },
                    new string[]
                    {
                        "One",
                        "Two;Three"
                    }
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description = (string)testCase[0];
                object[] parameters = (object[])testCase[1];
                string[] expected = (string[])testCase[2];

                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "SplitString",
                    parameters);

                // Assert
                Assert.IsInstanceOfType(
                    result,
                    typeof(List<string>),
                    "Failed test case: " + description);

                CollectionAssert.AreEqual(
                    expected,
                    ((List<string>)result).ToArray(),
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_NullOrEmptyInput_ShouldReturnNull()
        {
            object[] testCases =
            {
                null,
                string.Empty
            };

            foreach (object input in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "SplitString",
                    input);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_NullOrEmptySeparator_ShouldUseDefaultSeparator()
        {
            object[] testCases =
            {
                null,
                string.Empty
            };

            foreach (object separator in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.Run(
                    "SplitString",
                    "One Two Three",
                    separator);

                // Assert
                CollectionAssert.AreEqual(
                    new string[]
                    {
                        "One",
                        "Two",
                        "Three"
                    },
                    ((List<string>)result).ToArray());
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_InvalidFirstParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object()
            };

            foreach (object input in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "SplitString",
                            input);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_InvalidSecondParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                42,
                42L,
                true,
                new object()
            };

            foreach (object separator in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "SplitString",
                            "One Two Three",
                            separator);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_InvalidThirdParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "2",
                true,
                2.0,
                new object()
            };

            foreach (object count in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "SplitString",
                            "One;Two;Three",
                            ";",
                            count);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_InvalidCount_ShouldThrowInvalidArgumentException()
        {

           
            object[] testCases =
            {
                -1,
                -10
            };

            foreach (object count in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "SplitString",
                            "One;Two;Three",
                            ";",
                            count);
                    },
                    typeof(InvalidArgumentException),
                    null);
            }
            
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_LongOutsideIntRange_ShouldThrowOverflowNotAllowedException()
        {
            object[] testCases =
            {
                (long)int.MaxValue + 1L,
                (long)int.MinValue - 1L
            };

            foreach (object count in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "SplitString",
                            "One;Two;Three",
                            ";",
                            count);
                    },
                    typeof(OverflowNotAllowedException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_ParseMode_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "SplitString",
                "One;Two;Three",
                ";",
                2);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "SplitString",
                        "One;Two;Three",
                        ";",
                        "2");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_String_SplitString_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],

                new object[]
                {
                    "One",
                    ";",
                    2,
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
                            "SplitString",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}