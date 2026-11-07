using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using System.Collections.Generic;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.Reflection;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using System.Collections;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class UpdateResourcesTests
    {
        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_DynamicString_ShouldPrepareForResolution()
        {
            // Arrange
            const string source =
                "[//WorkflowData/DynamicSource]";

            const string resolvedDynamicString =
                "DynamicLookupString";

            const string target =
                "[//WorkflowData/TargetValue]";

            UpdateResources activity =
                new UpdateResources();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                resolvedDynamicString;

            // Verify that variable values are cleared by the method.
            activity.ActivityExpressionEvaluator.VariableCache["TestVariable"] =
                "OldValue";

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            updates.Add(
                new Definition(
                    source,
                    target,
                    false));

            // Existing values should be discarded.
            activity.DynamicStringsForResolution.Add(
                "OldDynamicString");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                1,
                activity.DynamicStringsForResolution.Count);

            Assert.AreEqual(
                resolvedDynamicString,
                activity.DynamicStringsForResolution[0]);

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["TestVariable"]);

            List<Definition> preparedUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                1,
                preparedUpdates.Count);

            Assert.AreEqual(
                resolvedDynamicString,
                preparedUpdates[0].Left);

            Assert.AreEqual(
                target,
                preparedUpdates[0].Right);

            Assert.IsFalse(
                preparedUpdates[0].Check);
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_ResolvedExpression_ShouldNotPrepareDynamicResolution()
        {
            // Arrange
            const string source =
                "[//WorkflowData/DynamicSource]";

            const string resolvedExpression =
                "[//WorkflowData/ResolvedSource]";

            const string target =
                "[//WorkflowData/TargetValue]";

            UpdateResources activity =
                new UpdateResources();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                resolvedExpression;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            updates.Add(
                new Definition(
                    source,
                    target,
                    false));

            activity.DynamicStringsForResolution.Add(
                "OldDynamicString");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                0,
                activity.DynamicStringsForResolution.Count);

            List<Definition> preparedUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                1,
                preparedUpdates.Count);

            Assert.AreEqual(
                resolvedExpression,
                preparedUpdates[0].Left);

            Assert.AreEqual(
                target,
                preparedUpdates[0].Right);

            Assert.IsFalse(
                preparedUpdates[0].Check);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    resolvedExpression));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.LookupCache[
                    resolvedExpression]);
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_VariableTarget_ShouldPublishVariable()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "$TargetVariable";

            const string expectedValue =
                "TestValue";

            UpdateResources activity =
                new UpdateResources();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                expectedValue;

            activity.ActivityExpressionEvaluator.VariableCache[target] =
                null;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            updates.Add(
                new Definition(
                    source,
                    target,
                    false));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                expectedValue,
                activity.ActivityExpressionEvaluator.VariableCache[target]);

            Assert.AreEqual(
                1,
                activity.DynamicStringsForResolution.Count);

            Assert.AreEqual(
                expectedValue,
                activity.DynamicStringsForResolution[0]);

            List<Definition> preparedUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                1,
                preparedUpdates.Count);

            Assert.AreEqual(
                expectedValue,
                preparedUpdates[0].Left);

            Assert.AreEqual(
                target,
                preparedUpdates[0].Right);

            Assert.IsFalse(
                preparedUpdates[0].Check);
        }

        [TestMethod]
        public void Activities_UpdateResources_ForEachDynamicStringForResolution_ChildInitialized_ShouldSetStringForResolution()
        {
            // Arrange
            const string expected =
                "DynamicLookupString";

            UpdateResources activity =
                new UpdateResources();

            Type resolveLookupStringType =
                typeof(UpdateResources).Assembly.GetType(
                    "MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.ComponentActivities.ResolveLookupString",
                    true);

            Activity resolveLookupString =
                (Activity)Activator.CreateInstance(
                    resolveLookupStringType,
                    true);

            ReplicatorChildEventArgs arguments =
                new ReplicatorChildEventArgs(
                    expected,
                    resolveLookupString);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachDynamicStringForResolution_ChildInitialized",
                null,
                arguments);

            // Assert
            PropertyInfo property =
                resolveLookupStringType.GetProperty(
                    "StringForResolution",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            Assert.IsNotNull(
                property);

            Assert.AreEqual(
                expected,
                property.GetValue(
                    resolveLookupString,
                    null));
        }

        [TestMethod]
        public void Activities_UpdateResources_ForEachDynamicStringForResolution_ChildCompleted_ShouldAddResolvedValueToLookupCache()
        {
            // Arrange
            const string stringForResolution =
                "DynamicLookupString";

            const string expectedResolvedValue =
                "ResolvedValue";

            UpdateResources activity =
                new UpdateResources();

            Type resolveLookupStringType =
                typeof(UpdateResources).Assembly.GetType(
                    "MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.ComponentActivities.ResolveLookupString",
                    true);

            Activity resolveLookupString =
                (Activity)Activator.CreateInstance(
                    resolveLookupStringType,
                    true);

            PropertyInfo resolvedProperty =
                resolveLookupStringType.GetProperty(
                    "Resolved",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            Assert.IsNotNull(
                resolvedProperty);

            resolvedProperty.SetValue(
                resolveLookupString,
                expectedResolvedValue,
                null);

            ReplicatorChildEventArgs arguments =
                new ReplicatorChildEventArgs(
                    stringForResolution,
                    resolveLookupString);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachDynamicStringForResolution_ChildCompleted",
                null,
                arguments);

            // Assert
            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    stringForResolution));

            Assert.AreEqual(
                expectedResolvedValue,
                activity.ActivityExpressionEvaluator.LookupCache[
                    stringForResolution]);
        }

        [TestMethod]
        public void Activities_UpdateResources_ActorIsNotValueExpression_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Value expression",
                    "[//Value/Actor]",
                    false
                },
                new object[]
                {
                    "WorkflowData expression",
                    "[//WorkflowData/Actor]",
                    true
                },
                new object[]
                {
                    "Literal Guid",
                    Guid.NewGuid().ToString(),
                    true
                },
                new object[]
                {
                    "Null",
                    null,
                    true
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string actorString =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                UpdateResources activity =
                    new UpdateResources();

                activity.ActorString =
                    actorString;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ActorIsNotValueExpression_Condition",
                    null,
                    arguments);

                // Assert
                Assert.AreEqual(
                    expected,
                    arguments.Result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_UpdateResources_DynamicGrammarResolutionNeed_ShouldReturnConfiguredValue()
        {
            bool[] testCases =
            {
                false,
                true
            };

            foreach (bool resolveDynamicGrammar in testCases)
            {
                // Arrange
                UpdateResources activity =
                    new UpdateResources();

                activity.ResolveDynamicGrammar =
                    resolveDynamicGrammar;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "DynamicGrammarResolutionNeed_Condition",
                    null,
                    arguments);

                // Assert
                Assert.AreEqual(
                    resolveDynamicGrammar,
                    arguments.Result);
            }
        }

        [TestMethod]
        public void Activities_UpdateResources_ForEachIteration_ChildCompleted_DynamicGrammar_ShouldRestoreOriginalUpdates()
        {
            // Arrange
            const string originalSource =
                "[//WorkflowData/OriginalSource]";

            const string originalTarget =
                "[//WorkflowData/TargetValue]";

            Hashtable updatesTable =
                new Hashtable();

            updatesTable["Count"] =
                1;

            updatesTable["0:0"] =
                originalSource;

            updatesTable["0:1"] =
                originalTarget;

            updatesTable["0:2"] =
                false;

            UpdateResources activity =
                new UpdateResources();

            activity.ResolveDynamicGrammar =
                true;

            activity.UpdatesTable =
                updatesTable;

            // Simulate the working definition having been modified
            // during dynamic grammar resolution.
            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            updates.Add(
                new Definition(
                    "ModifiedDynamicSource",
                    originalTarget,
                    false));

            // ChildCompleted expects an initialized replicator.
            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            forEachIteration.InitialChildData =
                new object[]
                {
            "TestValue"
                };

            activity.ActivityExpressionEvaluator.VariableCache[
                ExpressionEvaluator.ReservedVariableBreakIteration] =
                false;

            ReplicatorChildEventArgs arguments =
                new ReplicatorChildEventArgs(
                    "TestValue",
                    new SequenceActivity());

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildCompleted",
                null,
                arguments);

            // Assert
            List<Definition> restoredUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                1,
                restoredUpdates.Count);

            Assert.AreEqual(
                originalSource,
                restoredUpdates[0].Left);

            Assert.AreEqual(
                originalTarget,
                restoredUpdates[0].Right);

            Assert.IsFalse(
                restoredUpdates[0].Check);

            bool breakIteration =
                ActivityTestHelper.GetValueField<bool>(
                    activity,
                    "breakIteration");

            Assert.IsFalse(
                breakIteration);
        }

        [TestMethod]
        public void Activities_UpdateResources_ResolveSourceValue_ShouldReturnExpectedValue()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Expression source",
                    "[//WorkflowData/SourceValue]",
                    "ExpressionValue"
                },
                new object[]
                {
                    "Resolved dynamic source",
                    "ResolvedDynamicString",
                    "DynamicValue"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string source =
                    (string)testCase[1];

                object expected =
                    testCase[2];

                UpdateResources activity =
                    new UpdateResources();

                activity.ActivityExpressionEvaluator.LookupCache[source] =
                    expected;

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "ResolveSourceValue",
                        source);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareUpdate_DynamicallyResolvedSource_ShouldCreateModifyUpdate()
        {
            // Arrange
            const string dynamicSource =
                "DynamicLookupString";

            const string resolvedValue =
                "ResolvedValue";

            const string target =
                "[//WorkflowData/TargetValue]";

            UpdateResources activity =
                new UpdateResources();

            // Simulate the result produced by
            // ForEachDynamicStringForResolution_ChildCompleted.
            activity.ActivityExpressionEvaluator.LookupCache[dynamicSource] =
                resolvedValue;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            updates.Add(
                new Definition(
                    dynamicSource,
                    target,
                    false));

            // Add an old value to verify that PrepareUpdate
            // replaces the publication list.
            List<UpdateLookupDefinition> oldLookupUpdates =
                activity.LookupUpdates;

            oldLookupUpdates.Add(
                new UpdateLookupDefinition(
                    "[//WorkflowData/OldTarget]",
                    "OldValue",
                    UpdateMode.Modify));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareUpdate_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreNotSame(
                oldLookupUpdates,
                activity.LookupUpdates);

            Assert.AreEqual(
                1,
                activity.LookupUpdates.Count);

            UpdateLookupDefinition update =
                activity.LookupUpdates[0];

            Assert.AreEqual(
                target,
                update.TargetLookup);

            Assert.AreEqual(
                resolvedValue,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_NonStringResult_ShouldPreserveDefinition()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "[//WorkflowData/TargetValue]";

            UpdateResources activity =
                new UpdateResources();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                42L;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Definition originalDefinition =
                new Definition(
                    source,
                    target,
                    true);

            updates.Add(
                originalDefinition);

            activity.DynamicStringsForResolution.Add(
                "OldDynamicString");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                0,
                activity.DynamicStringsForResolution.Count);

            List<Definition> preparedUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                1,
                preparedUpdates.Count);

            Assert.AreSame(
                originalDefinition,
                preparedUpdates[0]);

            Assert.AreEqual(
                source,
                preparedUpdates[0].Left);

            Assert.AreEqual(
                target,
                preparedUpdates[0].Right);

            Assert.IsTrue(
                preparedUpdates[0].Check);
        }

        [TestMethod]
        public void Activities_UpdateResources_Prepare_UpdatesTable_ShouldCreateDefinitionsAndParseSources()
        {
            // Arrange
            const string firstSource =
                "[//WorkflowData/FirstSource]";

            const string secondSource =
                "[//WorkflowData/SecondSource]";

            const string firstTarget =
                "[//Target/FirstAttribute]";

            const string secondTarget =
                "[//Target/SecondAttribute]";

            Hashtable updatesTable =
                new Hashtable();

            updatesTable["Count"] =
                2;

            updatesTable["0:0"] =
                firstSource;

            updatesTable["0:1"] =
                firstTarget;

            updatesTable["0:2"] =
                false;

            updatesTable["1:0"] =
                secondSource;

            updatesTable["1:1"] =
                secondTarget;

            updatesTable["1:2"] =
                true;

            UpdateResources activity =
                new UpdateResources();

            activity.Advanced =
                true;

            activity.UpdatesTable =
                updatesTable;

            Dictionary<string, object> lookupCache =
                activity.ActivityExpressionEvaluator.LookupCache;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                2,
                updates.Count);

            Assert.AreEqual(
                firstSource,
                updates[0].Left);

            Assert.AreEqual(
                firstTarget,
                updates[0].Right);

            Assert.IsFalse(
                updates[0].Check);

            Assert.AreEqual(
                secondSource,
                updates[1].Left);

            Assert.AreEqual(
                secondTarget,
                updates[1].Right);

            Assert.IsTrue(
                updates[1].Check);

            Assert.IsTrue(
                lookupCache.ContainsKey(firstSource));

            Assert.IsNull(
                lookupCache[firstSource]);

            Assert.IsTrue(
                lookupCache.ContainsKey(secondSource));

            Assert.IsNull(
                lookupCache[secondSource]);
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_MultipleDynamicStrings_ShouldPrepareAllForResolution()
        {
            // Arrange
            const string firstSource =
                "[//WorkflowData/FirstSource]";

            const string secondSource =
                "[//WorkflowData/SecondSource]";

            const string firstResolved =
                "FirstDynamicLookup";

            const string secondResolved =
                "SecondDynamicLookup";

            UpdateResources activity =
                new UpdateResources();

            activity.ActivityExpressionEvaluator.LookupCache[firstSource] =
                firstResolved;

            activity.ActivityExpressionEvaluator.LookupCache[secondSource] =
                secondResolved;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            updates.Add(
                new Definition(
                    firstSource,
                    "[//Target/FirstAttribute]",
                    false));

            updates.Add(
                new Definition(
                    secondSource,
                    "[//Target/SecondAttribute]",
                    true));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                2,
                activity.DynamicStringsForResolution.Count);

            Assert.AreEqual(
                firstResolved,
                activity.DynamicStringsForResolution[0]);

            Assert.AreEqual(
                secondResolved,
                activity.DynamicStringsForResolution[1]);

            List<Definition> preparedUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                2,
                preparedUpdates.Count);

            Assert.AreEqual(
                firstResolved,
                preparedUpdates[0].Left);

            Assert.AreEqual(
                "[//Target/FirstAttribute]",
                preparedUpdates[0].Right);

            Assert.IsFalse(
                preparedUpdates[0].Check);

            Assert.AreEqual(
                secondResolved,
                preparedUpdates[1].Left);

            Assert.AreEqual(
                "[//Target/SecondAttribute]",
                preparedUpdates[1].Right);

            Assert.IsTrue(
                preparedUpdates[1].Check);
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_MixedResults_ShouldPrepareOnlyStringForResolution()
        {
            // Arrange
            const string firstSource =
                "[//WorkflowData/FirstSource]";

            const string secondSource =
                "[//WorkflowData/SecondSource]";

            const string dynamicString =
                "DynamicLookupString";

            UpdateResources activity =
                new UpdateResources();

            activity.ActivityExpressionEvaluator.LookupCache[firstSource] =
                dynamicString;

            activity.ActivityExpressionEvaluator.LookupCache[secondSource] =
                42L;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Definition firstDefinition =
                new Definition(
                    firstSource,
                    "[//Target/FirstAttribute]",
                    false);

            Definition secondDefinition =
                new Definition(
                    secondSource,
                    "[//Target/SecondAttribute]",
                    true);

            updates.Add(
                firstDefinition);

            updates.Add(
                secondDefinition);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                1,
                activity.DynamicStringsForResolution.Count);

            Assert.AreEqual(
                dynamicString,
                activity.DynamicStringsForResolution[0]);

            List<Definition> preparedUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreEqual(
                2,
                preparedUpdates.Count);

            // String result creates a new working definition.
            Assert.AreNotSame(
                firstDefinition,
                preparedUpdates[0]);

            Assert.AreEqual(
                dynamicString,
                preparedUpdates[0].Left);

            Assert.AreEqual(
                "[//Target/FirstAttribute]",
                preparedUpdates[0].Right);

            Assert.IsFalse(
                preparedUpdates[0].Check);

            // Non-string result retains the original definition.
            Assert.AreSame(
                secondDefinition,
                preparedUpdates[1]);

            Assert.AreEqual(
                secondSource,
                preparedUpdates[1].Left);

            Assert.AreEqual(
                "[//Target/SecondAttribute]",
                preparedUpdates[1].Right);

            Assert.IsTrue(
                preparedUpdates[1].Check);
        }

        [TestMethod]
        public void Activities_UpdateResources_PrepareDynamicGrammarResolution_ShouldLoadValueExpressionsIntoLookupCache()
        {
            // Arrange
            const string firstLookup =
                "[//Value/FirstValue]";

            const string secondLookup =
                "[//WorkflowData/SecondValue]";

            UpdateResources activity =
                new UpdateResources();

            activity.ValueExpressions[firstLookup] =
                "First";

            activity.ValueExpressions[secondLookup] =
                "Second";

            activity.ActivityExpressionEvaluator.LookupCache[firstLookup] =
                "OldFirst";

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareDynamicGrammarResolution_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                "First",
                activity.ActivityExpressionEvaluator.LookupCache[firstLookup]);

            Assert.AreEqual(
                "Second",
                activity.ActivityExpressionEvaluator.LookupCache[secondLookup]);
        }

        [TestMethod]
        public void Activities_UpdateResources_ForEachIteration_ChildCompleted_DynamicGrammarDisabled_ShouldPreserveWorkingUpdates()
        {
            // Arrange
            const string modifiedSource =
                "ModifiedDynamicSource";

            const string target =
                "[//WorkflowData/TargetValue]";

            UpdateResources activity =
                new UpdateResources();

            activity.ResolveDynamicGrammar =
                false;

            Hashtable updatesTable =
                new Hashtable();

            updatesTable["Count"] =
                1;

            updatesTable["0:0"] =
                "[//WorkflowData/OriginalSource]";

            updatesTable["0:1"] =
                target;

            updatesTable["0:2"] =
                false;

            activity.UpdatesTable =
                updatesTable;

            List<Definition> updates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Definition workingDefinition =
                new Definition(
                    modifiedSource,
                    target,
                    false);

            updates.Add(
                workingDefinition);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            forEachIteration.InitialChildData =
                new object[]
                {
            "TestValue"
                };

            activity.ActivityExpressionEvaluator.VariableCache[
                ExpressionEvaluator.ReservedVariableBreakIteration] =
                false;

            ReplicatorChildEventArgs arguments =
                new ReplicatorChildEventArgs(
                    "TestValue",
                    new SequenceActivity());

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildCompleted",
                null,
                arguments);

            // Assert
            List<Definition> resultingUpdates =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "updates");

            Assert.AreSame(
                updates,
                resultingUpdates);

            Assert.AreEqual(
                1,
                resultingUpdates.Count);

            Assert.AreSame(
                workingDefinition,
                resultingUpdates[0]);

            Assert.AreEqual(
                modifiedSource,
                resultingUpdates[0].Left);
        }

    }
}
