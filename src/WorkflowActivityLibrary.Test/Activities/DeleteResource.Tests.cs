using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using System.Reflection;
using System.Collections.Generic;
using System.Workflow.Activities;

namespace WorkflowActivityLibrary.Tests.Activities
{
    /// <summary>
    /// DeleteResourceTests tests
    /// </summary>
    /// <remarks>
    /// It covers the activity-specific behavior:
    ///
    /// - constructor initialization
    /// - advanced-setting cleanup
    /// - target-expression parsing
    /// - single Guid resolution
    /// - List<Guid> resolution
    /// - null resolution
    /// - preserving search results
    /// - assigning the current deletion target
    /// - resetting the target for null
    /// - loading ValueExpressions
    /// - target-selection conditions
    /// - representative iteration behavior
    /// - variable-cache cleanup
    /// 
    /// The following tests are not repeated from AddDelay and CreateRessource because they
    /// would only repeate tests already done and demonstrated that underlying machinery extensively
    /// - no condition
    /// - true condition
    /// - false condition
    /// - non-Boolean condition
    ///
    /// - no iteration
    /// - single iteration
    /// - list iteration
    /// - null iteration
    ///
    /// - Authorization = true
    /// - Authorization = false
    ///
    /// - ActorString value expression
    /// - ActorString non-value expression
    ///
    /// - ValueExpressions discovery
    /// - case-insensitive ValueExpressions
    /// </remarks>

    [TestClass]
    public class DeleteResourceTests
    {
        [TestMethod]
        public void Activities_DeleteResources_Constructor_Should_InitializeRequiredObjects()
        {
            DeleteResources activity = new DeleteResources();

            Assert.IsNotNull(activity.Targets);
            Assert.IsNotNull(activity.ActivityExpressionEvaluator);
            Assert.IsNotNull(activity.ValueExpressions);
        }

        [TestMethod]
        public void Activities_DeleteResources_PrepareResolve_NotAdvanced_Should_ClearAdvancedSettings()
        {
            DeleteResources activity = new DeleteResources();

            activity.Advanced = false;
            activity.ActivityExecutionCondition = "true";
            activity.Iteration = "$Values";
            activity.ActorType = ActorType.Requestor;
            activity.ApplyAuthorizationPolicy = true;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareResolve_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNull(activity.ActivityExecutionCondition);
            Assert.IsNull(activity.Iteration);
            Assert.AreEqual(
                ActorType.Service,
                activity.ActorType);
            Assert.IsFalse(activity.ApplyAuthorizationPolicy);
        }


        [TestMethod]
        public void Activities_DeleteResources_PrepareDelete_ResolveTargetSingleGuid_Should_AddTarget()
        {
            DeleteResources activity = new DeleteResources();

            Guid expectedTarget = Guid.NewGuid();

            activity.TargetType =
                DeleteResourcesTargetType.ResolveTarget;

            activity.TargetExpression = "[//Value]";

            object resolve =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "Resolve");

            PropertyInfo lookupsProperty =
                resolve.GetType().GetProperty(
                    "Lookups",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                lookupsProperty,
                "Lookups property was not found.");

            Dictionary<string, object> lookups =
                new Dictionary<string, object>(
                    StringComparer.OrdinalIgnoreCase);

            lookups[activity.TargetExpression] =
                expectedTarget;

            lookupsProperty.SetValue(
                resolve,
                lookups,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDelete_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                1,
                activity.Targets.Count);

            Assert.AreEqual(
                expectedTarget,
                activity.Targets[0]);
        }

        [TestMethod]
        public void Activities_DeleteResources_PrepareDelete_ResolveTargetGuidList_Should_SetTargets()
        {
            DeleteResources activity = new DeleteResources();

            Guid firstTarget = Guid.NewGuid();
            Guid secondTarget = Guid.NewGuid();

            List<Guid> expectedTargets = new List<Guid>();
            expectedTargets.Add(firstTarget);
            expectedTargets.Add(secondTarget);

            activity.TargetType =
                DeleteResourcesTargetType.ResolveTarget;

            activity.TargetExpression = "[//Value]";

            object resolve =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "Resolve");

            PropertyInfo lookupsProperty =
                resolve.GetType().GetProperty(
                    "Lookups",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                lookupsProperty,
                "Lookups property was not found.");

            Dictionary<string, object> lookups =
                new Dictionary<string, object>(
                    StringComparer.OrdinalIgnoreCase);

            lookups[activity.TargetExpression] =
                expectedTargets;

            lookupsProperty.SetValue(
                resolve,
                lookups,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDelete_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                2,
                activity.Targets.Count);

            Assert.AreEqual(
                firstTarget,
                activity.Targets[0]);

            Assert.AreEqual(
                secondTarget,
                activity.Targets[1]);

