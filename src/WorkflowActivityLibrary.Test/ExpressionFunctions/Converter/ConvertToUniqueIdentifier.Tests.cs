using System;
using System.Collections.Generic;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using Microsoft.ResourceManagement.WebServices;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Converter
{
    [TestClass]
    public class ConvertToUniqueIdentifierTests
    {
        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_Guid_ShouldReturnExpectedResult()
        {
            // Arrange
            Guid guid = new Guid(
                "550e8400-e29b-41d4-a716-446655440000");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertToUniqueIdentifier",
                guid);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(UniqueIdentifier));

            Assert.AreEqual(
                guid,
                ((UniqueIdentifier)result).GetGuid());
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_ByteArray_ShouldReturnExpectedResult()
        {
            // Arrange
            Guid guid = new Guid(
                "550e8400-e29b-41d4-a716-446655440000");

            byte[] parameter = guid.ToByteArray();

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertToUniqueIdentifier",
                (object)parameter);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(UniqueIdentifier));

            Assert.AreEqual(
                guid,
                ((UniqueIdentifier)result).GetGuid());
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_GuidList_ShouldReturnExpectedResult()
        {
            // Arrange
            Guid guid1 = new Guid(
                "550e8400-e29b-41d4-a716-446655440000");

            Guid guid2 = new Guid(
                "550e8400-e29b-41d4-a716-446655440001");

            Guid guid3 = new Guid(
                "550e8400-e29b-41d4-a716-446655440002");

            List<Guid> parameter = new List<Guid>
            {
                guid1,
                guid2,
                guid3
            };

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertToUniqueIdentifier",
                parameter);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<UniqueIdentifier>));

            List<UniqueIdentifier> resultList =
                (List<UniqueIdentifier>)result;

            Assert.AreEqual(3, resultList.Count);

            Assert.AreEqual(guid1, resultList[0].GetGuid());
            Assert.AreEqual(guid2, resultList[1].GetGuid());
            Assert.AreEqual(guid3, resultList[2].GetGuid());
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_EmptyGuidList_ShouldReturnEmptyList()
        {
            // Arrange
            List<Guid> parameter = new List<Guid>();

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertToUniqueIdentifier",
                parameter);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(List<UniqueIdentifier>));

            List<UniqueIdentifier> resultList =
                (List<UniqueIdentifier>)result;

            Assert.AreEqual(0, resultList.Count);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_Null_ShouldReturnNull()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "ConvertToUniqueIdentifier",
                (object)null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_InvalidByteArray_ShouldThrowArgumentException()
        {
            byte[][] testCases =
            {
                new byte[0],
                new byte[] { 1 },
                new byte[] { 1, 2, 3 },
                new byte[15],
                new byte[17]
            };

            foreach (byte[] parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToUniqueIdentifier",
                            (object)parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_InvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            object[] testCases =
            {
                "550e8400-e29b-41d4-a716-446655440000",
                42,
                42L,
                true,
                new object(),
                new List<string>()
            };

            foreach (object parameter in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToUniqueIdentifier",
                            parameter);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_ParseMode_ShouldReturnNull()
        {
            object[] testCases =
            {
                Guid.Empty,
                new List<Guid> { Guid.Empty },
                new byte[16],
                null
            };

            foreach (object parameter in testCases)
            {
                // Act
                object result = ExpressionFunctionTestHelper.RunParse(
                    "ConvertToUniqueIdentifier",
                    parameter);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_ParseModeWithInvalidParameterType_ShouldThrowInvalidFunctionFormatException()
        {
            // Act / Assert
            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.RunParse(
                        "ConvertToUniqueIdentifier",
                        "550e8400-e29b-41d4-a716-446655440000");
                },
                typeof(InvalidFunctionFormatException),
                null);
        }

        [TestMethod]
        public void ExpressionFunction_Converter_ConvertToUniqueIdentifier_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[0],
                new object[] { Guid.Empty, Guid.Empty }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "ConvertToUniqueIdentifier",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}