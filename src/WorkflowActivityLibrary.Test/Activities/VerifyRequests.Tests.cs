using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using System.Collections.Generic;
//using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using System.Collections;
using System.Reflection;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using System.Workflow.Activities;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
//using System.Workflow.Activities;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class VerifyRequestsTests
    {
        public object Messages { get; private set; }

        [TestMethod]
        public void Activities_VerifyRequest_ParseConditions_ShouldCreateDefinitionsAndParseExpressions()
        {
            // Arrange
            const string firstCondition =
                "[//WorkflowData/FirstCondition]";

            const string secondCondition =
                "[//WorkflowData/SecondCondition]";

            Hashtable conditionsTable =
                new Hashtable();

            conditionsTable["Count"] =
                2;

            conditionsTable["0:0"] =
                firstCondition;

            conditionsTable["0:1"] =
                "First denial message";

            conditionsTable["0:2"] =
                false;

            conditionsTable["1:0"] =
                secondCondition;

            conditionsTable["1:1"] =
                "Second denial message";

            conditionsTable["1:2"] =
                true;

            VerifyRequest activity =
                new VerifyRequest();

            activity.ConditionsTable =
                conditionsTable;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseConditions_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            List<Definition> conditions =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "conditions");

            Assert.AreEqual(
                2,
                conditions.Count);

            Assert.AreEqual(
                firstCondition,
                conditions[0].Left);

            Assert.AreEqual(
                "First denial message",
                conditions[0].Right);

            Assert.IsFalse(
                conditions[0].Check);

            Assert.AreEqual(
                secondCondition,
                conditions[1].Left);

            Assert.AreEqual(
                "Second denial message",
                conditions[1].Right);

            Assert.IsTrue(
                conditions[1].Check);

            Dictionary<string, object> lookupCache =
                activity.ActivityExpressionEvaluator.LookupCache;

            Assert.IsTrue(
                lookupCache.ContainsKey(firstCondition));

            Assert.IsNull(
                lookupCache[firstCondition]);

            Assert.IsTrue(
                lookupCache.ContainsKey(secondCondition));

            Assert.IsNull(
                lookupCache[secondCondition]);
        }

        [TestMethod]
        public void Activities_VerifyRequest_EnforceConditions_FirstFalseCondition_ShouldSetDenialMessage()
        {
            // Arrange
            const string firstCondition =
                "[//WorkflowData/FirstCondition]";

            const string secondCondition =
                "[//WorkflowData/SecondCondition]";

            const string thirdCondition =
                "[//WorkflowData/ThirdCondition]";

            const string expectedDenialMessage =
                "Second condition failed";

            VerifyRequest activity =
                new VerifyRequest();

            List<Definition> conditions =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "conditions");

            conditions.Add(
                new Definition(
                    firstCondition,
                    "First condition failed",
                    false));

            conditions.Add(
                new Definition(
                    secondCondition,
                    expectedDenialMessage,
                    false));

            conditions.Add(
                new Definition(
                    thirdCondition,
                    "Third condition failed",
                    false));

            activity.ActivityExpressionEvaluator.LookupCache[firstCondition] =
                true;

            activity.ActivityExpressionEvaluator.LookupCache[secondCondition] =
                false;

            activity.ActivityExpressionEvaluator.LookupCache[thirdCondition] =
                false;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EnforceConditions_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                expectedDenialMessage,
                activity.DenialMessage);
        }

        [TestMethod]
        public void Activities_VerifyRequest_EnforceConditions_AllConditionsTrue_ShouldNotSetDenialMessage()
        {
            // Arrange
            const string firstCondition =
                "[//WorkflowData/FirstCondition]";

            const string secondCondition =
                "[//WorkflowData/SecondCondition]";

            VerifyRequest activity =
                new VerifyRequest();

            List<Definition> conditions =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "conditions");

            conditions.Add(
                new Definition(
                    firstCondition,
                    "First denial message",
                    false));

            conditions.Add(
                new Definition(
                    secondCondition,
                    "Second denial message",
                    false));

            activity.ActivityExpressionEvaluator.LookupCache[firstCondition] =
                true;

            activity.ActivityExpressionEvaluator.LookupCache[secondCondition] =
                true;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EnforceConditions_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNull(
                activity.DenialMessage);
        }

        [TestMethod]
        public void Activities_VerifyRequest_EnforceConditions_NonBooleanResult_ShouldNotSetDenialMessage()
        {
            // Arrange
            const string condition =
                "[//WorkflowData/Condition]";

            VerifyRequest activity =
                new VerifyRequest();

            List<Definition> conditions =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "conditions");

            conditions.Add(
                new Definition(
                    condition,
                    "Condition failed",
                    false));

            activity.ActivityExpressionEvaluator.LookupCache[condition] =
                "NotABoolean";

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "EnforceConditions_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNull(
                activity.DenialMessage);
        }
        
        [TestMethod]
        public void Activities_VerifyRequest_CheckResource_ShouldSetDenialMessageWhenConflictExists()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "No conflict",
                    new List<Guid>(),
                    null
                },
                new object[]
                {
                    "Conflict found",
                    new List<Guid>
                    {
                        Guid.NewGuid()
                    },
                    "Resource conflict found"
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                List<Guid> foundIds =
                    (List<Guid>)testCase[1];

                string expected =
                    (string)testCase[2];

                VerifyRequest activity =
                    new VerifyRequest();

                activity.ConflictDenialMessage =
                    "Resource conflict found";

                object findConflict =
                    ActivityTestHelper.GetField<object>(
                        activity,
                        "FindConflict");

                Assert.IsNotNull(
                    findConflict);

                Type findConflictType =
                    findConflict.GetType();

                PropertyInfo foundIdsProperty =
                    findConflictType.GetProperty(
                        "FoundIds",
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

                Assert.IsNotNull(
                    foundIdsProperty);

                foundIdsProperty.SetValue(
                    findConflict,
                    foundIds,
                    null);

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckResource_ExecuteCode",
                    null,
                    EventArgs.Empty);

                // Assert
                Assert.AreEqual(
                    expected,
                    activity.DenialMessage,
                    "Failed test case: " + description);
            }
        }
        
        [TestMethod]
        public void Activities_VerifyRequest_CheckRequest_ShouldHandleConflictResult()
        {
            Guid conflictingRequest =
                Guid.NewGuid();

            object[][] testCases =
            {
                new object[]
                {
                    "No conflict",
                    false,
                    null,
                    Guid.Empty
                },
                new object[]
                {
                    "Conflict found",
                    true,
                    "Request conflict found",
                    conflictingRequest
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                bool conflictFound =
                    (bool)testCase[1];

                string expectedDenialMessage =
                    (string)testCase[2];

                Guid expectedConflictingRequest =
                    (Guid)testCase[3];

                VerifyRequest activity =
                    new VerifyRequest();

                activity.RequestConflictDenialMessage =
                    "Request conflict found";

                object findConflictRequest =
                    ActivityTestHelper.GetField<object>(
                        activity,
                        "FindConflictRequest");

                Assert.IsNotNull(
                    findConflictRequest);

                Type findConflictRequestType =
                    findConflictRequest.GetType();

                PropertyInfo conflictFoundProperty =
                    findConflictRequestType.GetProperty(
                        "ConflictFound",
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

                PropertyInfo conflictingRequestProperty =
                    findConflictRequestType.GetProperty(
                        "ConflictingRequest",
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

                Assert.IsNotNull(
                    conflictFoundProperty);

                Assert.IsNotNull(
                    conflictingRequestProperty);

                conflictFoundProperty.SetValue(
                    findConflictRequest,
                    conflictFound,
                    null);

                conflictingRequestProperty.SetValue(
                    findConflictRequest,
                    conflictingRequest,
                    null);

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckRequest_ExecuteCode",
                    null,
                    EventArgs.Empty);

                // Assert
                Assert.AreEqual(
                    expectedDenialMessage,
                    activity.DenialMessage,
                    "Failed denial message test case: " + description);

                Assert.AreEqual(
                    expectedConflictingRequest,
                    activity.ConflictingRequest,
                    "Failed conflicting request test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_VerifyRequest_CheckConflict_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Everything configured",
                    true,
                    "/Person[AccountName='Test']",
                    "Conflict found",
                    true
                },
                new object[]
                {
                    "Check disabled",
                    false,
                    "/Person[AccountName='Test']",
                    "Conflict found",
                    false
                },
                new object[]
                {
                    "Filter missing",
                    true,
                    null,
                    "Conflict found",
                    false
                },
                new object[]
                {
                    "Denial message missing",
                    true,
                    "/Person[AccountName='Test']",
                    null,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                VerifyRequest activity =
                    new VerifyRequest();

                activity.CheckForConflict =
                    (bool)testCase[1];

                activity.ConflictFilter =
                    (string)testCase[2];

                activity.ConflictDenialMessage =
                    (string)testCase[3];

                bool expected =
                    (bool)testCase[4];

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckConflict_Condition",
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
        public void Activities_VerifyRequest_DenialConditions_ShouldReturnExpectedResults()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "No denial pending",
                    null,
                    true,
                    false
                },
                new object[]
                {
                    "Empty denial message",
                    string.Empty,
                    true,
                    false
                },
                new object[]
                {
                    "Denial pending",
                    "Request denied",
                    false,
                    true
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string denialMessage =
                    (string)testCase[1];

                bool expectedUnique =
                    (bool)testCase[2];

                bool expectedDenialPending =
                    (bool)testCase[3];

                VerifyRequest activity =
                    new VerifyRequest();

                activity.DenialMessage =
                    denialMessage;

                ConditionalEventArgs uniqueArguments =
                    new ConditionalEventArgs();

                ConditionalEventArgs denialPendingArguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "Unique_Condition",
                    null,
                    uniqueArguments);

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "DenialPending_Condition",
                    null,
                    denialPendingArguments);

                // Assert
                Assert.AreEqual(
                    expectedUnique,
                    uniqueArguments.Result,
                    "Unique condition failed: " + description);

                Assert.AreEqual(
                    expectedDenialPending,
                    denialPendingArguments.Result,
                    "DenialPending condition failed: " + description);
            }
        }

        [TestMethod]
        public void Activities_VerifyRequest_ActivityExecutionConditionSatisfied_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "No condition",
                    null,
                    null,
                    true
                },
                new object[]
                {
                    "Condition true",
                    "[//WorkflowData/Condition]",
                    true,
                    true
                },
                new object[]
                {
                    "Condition false",
                    "[//WorkflowData/Condition]",
                    false,
                    false
                },
                new object[]
                {
                    "Condition not Boolean",
                    "[//WorkflowData/Condition]",
                    "NotABoolean",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string condition =
                    (string)testCase[1];

                object resolvedValue =
                    testCase[2];

                bool expected =
                    (bool)testCase[3];

                VerifyRequest activity =
                    new VerifyRequest();

                activity.ActivityExecutionCondition =
                    condition;

                if (!string.IsNullOrEmpty(condition))
                {
                    activity.ActivityExpressionEvaluator.LookupCache[condition] =
                        resolvedValue;
                }

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ActivityExecutionConditionSatisfied_Condition",
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
        public void Activities_VerifyRequest_DenyRequest_ShouldThrowExpectedDenialMessage()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Resolved denial message",
                    "Specific denial message",
                    true
                },
                new object[]
                {
                    "Null resolved message",
                    null,
                    false
                },
                new object[]
                {
                    "Empty resolved message",
                    string.Empty,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string resolvedMessage =
                    (string)testCase[1];

                bool expectSpecificMessage =
                    (bool)testCase[2];

                VerifyRequest activity =
                    new VerifyRequest();

                object resolveMessage =
                    ActivityTestHelper.GetField<object>(
                        activity,
                        "ResolveMessage");

                Assert.IsNotNull(
                    resolveMessage);

                Type resolveMessageType =
                    resolveMessage.GetType();

                PropertyInfo resolvedProperty =
                    resolveMessageType.GetProperty(
                        "Resolved",
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

                Assert.IsNotNull(
                    resolvedProperty);

                resolvedProperty.SetValue(
                    resolveMessage,
                    resolvedMessage,
                    null);

                try
                {
                    // Act
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "DenyRequest_ExecuteCode",
                        null,
                        EventArgs.Empty);

                    Assert.Fail(
                        "Expected WorkflowActivityLibraryException was not thrown. " +
                        "Test case: " +
                        description);
                }
                catch (TargetInvocationException ex)
                {
                    // Assert
                    Assert.IsNotNull(
                        ex.InnerException,
                        "Failed test case: " + description);

                    Assert.IsInstanceOfType(
                        ex.InnerException,
                        typeof(WorkflowActivityLibraryException),
                        "Failed test case: " + description);

                    Assert.IsFalse(
                        string.IsNullOrEmpty(
                            ex.InnerException.Message),
                        "Failed test case: " + description);

                    if (expectSpecificMessage)
                    {
                        Assert.AreEqual(
                            resolvedMessage,
                            ex.InnerException.Message,
                            "Failed test case: " + description);
                    }
                }
            }
        }

        [TestMethod]
        public void Activities_VerifyRequest_Prepare_ExecutionCondition_ShouldParseExpression()
        {
            // Arrange
            const string condition =
                "[//WorkflowData/ExecuteCondition]";

            VerifyRequest activity =
                new VerifyRequest();

            activity.Advanced =
                true;

            activity.ActivityExecutionCondition =
                condition;

            Dictionary<string, object> lookupCache =
                activity.ActivityExpressionEvaluator.LookupCache;

            Assert.IsFalse(
                lookupCache.ContainsKey(condition));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                condition,
                activity.ActivityExecutionCondition);

            Assert.IsTrue(
                lookupCache.ContainsKey(condition));

            Assert.IsNull(
                lookupCache[condition]);
        }

        [TestMethod]
        public void Activities_VerifyRequest_CheckResource_NoConflict_ShouldPreserveExistingDenialMessage()
        {
            // Arrange
            const string existingDenialMessage =
                "Existing denial";

            VerifyRequest activity =
                new VerifyRequest();

            activity.DenialMessage =
                existingDenialMessage;

            activity.ConflictDenialMessage =
                "Resource conflict";

            object findConflict =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindConflict");

            Assert.IsNotNull(
                findConflict);

            Type findConflictType =
                findConflict.GetType();

            PropertyInfo foundIdsProperty =
                findConflictType.GetProperty(
                    "FoundIds",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            Assert.IsNotNull(
                foundIdsProperty);

            foundIdsProperty.SetValue(
                findConflict,
                new List<Guid>(),
                null);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckResource_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                existingDenialMessage,
                activity.DenialMessage);
        }

    }
}