            Assert.AreSame(
                expectedTargets,
                activity.Targets);
        }

        [TestMethod]
        public void Activities_DeleteResources_PrepareDelete_ResolveTargetNull_Should_NotAddTarget()
        {
            DeleteResources activity = new DeleteResources();

            activity.TargetType =
                DeleteResourcesTargetType.ResolveTarget;

            activity.TargetExpression = "[//Value]";

            object resolve =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "Resolve");

            PropertyInfo lookupsProperty =
                resolve.GetType().GetProperty(
                    "Lookups",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                lookupsProperty,
                "Lookups property was not found.");

            Dictionary<string, object> lookups =
                new Dictionary<string, object>(
                    StringComparer.OrdinalIgnoreCase);

            lookups[activity.TargetExpression] = null;

            lookupsProperty.SetValue(
                resolve,
                lookups,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDelete_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                0,
                activity.Targets.Count);
        }

        [TestMethod]
        public void Activities_DeleteResources_ForEachTarget_ChildInitialized_Should_SetTarget()
        {
            DeleteResources activity = new DeleteResources();

            Guid expectedTarget = Guid.NewGuid();

            ReplicatorChildEventArgs args =
                new ReplicatorChildEventArgs(
                    expectedTarget,
                    null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachTarget_ChildInitialized",
                activity,
                args);

            Assert.AreEqual(
                expectedTarget,
                activity.Target);
        }

        [TestMethod]
        public void Activities_DeleteResources_ForEachTarget_ChildInitialized_Null_Should_SetEmptyTarget()
        {
            DeleteResources activity = new DeleteResources();

            activity.Target = Guid.NewGuid();

            ReplicatorChildEventArgs args =
                new ReplicatorChildEventArgs(
                    null,
                    null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachTarget_ChildInitialized",
                activity,
                args);

            Assert.AreEqual(
                Guid.Empty,
                activity.Target);
        }


        [TestMethod]
        public void Activities_DeleteResources_PrepareTarget_Should_LoadValueExpressionsIntoLookupCache()
        {
            DeleteResources activity = new DeleteResources();

            activity.ValueExpressions["[//Value/DisplayName]"] =
                "Test User";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareTarget_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    "[//Value/DisplayName]"));

            object value =
                activity.ActivityExpressionEvaluator.LookupCache[
                    "[//Value/DisplayName]"];

            Assert.AreEqual(
                "Test User",
                value);
        }


        [TestMethod]
        public void Activities_DeleteResources_TargetConditions_Should_ReturnExpectedResult()
        {
            DeleteResourcesTargetType[] targetTypes =
                new DeleteResourcesTargetType[]
                {
            DeleteResourcesTargetType.SearchForTarget,
            DeleteResourcesTargetType.ResolveTarget,
            DeleteResourcesTargetType.WorkflowTarget
                };

            foreach (DeleteResourcesTargetType targetType in targetTypes)
            {
                DeleteResources activity = new DeleteResources();
                activity.TargetType = targetType;

                ConditionalEventArgs searchArgs =
                    new ConditionalEventArgs();

                ConditionalEventArgs resolveArgs =
                    new ConditionalEventArgs();

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "SearchForTarget_Condition",
                    activity,
                    searchArgs);

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ResolveTarget_Condition",
                    activity,
                    resolveArgs);

                Assert.AreEqual(
                    targetType == DeleteResourcesTargetType.SearchForTarget,
                    searchArgs.Result,
                    "Unexpected SearchForTarget result for TargetType '" +
                    targetType +
                    "'.");

                Assert.AreEqual(
                    targetType == DeleteResourcesTargetType.ResolveTarget,
                    resolveArgs.Result,
                    "Unexpected ResolveTarget result for TargetType '" +
                    targetType +
                    "'.");
            }
        }

        [TestMethod]
        public void Activities_DeleteResources_PrepareDelete_SearchForTarget_Should_PreserveTargets()
        {
            DeleteResources activity = new DeleteResources();

            Guid firstTarget = Guid.NewGuid();
            Guid secondTarget = Guid.NewGuid();

            activity.TargetType =
                DeleteResourcesTargetType.SearchForTarget;

            activity.Targets.Add(firstTarget);
            activity.Targets.Add(secondTarget);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDelete_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                2,
                activity.Targets.Count);

            Assert.AreEqual(
                firstTarget,
                activity.Targets[0]);

            Assert.AreEqual(
                secondTarget,
                activity.Targets[1]);
        }


        [TestMethod]
        public void Activities_DeleteResources_ForEachIteration_ChildInitialized_Should_SetValue()
        {
            DeleteResources activity = new DeleteResources();

            string expectedValue = "Value1";

            ReplicatorChildEventArgs args =
                new ReplicatorChildEventArgs(
                    expectedValue,
                    null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildInitialized",
                activity,
                args);

            Assert.AreEqual(
                expectedValue,
                activity.Value);
        }

        [TestMethod]
        public void Activities_DeleteResources_OnSequenceComplete_Should_ClearVariableCacheValues()
        {
            DeleteResources activity = new DeleteResources();

            activity.ActivityExpressionEvaluator.VariableCache["$Target"] =
                Guid.NewGuid();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "OnSequenceComplete",
                new object[] { null });

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$Target"));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["$Target"]);
        }

        [TestMethod]
        public void Activities_DeleteResources_PrepareIteration_NoIteration_Should_CreateSingleNullIteration()
        {
            DeleteResources activity = new DeleteResources();

            activity.ActivityExecutionCondition = null;
            activity.Iteration = null;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(
                forEachIteration.InitialChildData);

            Assert.AreEqual(
                1,
                forEachIteration.InitialChildData.Count);

            Assert.IsNull(
                forEachIteration.InitialChildData[0]);
        }

        [TestMethod]
        public void Activities_DeleteResources_PrepareResolve_ResolveTarget_Should_ParseTargetExpression()
        {
            DeleteResources activity = new DeleteResources();

            activity.Advanced = true;
            activity.TargetType =
                DeleteResourcesTargetType.ResolveTarget;

            activity.TargetExpression =
                "[//Value/ObjectID]";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareResolve_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    "[//Value/ObjectID]"));
        }





    }
}
