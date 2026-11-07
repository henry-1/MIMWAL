using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using System.Collections;
using System.Workflow.Activities;
using System.Collections.Generic;

using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using System.Reflection;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.Activities
{
    /// <summary>
    /// Testing:
    ///  * constructor initialization
    ///  * dependency properties
    ///  * advanced-setting reset/preservation
    ///  * attribute and query definition conversion
    ///  * condition and iteration preparation
    ///  * scalar, list, inserted, removed, attribute, and variable targets
    ///  * conflict detection and publication
    ///  * created-resource publication
    ///  * authorization and actor conditions
    ///  * iteration stopping and break behavior
    ///  * Finish_ExecuteCode
    ///  * variable-cache initialization and cleanup
    /// </summary>
    [TestClass]
    public class CreateResourceTests
    {
        [TestMethod]
        public void Activities_CreateResource_Constructor_Should_InitializeRequiredObjects()
        {
            CreateResource activity = new CreateResource();

            Assert.IsNotNull(activity.Conflicts);
            Assert.IsNotNull(activity.ActivityExpressionEvaluator);
            Assert.IsNotNull(activity.LookupUpdates);
            Assert.IsNotNull(activity.Queries);
            Assert.IsNotNull(activity.ValueExpressions);
        }


        [TestMethod]
        public void Activities_CreateResource_ValueExpressions_Should_BeCaseInsensitive()
        {
            CreateResource activity = new CreateResource();

            activity.ValueExpressions["TestExpression"] = "Value";

            Assert.IsTrue(
                activity.ValueExpressions.ContainsKey("testexpression"));

            Assert.AreEqual(
                "Value",
                activity.ValueExpressions["TESTEXPRESSION"]);
        }

        [TestMethod]
        public void Activities_CreateResource_DependencyProperties_Should_PreserveAssignedValues()
        {
            CreateResource activity = new CreateResource();

            Hashtable attributes = new Hashtable();
            attributes.Add("DisplayName", "Test");

            activity.ActivityDisplayName = "Create Test Resource";
            activity.ResourceType = "Person";
            activity.Advanced = true;
            activity.AttributesTable = attributes;
            activity.ActorType = ActorType.Service;

            Assert.AreEqual(
                "Create Test Resource",
                activity.ActivityDisplayName);

            Assert.AreEqual(
                "Person",
                activity.ResourceType);

            Assert.IsTrue(
                activity.Advanced);

            Assert.AreSame(
                attributes,
                activity.AttributesTable);

            Assert.AreEqual(
                ActorType.Service,
                activity.ActorType);
        }

        [TestMethod]
        public void Activities_CreateResource_ParseDefinitions_NotAdvanced_Should_ClearAdvancedSettings()
        {
            CreateResource activity = new CreateResource();

            activity.Advanced = false;
            activity.QueryResources = true;
            activity.CreatedResourceIdTarget = "$CreatedId";
            activity.CheckForConflict = true;
            activity.ActivityExecutionCondition = "true";
            activity.Iteration = "$Values";
            activity.ActorType = ActorType.Requestor;
            activity.ApplyAuthorizationPolicy = true;

            activity.AttributesTable =
                ActivityTestHelper.CreateEmptyDefinitionsTable();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsFalse(activity.QueryResources);
            Assert.IsNull(activity.CreatedResourceIdTarget);
            Assert.IsFalse(activity.CheckForConflict);
            Assert.IsNull(activity.ActivityExecutionCondition);
            Assert.IsNull(activity.Iteration);
            Assert.AreEqual(ActorType.Service, activity.ActorType);
            Assert.IsFalse(activity.ApplyAuthorizationPolicy);
        }

        [TestMethod]
        public void Activities_CreateResource_ParseDefinitions_Advanced_Should_PreserveAdvancedSettings()
        {
            CreateResource activity = new CreateResource();

            activity.Advanced = true;
            activity.QueryResources = false;
            activity.CreatedResourceIdTarget = "$CreatedId";
            activity.CheckForConflict = true;
            activity.ActivityExecutionCondition = "true";
            activity.Iteration = "$Values";
            activity.ActorType = ActorType.Requestor;
            activity.ApplyAuthorizationPolicy = true;

            activity.AttributesTable =
                ActivityTestHelper.CreateEmptyDefinitionsTable();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsFalse(activity.QueryResources);
            Assert.AreEqual("$CreatedId", activity.CreatedResourceIdTarget);
            Assert.IsTrue(activity.CheckForConflict);
            Assert.AreEqual("true", activity.ActivityExecutionCondition);
            Assert.AreEqual("$Values", activity.Iteration);
            Assert.AreEqual(ActorType.Requestor, activity.ActorType);
            Assert.IsTrue(activity.ApplyAuthorizationPolicy);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_NoIteration_Should_CreateSingleNullIteration()
        {
            CreateResource activity = new CreateResource();

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

            Assert.IsNotNull(forEachIteration.InitialChildData);
            Assert.AreEqual(1, forEachIteration.InitialChildData.Count);
            Assert.IsNull(forEachIteration.InitialChildData[0]);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_FalseCondition_Should_NotCreateIterations()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExecutionCondition = "false";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.ActivityExecutionCondition);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNull(forEachIteration.InitialChildData);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_TrueCondition_Should_CreateIteration()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExecutionCondition = "true";
            activity.Iteration = null;

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.ActivityExecutionCondition);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(forEachIteration.InitialChildData);
            Assert.AreEqual(1, forEachIteration.InitialChildData.Count);
            Assert.IsNull(forEachIteration.InitialChildData[0]);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_ListExpression_Should_CreateIterationForEachValue()
        {
            CreateResource activity = new CreateResource();

            List<string> expectedValues = new List<string>();
            expectedValues.Add("Value1");
            expectedValues.Add("Value2");
            expectedValues.Add("Value3");

            activity.ActivityExpressionEvaluator.VariableCache["$Values"] =
                expectedValues;

            activity.Iteration = "$Values";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Iteration);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(forEachIteration.InitialChildData);
            Assert.AreEqual(3, forEachIteration.InitialChildData.Count);
            Assert.AreEqual("Value1", forEachIteration.InitialChildData[0]);
            Assert.AreEqual("Value2", forEachIteration.InitialChildData[1]);
            Assert.AreEqual("Value3", forEachIteration.InitialChildData[2]);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_SingleValueExpression_Should_CreateSingleIteration()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExpressionEvaluator.VariableCache["$Value"] =
                "Value1";

            activity.Iteration = "$Value";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Iteration);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(forEachIteration.InitialChildData);
            Assert.AreEqual(1, forEachIteration.InitialChildData.Count);
            Assert.AreEqual(
                "Value1",
                forEachIteration.InitialChildData[0]);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_NullExpressionResult_Should_CreateNoIterations()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExpressionEvaluator.VariableCache["$Value"] = null;
            activity.Iteration = "$Value";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Iteration);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(forEachIteration.InitialChildData);
            Assert.AreEqual(
                0,
                forEachIteration.InitialChildData.Count);
        }

        [TestMethod]
        public void Activities_CreateResource_CheckConflict_EnabledWithFilter_Should_ReturnTrue()
        {
            CreateResource activity = new CreateResource();

            activity.CheckForConflict = true;
            activity.ConflictFilter = "/Person[AccountName='test']";

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckConflict_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_CheckConflict_DisabledWithFilter_Should_ReturnFalse()
        {
            CreateResource activity = new CreateResource();

            activity.CheckForConflict = false;
            activity.ConflictFilter = "/Person[AccountName='test']";

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckConflict_Condition",
                activity,
                args);

            Assert.IsFalse(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_CheckConflict_EnabledWithoutFilter_Should_ReturnFalse()
        {
            CreateResource activity = new CreateResource();

            activity.CheckForConflict = true;
            activity.ConflictFilter = null;

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckConflict_Condition",
                activity,
                args);

            Assert.IsFalse(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_Unique_NoConflict_Should_ReturnTrue()
        {
            CreateResource activity = new CreateResource();
            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Unique_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_Unique_ConflictFound_Should_ReturnFalse()
        {
            CreateResource activity = new CreateResource();

            activity.Conflicts.Add(Guid.NewGuid());
            activity.ConflictingResourceIdTarget = null;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Unique_Condition",
                activity,
                args);

            Assert.IsFalse(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_ContentToPublish_NoUpdates_Should_ReturnFalse()
        {
            CreateResource activity = new CreateResource();
            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ContentToPublish_Condition",
                activity,
                args);

            Assert.IsFalse(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_ContentToPublish_WithUpdate_Should_ReturnTrue()
        {
            CreateResource activity = new CreateResource();

            activity.LookupUpdates.Add(
                new UpdateLookupDefinition(
                    "$CreatedResourceId",
                    Guid.NewGuid(),
                    UpdateMode.Modify));

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ContentToPublish_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_Authorization_Enabled_Should_ReturnTrue()
        {
            CreateResource activity = new CreateResource();
            activity.ApplyAuthorizationPolicy = true;

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Authorization_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_Authorization_Should_ReturnConfiguredValue()
        {
            bool[] testCases = new bool[] { true, false };

            foreach (bool expected in testCases)
            {
                CreateResource activity = new CreateResource();
                activity.ApplyAuthorizationPolicy = expected;

                ConditionalEventArgs args = new ConditionalEventArgs();

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "Authorization_Condition",
                    activity,
                    args);

                Assert.AreEqual(expected, args.Result);
            }
        }

        [TestMethod]
        public void Activities_CreateResource_ActorIsNotValueExpression_Should_ReturnExpectedResult()
        {
            object[,] testCases = new object[,]
            {
                { "SomeActor", true },
                { "[//Value]", false }
            };

            for (int i = 0; i < testCases.GetLength(0); i++)
            {
                CreateResource activity = new CreateResource();
                activity.ActorString = (string)testCases[i, 0];

                ConditionalEventArgs args = new ConditionalEventArgs();

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ActorIsNotValueExpression_Condition",
                    activity,
                    args);

                Assert.AreEqual(
                    (bool)testCases[i, 1],
                    args.Result,
                    "Unexpected result for ActorString '" +
                    activity.ActorString +
                    "'.");
            }
        }

        [TestMethod]
        public void Activities_CreateResource_ChildInitialized_Should_SetValueAndIncrementIteration()
        {
            CreateResource activity = new CreateResource();

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            List<object> iterationValues = new List<object>();
            iterationValues.Add("Value1");

            forEachIteration.InitialChildData = iterationValues;

            ReplicatorChildEventArgs childArgs =
                new ReplicatorChildEventArgs(
                    "Value1",
                    null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildInitialized",
                activity,
                childArgs);

            Assert.AreEqual(
                "Value1",
                activity.Value);

            ConditionalEventArgs conditionArgs =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_UntilCondition",
                activity,
                conditionArgs);

            Assert.IsTrue(conditionArgs.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_ChildCompleted_BreakIterationTrue_Should_StopIteration()
        {
            CreateResource activity = new CreateResource();

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            List<object> iterationValues = new List<object>();
            iterationValues.Add("Value1");
            iterationValues.Add("Value2");

            forEachIteration.InitialChildData = iterationValues;

            activity.ActivityExpressionEvaluator.VariableCache[
                ExpressionEvaluator.ReservedVariableBreakIteration] = true;

            ReplicatorChildEventArgs childArgs =
                new ReplicatorChildEventArgs(
                    "Value1",
                    null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildCompleted",
                activity,
                childArgs);

            ConditionalEventArgs conditionArgs =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_UntilCondition",
                activity,
                conditionArgs);

            Assert.IsTrue(conditionArgs.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_ChildCompleted_BreakIterationFalse_Should_ContinueIteration()
        {
            CreateResource activity = new CreateResource();

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            List<object> iterationValues = new List<object>();
            iterationValues.Add("Value1");
            iterationValues.Add("Value2");

            forEachIteration.InitialChildData = iterationValues;

            activity.ActivityExpressionEvaluator.VariableCache[
                ExpressionEvaluator.ReservedVariableBreakIteration] = false;

            ReplicatorChildEventArgs childArgs =
                new ReplicatorChildEventArgs(
                    "Value1",
                    null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildCompleted",
                activity,
                childArgs);

            ConditionalEventArgs conditionArgs =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_UntilCondition",
                activity,
                conditionArgs);

            Assert.IsFalse(conditionArgs.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_EvaluateResults_NoConflicts_Should_RemainUnique()
        {
            CreateResource activity = new CreateResource();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            ConditionalEventArgs args = new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Unique_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        
        [TestMethod]
        public void Activities_CreateResource_PublishCreated_ValidResourceId_Should_AddLookupUpdate()
        {
            CreateResource activity = new CreateResource();

            Guid createdResourceId = Guid.NewGuid();

            activity.CreatedResourceIdTarget = "$CreatedResourceId";
            activity.CreatedResourceId = createdResourceId;
            activity.Value = null;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PublishCreated_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                1,
                activity.LookupUpdates.Count);

            UpdateLookupDefinition update =
                activity.LookupUpdates[0];

            Assert.AreEqual(
                "$CreatedResourceId",
                update.TargetLookup);

            Assert.AreEqual(
                createdResourceId,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_CreateResource_PublishCreated_EmptyResourceId_Should_NotAddLookupUpdate()
        {
            CreateResource activity = new CreateResource();

            activity.CreatedResourceIdTarget = "$CreatedResourceId";
            activity.CreatedResourceId = Guid.Empty;
            activity.Value = null;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PublishCreated_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                0,
                activity.LookupUpdates.Count);
        }

        [TestMethod]
        public void Activities_CreateResource_PublishCreated_DuringIteration_Should_NotAddLookupUpdate()
        {
            CreateResource activity = new CreateResource();

            activity.CreatedResourceIdTarget = "$CreatedResourceId";
            activity.CreatedResourceId = Guid.NewGuid();
            activity.Value = "CurrentIterationValue";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PublishCreated_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                0,
                activity.LookupUpdates.Count);
        }

        [TestMethod]
        public void Activities_CreateResource_PublishCreated_NoTarget_Should_NotAddLookupUpdate()
        {
            CreateResource activity = new CreateResource();

            activity.CreatedResourceIdTarget = null;
            activity.CreatedResourceId = Guid.NewGuid();
            activity.Value = null;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PublishCreated_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                0,
                activity.LookupUpdates.Count);
        }

        [TestMethod]
        public void Activities_CreateResource_Finish_ConflictFoundAndFailEnabled_Should_ThrowException()
        {
            CreateResource activity = new CreateResource();

            activity.Conflicts.Add(Guid.NewGuid());
            activity.ConflictingResourceIdTarget = null;
            activity.FailOnConflict = true;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            try
            {
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "Finish_ExecuteCode",
                    activity,
                    EventArgs.Empty);

                Assert.Fail(
                    "Expected an exception when a conflict was found and FailOnConflict was enabled.");
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
        public void Activities_CreateResource_Finish_ConflictFoundAndFailDisabled_Should_NotThrow()
        {
            CreateResource activity = new CreateResource();

            activity.Conflicts.Add(Guid.NewGuid());
            activity.ConflictingResourceIdTarget = null;
            activity.FailOnConflict = false;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Finish_ExecuteCode",
                activity,
                EventArgs.Empty);
        }

        [TestMethod]
        public void Activities_CreateResource_Finish_NoConflictAndFailEnabled_Should_NotThrow()
        {
            CreateResource activity = new CreateResource();

            activity.FailOnConflict = true;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Finish_ExecuteCode",
                activity,
                EventArgs.Empty);
        }

      

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_NoAttributes_Should_CreateObjectTypeParameter()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.CreateParameters);

            Assert.AreEqual(
                1,
                activity.CreateParameters.Length);

            CreateRequestParameter parameter =
                activity.CreateParameters[0];

            Assert.AreEqual(
                "ObjectType",
                parameter.PropertyName);

            Assert.AreEqual(
                "Person",
                parameter.Value);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_ScalarAttribute_Should_CreateAttributeParameter()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            activity.ValueExpressions["[//Value]"] = "Test User";

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "DisplayName";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.CreateParameters);
            Assert.AreEqual(2, activity.CreateParameters.Length);

            CreateRequestParameter objectTypeParameter =
                activity.CreateParameters[0];

            CreateRequestParameter displayNameParameter =
                activity.CreateParameters[1];

            Assert.AreEqual(
                "ObjectType",
                objectTypeParameter.PropertyName);

            Assert.AreEqual(
                "Person",
                objectTypeParameter.Value);

            Assert.AreEqual(
                "DisplayName",
                displayNameParameter.PropertyName);

            Assert.AreEqual(
                "Test User",
                displayNameParameter.Value);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_NullAttributeValue_Should_SkipAttributeParameter()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            activity.ValueExpressions["[//Value]"] = null;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "DisplayName";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.CreateParameters);

            Assert.AreEqual(
                1,
                activity.CreateParameters.Length);

            CreateRequestParameter parameter =
                activity.CreateParameters[0];

            Assert.AreEqual(
                "ObjectType",
                parameter.PropertyName);

            Assert.AreEqual(
                "Person",
                parameter.Value);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_ListAttribute_Should_CreateParameterForEachValue()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            List<string> values = new List<string>();
            values.Add("Value1");
            values.Add("Value2");

            activity.ValueExpressions["[//Value]"] = values;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "ProxyAddress";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.CreateParameters);
            Assert.AreEqual(3, activity.CreateParameters.Length);

            CreateRequestParameter firstValueParameter =
                activity.CreateParameters[1];

            CreateRequestParameter secondValueParameter =
                activity.CreateParameters[2];

            Assert.AreEqual(
                "ProxyAddress",
                firstValueParameter.PropertyName);

            Assert.AreEqual(
                "Value1",
                firstValueParameter.Value);

            Assert.AreEqual(
                "ProxyAddress",
                secondValueParameter.PropertyName);

            Assert.AreEqual(
                "Value2",
                secondValueParameter.Value);
        }

        

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_ListVariable_Should_PublishCompleteList()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            List<string> values = new List<string>();
            values.Add("Value1");
            values.Add("Value2");

            activity.ValueExpressions["[//Value]"] = values;

            // PublishVariable only updates variables that already exist in VariableCache.
            activity.ActivityExpressionEvaluator.VariableCache["$TargetValues"] = null;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "$TargetValues";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            // Only the mandatory ObjectType parameter should be created.
            Assert.AreEqual(
                1,
                activity.CreateParameters.Length);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$TargetValues"));

            object publishedValue =
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$TargetValues"];

            Assert.AreSame(
                values,
                publishedValue);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_InsertedValuesAttribute_Should_CreateParameterForEachValue()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            InsertedValuesCollection values =
                new InsertedValuesCollection();

            values.Add("Value1");
            values.Add("Value2");

            activity.ValueExpressions["[//Value]"] = values;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "ProxyAddress";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.CreateParameters);

            Assert.AreEqual(
                3,
                activity.CreateParameters.Length);

            CreateRequestParameter firstValueParameter =
                activity.CreateParameters[1];

            CreateRequestParameter secondValueParameter =
                activity.CreateParameters[2];

            Assert.AreEqual(
                "ProxyAddress",
                firstValueParameter.PropertyName);

            Assert.AreEqual(
                "Value1",
                firstValueParameter.Value);

            Assert.AreEqual(
                "ProxyAddress",
                secondValueParameter.PropertyName);

            Assert.AreEqual(
                "Value2",
                secondValueParameter.Value);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_InsertedValuesVariable_Should_InsertAllValues()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            InsertedValuesCollection values =
                new InsertedValuesCollection();

            values.Add("Value1");
            values.Add("Value2");

            activity.ValueExpressions["[//Value]"] = values;

            // PublishVariable only updates variables that already exist.
            activity.ActivityExpressionEvaluator.VariableCache["$TargetValues"] =
                null;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "$TargetValues";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            // Only the mandatory ObjectType parameter should be created.
            Assert.AreEqual(
                1,
                activity.CreateParameters.Length);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$TargetValues"));

            object publishedValue =
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$TargetValues"];

            Assert.IsInstanceOfType(
                publishedValue,
                typeof(List<string>));

            List<string> publishedValues =
                (List<string>)publishedValue;

            Assert.AreEqual(
                2,
                publishedValues.Count);

            Assert.AreEqual(
                "Value1",
                publishedValues[0]);

            Assert.AreEqual(
                "Value2",
                publishedValues[1]);
        }

        [TestMethod]
        public void Activities_CreateResource_ApplyRemovedValuesCollection_VariableTarget_Should_RemoveValues()
        {
            CreateResource activity = new CreateResource();

            List<string> existingValues = new List<string>();
            existingValues.Add("Value1");
            existingValues.Add("Value2");

            activity.ActivityExpressionEvaluator.VariableCache["$TargetValues"] =
                existingValues;

            RemovedValuesCollection removedValues =
                new RemovedValuesCollection();

            removedValues.Add("Value1");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ApplyRemovedValuesCollection",
                "$TargetValues",
                true,
                removedValues);

            object publishedValue =
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$TargetValues"];

            Assert.AreEqual(
                "Value2",
                publishedValue);
        }

        [TestMethod]
        public void Activities_CreateResource_ApplyRemovedValuesCollection_AttributeTarget_Should_NotThrow()
        {
            CreateResource activity = new CreateResource();

            RemovedValuesCollection removedValues =
                new RemovedValuesCollection();

            removedValues.Add("Value1");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ApplyRemovedValuesCollection",
                "ProxyAddress",
                false,
                removedValues);
        }

        [TestMethod]
        public void Activities_CreateResource_IsVariableTarget_Should_ReturnExpectedResult()
        {
            object[,] testCases = new object[,]
            {
                { "$TargetValues", true },
                { "ProxyAddress", false },
                { null, false }
            };

            for (int i = 0; i < testCases.GetLength(0); i++)
            {
                string target =
                    (string)testCases[i, 0];

                bool expected =
                    (bool)testCases[i, 1];

                object result =
                    ActivityTestHelper.InvokePrivateStaticMethod(
                        typeof(CreateResource),
                        "IsVariableTarget",
                        target);

                Assert.AreEqual(
                    expected,
                    (bool)result,
                    "Unexpected result for target '" +
                    Convert.ToString(target) +
                    "'.");
            }
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareCreate_ScalarVariable_Should_PublishValue()
        {
            CreateResource activity = new CreateResource();
            activity.ResourceType = "Person";

            activity.ValueExpressions["[//Value]"] =
                "Test User";

            // PublishVariable only updates variables that already exist.
            activity.ActivityExpressionEvaluator.VariableCache["$TargetValue"] =
                null;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "$TargetValue";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareCreate_ExecuteCode",
                activity,
                EventArgs.Empty);

            // Only the mandatory ObjectType parameter should be created.
            Assert.AreEqual(
                1,
                activity.CreateParameters.Length);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$TargetValue"));

            object publishedValue =
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$TargetValue"];

            Assert.AreEqual(
                "Test User",
                publishedValue);
        }

        [TestMethod]
        public void Activities_CreateResource_EvaluateResults_ConflictWithTarget_Should_AddLookupUpdate()
        {
            CreateResource activity = new CreateResource();

            Guid conflictId = Guid.NewGuid();

            activity.Conflicts.Add(conflictId);
            activity.ConflictingResourceIdTarget = "$ConflictingResourceId";
            activity.Value = null;

            object findConflict =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindConflict");

            PropertyInfo foundIdsProperty =
                findConflict.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds = new List<Guid>();
            foundIds.Add(conflictId);

            foundIdsProperty.SetValue(
                findConflict,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                1,
                activity.LookupUpdates.Count);

            UpdateLookupDefinition update =
                activity.LookupUpdates[0];

            Assert.AreEqual(
                "$ConflictingResourceId",
                update.TargetLookup);

            Assert.AreEqual(
                conflictId,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_CreateResource_EvaluateResults_ConflictDuringIteration_Should_NotAddLookupUpdate()
        {
            CreateResource activity = new CreateResource();

            Guid conflictId = Guid.NewGuid();

            activity.Conflicts.Add(conflictId);
            activity.ConflictingResourceIdTarget = "$ConflictingResourceId";
            activity.Value = "CurrentIterationValue";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EvaluateResults_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                0,
                activity.LookupUpdates.Count);

            ConditionalEventArgs args =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Unique_Condition",
                activity,
                args);

            Assert.IsFalse(args.Result);
        }

        
        [TestMethod]
        public void Activities_CreateResource_ParseDefinitions_QueryResourcesEnabled_Should_ConvertQueriesTable()
        {
            CreateResource activity = new CreateResource();

            activity.Advanced = true;
            activity.QueryResources = true;

            activity.AttributesTable =
                ActivityTestHelper.CreateEmptyDefinitionsTable();

            Hashtable queriesTable = new Hashtable();
            queriesTable["Count"] = 1;
            queriesTable["0:0"] = "/Person";
            queriesTable["0:1"] = "People";
            queriesTable["0:2"] = false;

            activity.QueriesTable = queriesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.Queries);

            Assert.AreEqual(
                1,
                activity.Queries.Count);

            Definition query =
                activity.Queries[0];

            Assert.AreEqual(
                "/Person",
                query.Left);

            Assert.AreEqual(
                "People",
                query.Right);

            Assert.IsFalse(
                query.Check);
        }

        [TestMethod]
        public void Activities_CreateResource_OnSequenceComplete_Should_ClearVariableCacheValues()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExpressionEvaluator.VariableCache["$Name"] =
                "Test User";

            activity.ActivityExpressionEvaluator.VariableCache["$Values"] =
                new List<string>
                {
                    "Value1",
                    "Value2"
                };

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "OnSequenceComplete",
                new object[] { null });

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$Name"));

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$Values"));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["$Name"]);

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["$Values"]);
        }

        [TestMethod]
        public void Activities_CreateResource_UntilCondition_NoInitialChildData_Should_ReturnTrue()
        {
            CreateResource activity = new CreateResource();

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            forEachIteration.InitialChildData = null;

            ConditionalEventArgs args =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_UntilCondition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_ValueLookup_Should_AddValueExpression()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExpressionEvaluator.VariableCache["$IterationValue"] =
                "Value1";

            activity.Iteration = "$IterationValue";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Iteration);

            activity.ActivityExpressionEvaluator.LookupCache["[//Value]"] =
                "PreviousValue";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsTrue(
                activity.ValueExpressions.ContainsKey(
                    "[//Value]"));

            Assert.IsNull(
                activity.ValueExpressions["[//Value]"]);
        }

        [TestMethod]
        public void Activities_CreateResource_ParseDefinitions_AttributesTable_Should_ConvertDefinitions()
        {
            CreateResource activity = new CreateResource();
            activity.Advanced = true;

            Hashtable attributesTable = new Hashtable();
            attributesTable["Count"] = 1;
            attributesTable["0:0"] = "[//Value]";
            attributesTable["0:1"] = "DisplayName";
            attributesTable["0:2"] = false;

            activity.AttributesTable = attributesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            List<Definition> attributes =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "attributes");

            Assert.AreEqual(
                1,
                attributes.Count);

            Definition attribute =
                attributes[0];

            Assert.AreEqual(
                "[//Value]",
                attribute.Left);

            Assert.AreEqual(
                "DisplayName",
                attribute.Right);

            Assert.IsFalse(
                attribute.Check);
        }

        [TestMethod]
        public void Activities_CreateResource_InitializeExpressionEvaluator_Should_LoadLookupsAndClearVariables()
        {
            CreateResource activity = new CreateResource();

            activity.ValueExpressions["[//Value]"] =
                "CurrentValue";

            activity.ActivityExpressionEvaluator.VariableCache["$TargetValue"] =
                "PreviousValue";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "InitializeExpressionEvaluator");

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    "[//Value]"));

            Assert.AreEqual(
                "CurrentValue",
                activity.ActivityExpressionEvaluator.LookupCache[
                    "[//Value]"]);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$TargetValue"));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$TargetValue"]);
        }

        [TestMethod]
        public void Activities_CreateResource_PrepareIteration_NonBooleanCondition_Should_NotCreateIterations()
        {
            CreateResource activity = new CreateResource();

            activity.ActivityExpressionEvaluator.VariableCache["$Condition"] =
                "true";

            activity.ActivityExecutionCondition = "$Condition";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.ActivityExecutionCondition);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                activity,
                EventArgs.Empty);

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNull(
                forEachIteration.InitialChildData);
        }

        [TestMethod]
        public void Activities_CreateResource_ParseDefinitions_Should_ParseExecutionCondition()
        {
            CreateResource activity = new CreateResource();

            activity.Advanced = true;
            activity.ActivityExecutionCondition = "true";
            activity.Iteration = null;

            activity.AttributesTable =
                ActivityTestHelper.CreateEmptyDefinitionsTable();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

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
        public void Activities_CreateResource_ParseDefinitions_QueryResourcesDisabled_Should_NotConvertQueriesTable()
        {
            CreateResource activity = new CreateResource();

            activity.Advanced = true;
            activity.QueryResources = false;

            activity.AttributesTable =
                ActivityTestHelper.CreateEmptyDefinitionsTable();

            Hashtable queriesTable = new Hashtable();
            queriesTable["Count"] = 1;
            queriesTable["0:0"] = "/Person";
            queriesTable["0:1"] = "People";
            queriesTable["0:2"] = false;

            activity.QueriesTable = queriesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.Queries);

            Assert.AreEqual(
                0,
                activity.Queries.Count);
        }

        [TestMethod]
        public void Activities_CreateResource_ParseDefinitions_Should_ParseIteration()
        {
            CreateResource activity = new CreateResource();

            activity.Advanced = true;

            List<string> expectedValues = new List<string>();
            expectedValues.Add("Value1");
            expectedValues.Add("Value2");

            activity.ActivityExpressionEvaluator.VariableCache["$Values"] =
                expectedValues;

            activity.Iteration = "$Values";

            activity.AttributesTable =
                ActivityTestHelper.CreateEmptyDefinitionsTable();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseDefinitions_ExecuteCode",
                activity,
                EventArgs.Empty);

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
                2,
                forEachIteration.InitialChildData.Count);

            Assert.AreEqual(
                "Value1",
                forEachIteration.InitialChildData[0]);

            Assert.AreEqual(
                "Value2",
                forEachIteration.InitialChildData[1]);
        }



    }
}
