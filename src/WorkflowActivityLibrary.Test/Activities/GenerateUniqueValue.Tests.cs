using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using System.Reflection;
using System.Collections.Generic;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using System.Workflow.Activities;
using System.Collections;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class GenerateUniqueValueTests
    {
        [TestMethod]
        public void Activities_GenerateUniqueValue_Constructor_Should_InitializeRequiredObjects()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            Assert.IsNotNull(activity.ActivityExpressionEvaluator);
            Assert.IsNotNull(activity.FoundIds);
            Assert.IsNotNull(activity.FoundResources);
            Assert.IsNotNull(activity.LdapQueries);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_ResolveValueFilter_Should_ReplaceAllValueLookups()
        {
            string filter =
                "/Person[AccountName='[//Value]' or DisplayName='[//value]']";

            object result =
                ActivityTestHelper.InvokePrivateStaticMethod(
                    typeof(GenerateUniqueValue),
                    "ResolveValueFilter",
                    filter,
                    "TestUser");

            string resolvedFilter =
                (string)result;

            Assert.AreEqual(
                "/Person[AccountName='TestUser' or DisplayName='TestUser']",
                resolvedFilter);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_ResolveUniquenessKey_Should_ReplaceAllUniquenessKeyLookups()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            string expression =
                "\"User\" + [//UniquenessKey] + \"-\" + [//uniquenesskey]";

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ResolveUniquenessKey",
                    expression,
                    5,
                    false);

            string resolvedExpression =
                (string)result;

            Assert.AreEqual(
                "\"User\" + \"5\" + \"-\" + \"5\"",
                resolvedExpression);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_ResolveUniquenessKey_OptimizeAndReset_Should_ReplaceWithEmptyString()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                true);

            string expression =
                "\"User\" + [//UniquenessKey]";

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ResolveUniquenessKey",
                    expression,
                    5,
                    true);

            string resolvedExpression =
                (string)result;

            Assert.AreEqual(
                "\"User\" + \"\"",
                resolvedExpression);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_IsValueUnique_NoConflicts_Should_ReturnTrue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "IsValueUnique");

            bool isUnique =
                (bool)result;

            Assert.IsTrue(isUnique);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_IsValueUnique_LdapConflict_Should_ReturnFalse()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            ActivityTestHelper.SetField<bool>(
                activity,
                "ldapConflict",
                true);

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "IsValueUnique");

            bool isUnique =
                (bool)result;

            Assert.IsFalse(isUnique);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_IsValueUnique_ResourceConflictWithoutOptimization_Should_ReturnFalse()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.FoundIds.Add(
                Guid.NewGuid());

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                false);

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "IsValueUnique");

            bool isUnique =
                (bool)result;

            Assert.IsFalse(isUnique);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_Decide_NoConflicts_Should_PublishResolvedValue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.PublicationTarget = "$UniqueValue";

            ActivityTestHelper.SetField<string>(
                activity,
                "resolvedValue",
                "TestUser");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Decide_ExecuteCode",
                activity,
                EventArgs.Empty);

            object publish =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "Publish");

            PropertyInfo updateDefinitionsProperty =
                publish.GetType().GetProperty(
                    "UpdateLookupDefinitions",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                updateDefinitionsProperty,
                "UpdateLookupDefinitions property was not found.");

            List<UpdateLookupDefinition> updateDefinitions =
                (List<UpdateLookupDefinition>)
                    updateDefinitionsProperty.GetValue(
                        publish,
                        null);

            Assert.IsNotNull(updateDefinitions);

            Assert.AreEqual(
                1,
                updateDefinitions.Count);

            UpdateLookupDefinition update =
                updateDefinitions[0];

            Assert.AreEqual(
                "$UniqueValue",
                update.TargetLookup);

            Assert.AreEqual(
                "TestUser",
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_WhileNotUnique_AfterUniqueValueFound_Should_ReturnFalse()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.PublicationTarget = "$UniqueValue";

            ActivityTestHelper.SetField<string>(
                activity,
                "resolvedValue",
                "TestUser");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Decide_ExecuteCode",
                activity,
                EventArgs.Empty);

            ConditionalEventArgs args =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "WhileNotUnique_Condition",
                activity,
                args);

            Assert.IsFalse(args.Result);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_WhileNotUnique_BeforeUniqueValueFound_Should_ReturnTrue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            ConditionalEventArgs args =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "WhileNotUnique_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_SetAttributesToRead_StartsWithFilter_Should_SetConflictAttributes()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ConflictFilter =
                "/Person[starts-with(AccountName,'[//Value]')]";

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                true);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "SetAttributesToReadForConflictResources");

            object findConflict =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindConflict");

            PropertyInfo attributesProperty =
                findConflict.GetType().GetProperty(
                    "Attributes",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                attributesProperty,
                "Attributes property was not found.");

            string[] attributes =
                (string[])attributesProperty.GetValue(
                    findConflict,
                    null);

            Assert.IsNotNull(attributes);

            Assert.AreEqual(
                1,
                attributes.Length);

            Assert.AreEqual(
                "AccountName",
                attributes[0]);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_SetAttributesToRead_NoStartsWithFilter_Should_DisableOptimization()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                true);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "SetAttributesToReadForConflictResources");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ResolveUniquenessKey",
                    "[//UniquenessKey]",
                    5,
                    true);

            string resolvedExpression =
                (string)result;

            Assert.AreEqual(
                "\"5\"",
                resolvedExpression);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_Prepare_Should_SelectFirstNonNullValue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.UniquenessSeed = 1;
            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            activity.ValueExpressions = new ArrayList();
            activity.ValueExpressions.Add("[//Value1]");
            activity.ValueExpressions.Add("[//Value2]");

            activity.ActivityExpressionEvaluator.LookupCache["[//Value1]"] =
                null;

            activity.ActivityExpressionEvaluator.LookupCache["[//Value2]"] =
                "TestUser";

            activity.ActivityExpressionEvaluator.ParseExpression(
                "[//Value1]");

            activity.ActivityExpressionEvaluator.ParseExpression(
                "[//Value2]");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                "/Person[AccountName='TestUser']",
                activity.XPathConflictFilter);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_ParseExpressions_QueryLdapEnabled_Should_ConvertLdapQueries()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ValueExpressions = new ArrayList();
            activity.QueryLdap = true;

            Hashtable ldapQueriesTable = new Hashtable();
            ldapQueriesTable["Count"] = 1;
            ldapQueriesTable["0:0"] = "LDAP://DC=example,DC=com";
            ldapQueriesTable["0:1"] = "(sAMAccountName=[//Value])";
            ldapQueriesTable["0:2"] = false;

            activity.LdapQueriesTable = ldapQueriesTable;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseExpressions_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNotNull(activity.LdapQueries);

            Assert.AreEqual(
                1,
                activity.LdapQueries.Count);

            Definition ldapQuery =
                activity.LdapQueries[0];

            Assert.AreEqual(
                "LDAP://DC=example,DC=com",
                ldapQuery.Left);

            Assert.AreEqual(
                "(sAMAccountName=[//Value])",
                ldapQuery.Right);

            Assert.IsFalse(
                ldapQuery.Check);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_ExecutionCondition_Should_ReturnExpectedResult()
        {
            object[,] testCases = new object[,]
            {
        { null, true },
        { "true", true },
        { "false", false }
            };

            for (int i = 0; i < testCases.GetLength(0); i++)
            {
                GenerateUniqueValue activity =
                    new GenerateUniqueValue();

                activity.ActivityExecutionCondition =
                    (string)testCases[i, 0];

                bool expected =
                    (bool)testCases[i, 1];

                activity.ActivityExpressionEvaluator.ParseIfExpression(
                    activity.ActivityExecutionCondition);

                ConditionalEventArgs args =
                    new ConditionalEventArgs();

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ActivityExecutionConditionSatisfied_Condition",
                    activity,
                    args);

                Assert.AreEqual(
                    expected,
                    args.Result,
                    "Unexpected result for condition '" +
                    Convert.ToString(activity.ActivityExecutionCondition) +
                    "'.");
            }
        }


        [TestMethod]
        public void Activities_GenerateUniqueValue_Decide_ConflictWithNextExpression_Should_UseNextValue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            activity.ValueExpressions = new ArrayList();
            activity.ValueExpressions.Add("\"TestUser\"");
            activity.ValueExpressions.Add("\"AlternativeUser\"");

            activity.FoundIds.Add(
                Guid.NewGuid());

            ActivityTestHelper.SetField<string>(
                activity,
                "resolvedValue",
                "TestUser");

            ActivityTestHelper.SetField<int>(
                activity,
                "valueIndex",
                0);

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                false);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Decide_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                "/Person[AccountName='AlternativeUser']",
                activity.XPathConflictFilter);

            ConditionalEventArgs args =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "WhileNotUnique_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }



        [TestMethod]
        public void Activities_GenerateUniqueValue_Decide_ConflictOnLastExpression_Should_IncrementUniquenessKey()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            activity.ValueExpressions = new ArrayList();
            activity.ValueExpressions.Add(
                "\"TestUser\" + [//UniquenessKey]");

            activity.FoundIds.Add(
                Guid.NewGuid());

            ActivityTestHelper.SetField<string>(
                activity,
                "resolvedValue",
                "TestUser1");

            ActivityTestHelper.SetField<int>(
                activity,
                "valueIndex",
                0);

            ActivityTestHelper.SetField<int>(
                activity,
                "uniquenessKey",
                1);

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                false);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Decide_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                "/Person[AccountName='TestUser2']",
                activity.XPathConflictFilter);

            ConditionalEventArgs args =
                new ConditionalEventArgs();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "WhileNotUnique_Condition",
                activity,
                args);

            Assert.IsTrue(args.Result);
        }


        [TestMethod]
        public void Activities_GenerateUniqueValue_Decide_MaxLoopCountExceeded_Should_ThrowException()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            activity.ValueExpressions = new ArrayList();
            activity.ValueExpressions.Add(
                "\"TestUser\" + [//UniquenessKey]");

            activity.FoundIds.Add(
                Guid.NewGuid());

            ActivityTestHelper.SetField<string>(
                activity,
                "resolvedValue",
                "TestUser1");

            ActivityTestHelper.SetField<int>(
                activity,
                "valueIndex",
                0);

            ActivityTestHelper.SetField<int>(
                activity,
                "uniquenessKey",
                1);

            ActivityTestHelper.SetField<int>(
                activity,
                "maxLoopCount",
                0);

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                false);

            try
            {
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "Decide_ExecuteCode",
                    activity,
                    EventArgs.Empty);

                Assert.Fail(
                    "Expected an exception when the maximum loop count was exceeded.");
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
        public void Activities_GenerateUniqueValue_OnSequenceComplete_Should_ClearVariableCacheValues()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ActivityExpressionEvaluator.VariableCache["$GeneratedValue"] =
                "TestUser";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "OnSequenceComplete",
                new object[] { null });

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$GeneratedValue"));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$GeneratedValue"]);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_Decide_LdapConflict_Should_ResetConflictForNextValue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            activity.ValueExpressions = new ArrayList();
            activity.ValueExpressions.Add("\"TestUser\"");
            activity.ValueExpressions.Add("\"AlternativeUser\"");

            ActivityTestHelper.SetField<string>(
                activity,
                "resolvedValue",
                "TestUser");

            ActivityTestHelper.SetField<int>(
                activity,
                "valueIndex",
                0);

            ActivityTestHelper.SetField<bool>(
                activity,
                "ldapConflict",
                true);

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                false);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Decide_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                "/Person[AccountName='AlternativeUser']",
                activity.XPathConflictFilter);

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "IsValueUnique");

            bool isUnique =
                (bool)result;

            Assert.IsTrue(isUnique);
        }


        [TestMethod]
        public void Activities_GenerateUniqueValue_IsValueUnique_ResourceFoundWithOptimizationAndNoActualConflict_Should_ReturnTrue()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.FoundIds.Add(
                Guid.NewGuid());

            ActivityTestHelper.SetField<bool>(
                activity,
                "optimizeUniquenessKey",
                true);

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "IsValueUnique");

            bool isUnique =
                (bool)result;

            Assert.IsTrue(isUnique);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_Prepare_WithUniquenessSeed_Should_CreateInitialCandidate()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.UniquenessSeed = 3;

            activity.ConflictFilter =
                "/Person[AccountName='[//Value]']";

            activity.ValueExpressions =
                new ArrayList();

            activity.ValueExpressions.Add(
                "\"TestUser\" + [//UniquenessKey]");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                "/Person[AccountName='TestUser3']",
                activity.XPathConflictFilter);
        }

        [TestMethod]
        public void Activities_GenerateUniqueValue_ParseExpressions_Should_ParseValueExpressionWithoutUniquenessKeyLookup()
        {
            GenerateUniqueValue activity = new GenerateUniqueValue();

            activity.ValueExpressions =
                new ArrayList();

            activity.ValueExpressions.Add(
                "[//Value/AccountName] + [//UniquenessKey]");

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseExpressions_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    "[//Value/AccountName]"));

            Assert.IsFalse(
                activity.ActivityExpressionEvaluator.LookupCache.ContainsKey(
                    "[//UniquenessKey]"));
        }


    }


}
