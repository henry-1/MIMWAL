using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.Boolean
{
    // 
    [TestClass]
    public class ContainsTests
    {
        [TestMethod]
        public void ExpressionFunction_Collection_Contains_ListContainsValue_ShouldReturnTrue()
        {
            // Arrange
            List<string> values = new List<string>();
            values.Add("One");
            values.Add("Two");
            values.Add("Three");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                values,
                "Two");

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_ListDoesNotContainValue_ShouldReturnFalse()
        {
            // Arrange
            List<string> values = new List<string>();
            values.Add("One");
            values.Add("Two");
            values.Add("Three");

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                values,
                "Four");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_EmptyList_ShouldReturnFalse()
        {
            // Arrange
            List<string> values = new List<string>();

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                values,
                "Two");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_IntegerListContainsValue_ShouldReturnTrue()
        {
            // Arrange
            List<int> values = new List<int>();
            values.Add(10);
            values.Add(20);
            values.Add(30);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                values,
                20);

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_IntegerListDoesNotContainValue_ShouldReturnFalse()
        {
            // Arrange
            List<int> values = new List<int>();
            values.Add(10);
            values.Add(20);
            values.Add(30);

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                values,
                40);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_EqualScalarParameters_ShouldReturnTrue()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                "Test",
                "Test");

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_DifferentScalarParameters_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                "Test1",
                "Test2");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_FirstParameterNull_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                null,
                "Test");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_SecondParameterNull_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "Contains",
                "Test",
                null);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_ParseMode_ShouldReturnFalse()
        {
            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "Contains",
                "Test",
                "Test");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_WrongParameterCount_ShouldThrowException()
        {
            // Arrange
            Exception exception = null;
            List<string> values = new List<string>();

            // Act
            try
            {
                ExpressionFunctionTestHelper.Run("Contains", values);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(
                exception,
                typeof(InvalidFunctionFormatException));
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_NoParameters_ShouldThrowInvalidFunctionFormatException()
        {
            AssertContainsInvalidParameterCount(
                new object[0],
                0);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_OneParameter_ShouldThrowInvalidFunctionFormatException()
        {
            AssertContainsInvalidParameterCount(
                new object[] { "One" },
                1);
        }

        [TestMethod]
        public void ExpressionFunction_Collection_Contains_ThreeParameters_ShouldThrowInvalidFunctionFormatException()
        {
            AssertContainsInvalidParameterCount(
                new object[] { "One", "Two", "Three" },
                3);
        }

        private static void AssertContainsInvalidParameterCount(
            object[] parameters,
            int actualParameterCount)
        {
            InvalidFunctionFormatException expectedException =
                new InvalidFunctionFormatException(
                    "Contains",
                    2,
                    actualParameterCount);

            ExpressionFunctionTestHelper.AssertThrows(
                delegate
                {
                    ExpressionFunctionTestHelper.Run(
                        "Contains",
                        parameters);
                },
                typeof(InvalidFunctionFormatException),
                null);
        }
    }
}
