using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.ExpressionFunctions.DateAndTime
{
    [TestClass]
    public class DateTimeNowTests
    {
        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeNow_NoParameters_ShouldReturnCurrentUtcDateTime()
        {
            // Arrange
            DateTime before = DateTime.UtcNow;

            // Act
            object result = ExpressionFunctionTestHelper.Run(
                "DateTimeNow");

            DateTime after = DateTime.UtcNow;

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(DateTime));

            DateTime resultDate = (DateTime)result;

            Assert.IsTrue(
                resultDate >= before,
                "DateTimeNow returned a value before the test started.");

            Assert.IsTrue(
                resultDate <= after,
                "DateTimeNow returned a value after the test completed.");

            Assert.AreEqual(
                DateTimeKind.Utc,
                resultDate.Kind);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeNow_ParseMode_ShouldReturnCurrentUtcDateTime()
        {
            // Arrange
            DateTime before = DateTime.UtcNow;

            // Act
            object result = ExpressionFunctionTestHelper.RunParse(
                "DateTimeNow");

            DateTime after = DateTime.UtcNow;

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(DateTime));

            DateTime resultDate = (DateTime)result;

            Assert.IsTrue(
                resultDate >= before);

            Assert.IsTrue(
                resultDate <= after);

            Assert.AreEqual(
                DateTimeKind.Utc,
                resultDate.Kind);
        }

        [TestMethod]
        public void ExpressionFunction_DateTime_DateTimeNow_InvalidParameterCount_ShouldThrowInvalidFunctionFormatException()
        {
            object[][] testCases =
            {
                new object[] { 1 },
                new object[] { 1, 2 }
            };

            foreach (object[] parameters in testCases)
            {
                // Act / Assert
                ExpressionFunctionTestHelper.AssertThrows(
                    delegate
                    {
                        ExpressionFunctionTestHelper.Run(
                            "DateTimeNow",
                            parameters);
                    },
                    typeof(InvalidFunctionFormatException),
                    null);
            }
        }
    }
}