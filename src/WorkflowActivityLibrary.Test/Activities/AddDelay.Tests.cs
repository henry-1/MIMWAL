using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using System.Reflection;
using System.Workflow.Activities;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class AddDelayTests
    {
        private void InvokeOnSequenceComplete(AddDelay activity)
        {
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "OnSequenceComplete",
                new object[] { null });
        }

        private void InitializeTimeoutDuration(AddDelay activity)
        {
            ActivityTestHelper.InvokePrivateMethod(
            activity,
            "Delay_InitializeTimeoutDuration",
            activity,
            EventArgs.Empty);
        }

        private ConditionalEventArgs EvaluateExecutionCondition(AddDelay activity)
        {
            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.ActivityExecutionCondition);

            MethodInfo method = typeof(AddDelay).GetMethod(
                "ActivityExecutionConditionSatisfied_Condition",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(method, "Execution condition handler was not found.");

            ConditionalEventArgs args = new ConditionalEventArgs();

            method.Invoke(activity, new object[] { activity, args });

            return args;
        }

        [TestMethod]
        public void Activities_AddDelay_TimeoutDuration_ExpressionReturningString_Should_SetDelayTimeout()
        {
            AddDelay activity = new AddDelay();

            activity.ActivityExpressionEvaluator.VariableCache["$Timeout"] =
                "00:30:00";

            activity.TimeoutDuration = "$Timeout";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.TimeoutDuration);

            InitializeTimeoutDuration(activity);

            DelayActivity delay =
                ActivityTestHelper.GetField<DelayActivity>(
                    activity,
                    "Delay");

            Assert.AreEqual(
                TimeSpan.FromMinutes(30),
                delay.TimeoutDuration);
        }

        [TestMethod]
        public void Activities_AddDelay_ExecutionCondition_NonBooleanExpression_Should_ReturnFalse()
        {
            AddDelay activity = new AddDelay();

            activity.ActivityExpressionEvaluator.VariableCache["$Condition"] = "true";
            activity.ActivityExecutionCondition = "$Condition";

            ConditionalEventArgs args = EvaluateExecutionCondition(activity);

            Assert.IsFalse(args.Result);
        }

        [TestMethod]
        public void Activities_AddDelay_ExecutionCondition_EmptyExpression_Should_ReturnTrue()
        {
            AddDelay activity = new AddDelay();
            activity.ActivityExecutionCondition = string.Empty;

            ConditionalEventArgs args = EvaluateExecutionCondition(activity);

            Assert.IsTrue(args.Result);
        }


        [TestMethod]
        public void Activities_AddDelay_Should_NotBeNull()
        {
            AddDelay activity = new AddDelay();

            Assert.IsNotNull(activity);
        }

        [TestMethod]
        public void Activities_AddDelay_Constructor_Should_InitializeExpressionEvaluator()
        {
            AddDelay activity = new AddDelay();

            Assert.IsNotNull(activity.ActivityExpressionEvaluator);
        }

        
        [TestMethod]
        public void Activities_AddDelay_TimeoutDuration_Should_PreserveAssignedValue()
        {
            AddDelay activity = new AddDelay();

            activity.TimeoutDuration = "00:30:00";

            Assert.AreEqual("00:30:00", activity.TimeoutDuration);
        }

        [TestMethod]
        public void Activities_AddDelay_ExecutionCondition_Should_PreserveAssignedValue()
        {
            AddDelay activity = new AddDelay();

            activity.ActivityExecutionCondition = "true";

            Assert.AreEqual("true", activity.ActivityExecutionCondition);
        }

        [TestMethod]
        public void Activities_AddDelay_DisplayName_Should_PreserveAssignedValue()
        {
            string displayName = "Activity Test";
            AddDelay activity = new AddDelay();

            activity.ActivityDisplayName = displayName;

            Assert.AreEqual(displayName, activity.ActivityDisplayName);
        }

        [TestMethod]
        public void Activities_AddDelay_TimeoutDuration_ValidStringValue_Should_SetDelayTimeout()
        {
            AddDelay activity = new AddDelay();
            activity.TimeoutDuration = "00:30:00";

            InitializeTimeoutDuration(activity);

            FieldInfo delayField = typeof(AddDelay).GetField(
                "Delay",
                BindingFlags.Instance | BindingFlags.NonPublic |
                BindingFlags.Public);

            Assert.IsNotNull(delayField, "Delay field was not found.");

            DelayActivity delay =
                (DelayActivity)delayField.GetValue(activity);

            Assert.AreEqual(
                TimeSpan.FromMinutes(30),
                delay.TimeoutDuration);
        }        

        [TestMethod]
        public void Activities_AddDelay_TimeoutDuration_InvalidValue_Should_ThrowException()
        {
            AddDelay activity = new AddDelay();
            activity.TimeoutDuration = "not-a-time";

            try
            {
                InitializeTimeoutDuration(activity);
                Assert.Fail("Expected an exception for an invalid timeout.");
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);

                Assert.AreEqual(
                    typeof(WorkflowActivityLibraryException),
                    ex.InnerException.GetType());
            }
        }


        [TestMethod]
        public void Activities_AddDelay_ExecutionCondition_TrueExpression_Should_ReturnTrue()
        {
            AddDelay activity = new AddDelay();
            activity.ActivityExecutionCondition = "true";

            ConditionalEventArgs args = EvaluateExecutionCondition(activity);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_AddDelay_ExecutionCondition_NullExpression_Should_ReturnTrue()
        {
            AddDelay activity = new AddDelay();
            activity.ActivityExecutionCondition = null;

            ConditionalEventArgs args = EvaluateExecutionCondition(activity);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_AddDelay_ExecutionCondition_FalseExpression_Should_ReturnFalse()
        {
            AddDelay activity = new AddDelay();
            activity.ActivityExecutionCondition = "false";

            ConditionalEventArgs args = EvaluateExecutionCondition(activity);

            Assert.IsFalse(args.Result);
        }

        

        [TestMethod]
        public void Activities_AddDelay_ParseExpressions_Should_ParseCondition()
        {
            AddDelay activity = new AddDelay();
            activity.ActivityExecutionCondition = "true";

            MethodInfo method = typeof(AddDelay).GetMethod(
                "ParseExpressions_ExecuteCode",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(method, "Parse expressions handler was not found.");

            method.Invoke(activity, new object[] { activity, EventArgs.Empty });

            // Verify the execution condition evaluates to true.
            MethodInfo conditionMethod = typeof(AddDelay).GetMethod(
                "ActivityExecutionConditionSatisfied_Condition",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(conditionMethod, "Execution condition handler was not found.");

            ConditionalEventArgs args = new ConditionalEventArgs();
            conditionMethod.Invoke(activity, new object[] { activity, args });

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_AddDelay_ParseExpressions_Should_ParseTimeout()
        {
            AddDelay activity = new AddDelay();
            TimeSpan expectedTimeout = TimeSpan.FromMinutes(30);

            activity.TimeoutDuration = "$Timeout";
            activity.ActivityExpressionEvaluator.VariableCache["$Timeout"] =
                expectedTimeout;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseExpressions_ExecuteCode",
                activity,
                EventArgs.Empty);

            InitializeTimeoutDuration(activity);

            DelayActivity delay =
                ActivityTestHelper.GetField<DelayActivity>(
                    activity,
                    "Delay");

            Assert.AreEqual(
                expectedTimeout,
                delay.TimeoutDuration);
        }

        [TestMethod]
        public void Activities_AddDelay_OnSequenceComplete_EmptyVariableCache_Should_NotThrow()
        {
            AddDelay activity = new AddDelay();

            // if this fails, the test fails
            InvokeOnSequenceComplete(activity);
        }

        

        [TestMethod]
        public void Activities_AddDelay_TimeoutDuration_Expression_Should_SetDelayTimeout()
        {
            AddDelay activity = new AddDelay();

            TimeSpan expectedTimeout = TimeSpan.FromMinutes(30);

            // Arrange: provide a variable that resolves to a TimeSpan.
            activity.ActivityExpressionEvaluator.VariableCache["$Timeout"] =
                expectedTimeout;

            activity.TimeoutDuration = "$Timeout";

            // Parse the expression, as AddDelay does before initializing the delay.
            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.TimeoutDuration);

            // Act: invoke the timeout initialization handler.
            InitializeTimeoutDuration(activity);

            // Assert: verify that the resolved TimeSpan was assigned to Delay.
            FieldInfo delayField = typeof(AddDelay).GetField(
                "Delay",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(delayField, "Delay field was not found.");

            DelayActivity delay =
                (DelayActivity)delayField.GetValue(activity);

            Assert.AreEqual(expectedTimeout, delay.TimeoutDuration);
        }

        [TestMethod]
        public void Activities_AddDelay_OnSequenceComplete_Should_ClearVariableCacheValues()
        {
            AddDelay activity = new AddDelay();

            activity.ActivityExpressionEvaluator.VariableCache["$Timeout"] =
                TimeSpan.FromMinutes(30);
            activity.ActivityExpressionEvaluator.VariableCache["$Name"] = "Test";

            MethodInfo method = typeof(AddDelay).GetMethod(
                "OnSequenceComplete",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(method, "OnSequenceComplete was not found.");

            method.Invoke(activity, new object[] { null });

            // Assert: keys remain, but their values are cleared.
            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey("$Timeout"));
            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey("$Name"));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["$Timeout"]);
            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["$Name"]);
        }

        [TestMethod]
        public void Activities_AddDelay_OnSequenceComplete_NoUserVariables_Should_ClearCacheValues()
        {
            AddDelay activity = new AddDelay();

            InvokeOnSequenceComplete(activity);

            foreach (string key in activity.ActivityExpressionEvaluator.VariableCache.Keys)
            {
                Assert.IsNull(
                    activity.ActivityExpressionEvaluator.VariableCache[key]);
            }
        }
    }
}
