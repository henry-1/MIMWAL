using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using System.Collections.Generic;
using System.Reflection;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
using System.Workflow.Activities;
using Microsoft.ResourceManagement.Workflow.Activities;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
using System.Collections;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class SendEmailNotificationTests
    {
        [TestMethod]
        public void Activities_SendEmailNotification_GetEmailTemplateGuid_GuidString_ShouldReturnGuid()
        {
            // Arrange
            Guid expected =
                new Guid(
                    "c302a07d-5400-43af-94dd-83aa65e5aed4");

            SendEmailNotification activity =
                new SendEmailNotification();

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    expected.ToString());

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Guid));

            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_GetEmailTemplateGuid_ExpressionResolvesToGuid_ShouldReturnGuid()
        {
            // Arrange
            const string templateLookup =
                "[//WorkflowData/EmailTemplate]";

            Guid expected =
                Guid.NewGuid();

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[templateLookup] =
                expected;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    templateLookup);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Guid));

            Assert.AreEqual(
                expected,
                result);
        }


        [TestMethod]
        public void Activities_SendEmailNotification_GetEmailTemplateGuid_ExpressionResolvesToByteArray_ShouldReturnGuid()
        {
            // Arrange
            const string templateLookup =
                "[//WorkflowData/EmailTemplate]";

            Guid expected =
                Guid.NewGuid();

            byte[] guidBytes =
                expected.ToByteArray();

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[templateLookup] =
                guidBytes;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    templateLookup);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Guid));

            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_GetEmailTemplateGuid_ExpressionResolvesToGuidString_ShouldReturnGuid()
        {
            // Arrange
            const string templateLookup =
                "[//WorkflowData/EmailTemplate]";

            Guid expected =
                Guid.NewGuid();

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[templateLookup] =
                expected.ToString();

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    templateLookup);

            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(Guid));

            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_GetEmailTemplateGuid_ExpressionResolvesToMultipleGuids_ShouldThrowWorkflowActivityLibraryException()
        {
            // Arrange
            const string templateLookup =
                "[//WorkflowData/EmailTemplate]";

            List<Guid> templateGuids =
                new List<Guid>
                {
            Guid.NewGuid(),
            Guid.NewGuid()
                };

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[templateLookup] =
                templateGuids;

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    templateLookup);

                Assert.Fail(
                    "Expected WorkflowActivityLibraryException was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsNotNull(
                    ex.InnerException);

                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(WorkflowActivityLibraryException));
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_GetEmailTemplateGuid_InvalidValue_ShouldThrowWorkflowActivityLibraryException()
        {
            string[] testCases =
            {
                null,
                string.Empty,
                "Not-A-Guid"
            };

            foreach (string emailTemplate in testCases)
            {
                // Arrange
                SendEmailNotification activity =
                    new SendEmailNotification();

                try
                {
                    // Act
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "GetEmailTemplateGuid",
                        emailTemplate);

                    Assert.Fail(
                        "Expected WorkflowActivityLibraryException was not thrown.");
                }
                catch (TargetInvocationException ex)
                {
                    // Assert
                    Assert.IsNotNull(
                        ex.InnerException);

                    Assert.IsInstanceOfType(
                        ex.InnerException,
                        typeof(WorkflowActivityLibraryException));
                }
            }
        }

        
        [TestMethod]
        public void ExpressionEvaluator_IsEmailAddress_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single address",
                    "user@contoso.com",
                    true
                },
                new object[]
                {
                    "Address without dotted domain",
                    "user@contoso",
                    true
                },
                new object[]
                {
                    "Semicolon separated addresses",
                    "user1@contoso.com;user2@contoso.com",
                    true
                },
                new object[]
                {
                    "Comma separated addresses",
                    "user1@contoso.com,user2@contoso.com",
                    true
                },
                new object[]
                {
                    "Semicolon separated with spaces",
                    "user1@contoso.com; user2@contoso.com",
                    true
                },
                new object[]
                {
                    "Comma separated with spaces",
                    "user1@contoso.com, user2@contoso.com",
                    true
                },
                new object[]
                {
                    "Null",
                    null,
                    false
                },
                new object[]
                {
                    "Empty",
                    string.Empty,
                    false
                },
                new object[]
                {
                    "Missing address",
                    "@",
                    false
                },
                new object[]
                {
                    "Missing domain",
                    "user@",
                    false
                },
                new object[]
                {
                    "Missing user",
                    "@contoso.com",
                    false
                },
                new object[]
                {
                    "No at sign",
                    "user.contoso.com",
                    false
                },
                new object[]
                {
                    "Invalid second semicolon address",
                    "user1@contoso.com;invalid",
                    false
                },
                new object[]
                {
                    "Invalid second comma address",
                    "user1@contoso.com,invalid",
                    false
                },
                new object[]
                {
                    "Trailing semicolon",
                    "user@contoso.com;",
                    false
                },
                new object[]
                {
                    "Leading semicolon",
                    ";user@contoso.com",
                    false
                },
                new object[]
                {
                    "Trailing comma",
                    "user@contoso.com,",
                    false
                },
                new object[]
                {
                    "Empty address between separators",
                    "user1@contoso.com;;user2@contoso.com",
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string input =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                // Act
                bool result =
                    ExpressionEvaluator.IsEmailAddress(
                        input);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_FormatRecipient_LiteralRecipient_ShouldReturnUnchanged()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Single address",
                    "user@contoso.com"
                },
                new object[]
                {
                    "Semicolon separated addresses",
                    "user1@contoso.com;user2@contoso.com"
                },
                new object[]
                {
                    "Comma separated addresses",
                    "user1@contoso.com,user2@contoso.com"
                },
                new object[]
                {
                    "Empty string",
                    string.Empty
                },
                new object[]
                {
                    "Null",
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string recipient =
                    (string)testCase[1];

                SendEmailNotification activity =
                    new SendEmailNotification();

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "FormatRecipient",
                        recipient);

                // Assert
                Assert.AreEqual(
                    recipient,
                    result,
                    "Failed test case: " + description);
            }
        }


        [TestMethod]
        public void Activities_SendEmailNotification_FormatRecipient_ExpressionResolvesToGuid_ShouldReturnGuidString()
        {
            // Arrange
            const string recipientLookup =
                "[//WorkflowData/Recipient]";

            Guid expected =
                Guid.NewGuid();

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[recipientLookup] =
                expected;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    recipientLookup);

            // Assert
            Assert.AreEqual(
                expected.ToString(),
                result);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_FormatRecipient_ExpressionResolvesToByteArray_ShouldReturnGuidString()
        {
            // Arrange
            const string recipientLookup =
                "[//WorkflowData/Recipient]";

            Guid expected =
                Guid.NewGuid();

            byte[] recipientBytes =
                expected.ToByteArray();

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[recipientLookup] =
                recipientBytes;

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    recipientLookup);

            // Assert
            Assert.AreEqual(
                expected.ToString(),
                result);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_FormatRecipient_ExpressionResolvesToGuidList_ShouldReturnSemicolonSeparatedGuidString()
        {
            // Arrange
            const string recipientLookup =
                "[//WorkflowData/Recipients]";

            Guid firstRecipient =
                Guid.NewGuid();

            Guid secondRecipient =
                Guid.NewGuid();

            List<Guid> recipients =
                new List<Guid>
                {
            firstRecipient,
            secondRecipient
                };

            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[recipientLookup] =
                recipients;

            string expected =
                firstRecipient.ToString() +
                ";" +
                secondRecipient.ToString();

            // Act
            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    recipientLookup);

            // Assert
            Assert.AreEqual(
                expected,
                result);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_FormatRecipient_ExpressionResolvesToStringOrNull_ShouldReturnExpectedResult()
        {
            const string recipientLookup =
                "[//WorkflowData/Recipient]";

            object[][] testCases =
            {
                new object[]
                {
                    "Single email address",
                    "user@contoso.com",
                    "user@contoso.com"
                },
                new object[]
                {
                    "Semicolon separated addresses",
                    "user1@contoso.com;user2@contoso.com",
                    "user1@contoso.com;user2@contoso.com"
                },
                new object[]
                {
                    "Null",
                    null,
                    null
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                object resolvedRecipient =
                    testCase[1];

                string expected =
                    (string)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                Dictionary<string, object> lookupCache =
                    ActivityTestHelper.GetField<Dictionary<string, object>>(
                        activity.ActivityExpressionEvaluator,
                        "lookupCache");

                lookupCache[recipientLookup] =
                    resolvedRecipient;

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "FormatRecipient",
                        recipientLookup);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_TestSubmitRequests_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "No condition",
                    null,
                    false,
                    true
                },
                new object[]
                {
                    "True condition",
                    "true",
                    false,
                    true
                },
                new object[]
                {
                    "False condition with false input",
                    "false",
                    false,
                    false
                },
                new object[]
                {
                    "False condition with true input",
                    "false",
                    true,
                    true
                },
                new object[]
                {
                    "Non-Boolean condition",
                    "\"Test\"",
                    false,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.ActivityExecutionCondition =
                    (string)testCase[1];

                bool submitRequests =
                    (bool)testCase[2];

                bool expected =
                    (bool)testCase[3];

                // Act
                object result =
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "TestSubmitRequests",
                        submitRequests);

                // Assert
                Assert.AreEqual(
                    expected,
                    result,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_PrepareIteration_NoIteration_ShouldCreateSingleNullIteration()
        {
            // Arrange
            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExecutionCondition =
                null;

            activity.Iteration =
                null;

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(
                forEachIteration);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNotNull(
                forEachIteration.InitialChildData);

            Assert.AreEqual(
                1,
                forEachIteration.InitialChildData.Count);

            Assert.IsNull(
                forEachIteration.InitialChildData[0]);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_PrepareIteration_FalseCondition_ShouldNotCreateIterations()
        {
            // Arrange
            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExecutionCondition =
                "false";

            activity.Iteration =
                null;

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            Assert.IsNotNull(
                forEachIteration);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNull(
                forEachIteration.InitialChildData);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_PrepareIteration_SingleValue_ShouldCreateSingleIteration()
        {
            // Arrange
            const string iterationLookup =
                "[//WorkflowData/IterationValue]";

            const string expected =
                "TestValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExecutionCondition =
                null;

            activity.Iteration =
                iterationLookup;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[iterationLookup] =
                expected;

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNotNull(
                forEachIteration.InitialChildData);

            Assert.AreEqual(
                1,
                forEachIteration.InitialChildData.Count);

            Assert.AreEqual(
                expected,
                forEachIteration.InitialChildData[0]);
        }


        [TestMethod]
        public void Activities_SendEmailNotification_PrepareIteration_List_ShouldCreateIterationForEachValue()
        {
            // Arrange
            const string iterationLookup =
                "[//WorkflowData/IterationValues]";

            List<string> iterationValues =
                new List<string>
                {
                    "One",
                    "Two",
                    "Three"
                };

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExecutionCondition =
                null;

            activity.Iteration =
                iterationLookup;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[iterationLookup] =
                iterationValues;

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNotNull(
                forEachIteration.InitialChildData);

            Assert.AreEqual(
                3,
                forEachIteration.InitialChildData.Count);

            Assert.AreEqual(
                "One",
                forEachIteration.InitialChildData[0]);

            Assert.AreEqual(
                "Two",
                forEachIteration.InitialChildData[1]);

            Assert.AreEqual(
                "Three",
                forEachIteration.InitialChildData[2]);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_PrepareIteration_NullValue_ShouldCreateNoIterations()
        {
            // Arrange
            const string iterationLookup =
                "[//WorkflowData/IterationValues]";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExecutionCondition =
                null;

            activity.Iteration =
                iterationLookup;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[iterationLookup] =
                null;

            ReplicatorActivity forEachIteration =
                ActivityTestHelper.GetField<ReplicatorActivity>(
                    activity,
                    "ForEachIteration");

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNotNull(
                forEachIteration.InitialChildData);

            Assert.AreEqual(
                0,
                forEachIteration.InitialChildData.Count);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_PrepareIteration_ShouldLoadApplicableLookupsIntoValueExpressions()
        {
            // Arrange
            const string iterationLookup =
                "[//WorkflowData/IterationValues]";

            const string valueLookup =
                "[//Value/DisplayName]";

            const string workflowDataLookup =
                "[//WorkflowData/TestValue]";

            const string queryLookup =
                "[//Queries/TestQuery]";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Iteration =
                iterationLookup;

            Dictionary<string, object> lookupCache =
                ActivityTestHelper.GetField<Dictionary<string, object>>(
                    activity.ActivityExpressionEvaluator,
                    "lookupCache");

            lookupCache[iterationLookup] =
                new List<string>
                {
                    "One",
                    "Two"
                };

            lookupCache[valueLookup] =
                null;

            lookupCache[workflowDataLookup] =
                null;

            lookupCache[queryLookup] =
                null;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareIteration_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsTrue(
                activity.ValueExpressions.ContainsKey(valueLookup));

            Assert.IsNull(
                activity.ValueExpressions[valueLookup]);

            Assert.IsTrue(
                activity.ValueExpressions.ContainsKey(workflowDataLookup));

            Assert.IsNull(
                activity.ValueExpressions[workflowDataLookup]);

            Assert.IsTrue(
                activity.ValueExpressions.ContainsKey(queryLookup));

            Assert.IsNull(
                activity.ValueExpressions[queryLookup]);
        }


        [TestMethod]
        public void Activities_SendEmailNotification_ForEachIteration_ChildInitialized_ShouldSetValueIncrementIterationAndSuppressException()
        {
            // Arrange
            const string iterationValue =
                "CurrentValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.SuppressException =
                true;

            SequenceActivity sequence =
                new SequenceActivity();

            EmailNotificationActivity emailNotification =
                new EmailNotificationActivity();

            sequence.Activities.Add(
                emailNotification);

            ReplicatorChildEventArgs arguments =
                new ReplicatorChildEventArgs(
                    iterationValue,
                    sequence);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ForEachIteration_ChildInitialized",
                null,
                arguments);

            // Assert
            Assert.AreEqual(
                iterationValue,
                activity.Value);

            int iterations =
                ActivityTestHelper.GetValueField<int>(
                    activity,
                    "iterations");

            Assert.AreEqual(
                1,
                iterations);

            Assert.IsTrue(
                emailNotification.SuppressException);
        }


        [TestMethod]
        public void Activities_SendEmailNotification_ForEachIteration_ChildCompleted_ShouldSetBreakIteration()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Continue iteration",
                    false,
                    false
                },
                new object[]
                {
                    "Break iteration",
                    true,
                    true
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                bool variableValue =
                    (bool)testCase[1];

                bool expected =
                    (bool)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.ActivityExpressionEvaluator.VariableCache[
                    ExpressionEvaluator.ReservedVariableBreakIteration] =
                    variableValue;

                ReplicatorActivity forEachIteration =
                    ActivityTestHelper.GetField<ReplicatorActivity>(
                        activity,
                        "ForEachIteration");

                forEachIteration.InitialChildData =
                    new object[]
                    {
                "TestValue"
                    };

                SequenceActivity sequence =
                    new SequenceActivity();

                ReplicatorChildEventArgs arguments =
                    new ReplicatorChildEventArgs(
                        "TestValue",
                        sequence);

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ForEachIteration_ChildCompleted",
                    null,
                    arguments);

                // Assert
                bool breakIteration =
                    ActivityTestHelper.GetValueField<bool>(
                        activity,
                        "breakIteration");

                Assert.AreEqual(
                    expected,
                    breakIteration,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_LoadValueExpressionsIntoCache_ShouldUpdateLookupCache()
        {
            // Arrange
            const string firstLookup =
                "[//Value/DisplayName]";

            const string secondLookup =
                "[//WorkflowData/TestValue]";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ValueExpressions[firstLookup] =
                "NewValue";

            activity.ValueExpressions[secondLookup] =
                42L;

            Dictionary<string, object> lookupCache =
                activity.ActivityExpressionEvaluator.LookupCache;

            lookupCache[firstLookup] =
                "OldValue";

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "LoadValueExpressionsIntoCache");

            // Assert
            Assert.AreEqual(
                "NewValue",
                lookupCache[firstLookup]);

            Assert.AreEqual(
                42L,
                lookupCache[secondLookup]);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ClearVariableCache_ShouldClearAllValues()
        {
            // Arrange
            SendEmailNotification activity =
                new SendEmailNotification();

            Dictionary<string, object> variableCache =
                activity.ActivityExpressionEvaluator.VariableCache;

            variableCache["FirstVariable"] =
                "Value";

            variableCache["SecondVariable"] =
                42L;

            variableCache["ThirdVariable"] =
                true;

            int expectedCount =
                variableCache.Count;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ClearVariableCache");

            // Assert
            Assert.AreEqual(
                expectedCount,
                variableCache.Count);

            foreach (KeyValuePair<string, object> variable in variableCache)
            {
                Assert.IsNull(
                    variable.Value,
                    "Variable was not cleared: " + variable.Key);
            }
        }


        [TestMethod]
        public void Activities_SendEmailNotification_ApplyUpdate_LookupTarget_ShouldAddLookupUpdate()
        {
            // Arrange
            const string target =
                "[//WorkflowData/TestValue]";

            const string value =
                "TestValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ApplyUpdate",
                target,
                value,
                false,
                UpdateMode.Modify);

            // Assert
            Assert.AreEqual(
                1,
                activity.LookupUpdates.Count);

            UpdateLookupDefinition update =
                activity.LookupUpdates[0];

            Assert.AreEqual(
                target,
                update.TargetLookup);

            Assert.AreEqual(
                value,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ApplyUpdate_VariableTarget_ShouldPublishVariable()
        {
            // Arrange
            const string target =
                "$TestVariable";

            const string value =
                "TestValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExpressionEvaluator.VariableCache[target] =
                null;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ApplyUpdate",
                target,
                value,
                true,
                UpdateMode.Modify);

            // Assert
            Assert.AreEqual(
                value,
                activity.ActivityExpressionEvaluator.VariableCache[target]);

            Assert.AreEqual(
                0,
                activity.LookupUpdates.Count);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ProcessUpdateDefinition_ResolvedValue_ShouldCreateModifyUpdate()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "[//WorkflowData/TargetValue]";

            const string expectedValue =
                "TestValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                expectedValue;

            Definition definition =
                new Definition(
                    source,
                    target,
                    false);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ProcessUpdateDefinition",
                definition);

            // Assert
            Assert.AreEqual(
                1,
                activity.LookupUpdates.Count);

            UpdateLookupDefinition update =
                activity.LookupUpdates[0];

            Assert.AreEqual(
                target,
                update.TargetLookup);

            Assert.AreEqual(
                expectedValue,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ProcessUpdateDefinition_CachedSource_ShouldCreateModifyUpdate()
        {
            // Arrange
            const string source =
                "ResolvedDynamicValue";

            const string target =
                "[//WorkflowData/TargetValue]";

            const string expectedValue =
                "TestValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                expectedValue;

            Definition definition =
                new Definition(
                    source,
                    target,
                    false);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ProcessUpdateDefinition",
                definition);

            // Assert
            Assert.AreEqual(
                1,
                activity.LookupUpdates.Count);

            UpdateLookupDefinition update =
                activity.LookupUpdates[0];

            Assert.AreEqual(
                target,
                update.TargetLookup);

            Assert.AreEqual(
                expectedValue,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ProcessUpdateDefinition_NullValue_ShouldRespectCheckSetting()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "Null not allowed",
                    false,
                    0
                },
                new object[]
                {
                    "Null allowed",
                    true,
                    1
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                bool check =
                    (bool)testCase[1];

                int expectedUpdateCount =
                    (int)testCase[2];

                const string source =
                    "[//WorkflowData/SourceValue]";

                const string target =
                    "[//WorkflowData/TargetValue]";

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.ActivityExpressionEvaluator.LookupCache[source] =
                    null;

                Definition definition =
                    new Definition(
                        source,
                        target,
                        check);

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ProcessUpdateDefinition",
                    definition);

                // Assert
                Assert.AreEqual(
                    expectedUpdateCount,
                    activity.LookupUpdates.Count,
                    "Failed test case: " + description);

                if (check)
                {
                    UpdateLookupDefinition update =
                        activity.LookupUpdates[0];

                    Assert.AreEqual(
                        target,
                        update.TargetLookup);

                    Assert.IsNull(
                        update.Value);

                    Assert.AreEqual(
                        UpdateMode.Modify,
                        update.Mode);
                }
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ProcessUpdateDefinition_InsertedValues_ShouldCreateInsertUpdates()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "[//WorkflowData/TargetValue]";

            InsertedValuesCollection insertedValues =
                new InsertedValuesCollection
                {
                    "FirstValue",
                    "SecondValue",
                    "ThirdValue"
                };

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                insertedValues;

            Definition definition =
                new Definition(
                    source,
                    target,
                    false);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ProcessUpdateDefinition",
                definition);

            // Assert
            Assert.AreEqual(
                3,
                activity.LookupUpdates.Count);

            Assert.AreEqual(
                target,
                activity.LookupUpdates[0].TargetLookup);

            Assert.AreEqual(
                "FirstValue",
                activity.LookupUpdates[0].Value);

            Assert.AreEqual(
                UpdateMode.Insert,
                activity.LookupUpdates[0].Mode);

            Assert.AreEqual(
                target,
                activity.LookupUpdates[1].TargetLookup);

            Assert.AreEqual(
                "SecondValue",
                activity.LookupUpdates[1].Value);

            Assert.AreEqual(
                UpdateMode.Insert,
                activity.LookupUpdates[1].Mode);

            Assert.AreEqual(
                target,
                activity.LookupUpdates[2].TargetLookup);

            Assert.AreEqual(
                "ThirdValue",
                activity.LookupUpdates[2].Value);

            Assert.AreEqual(
                UpdateMode.Insert,
                activity.LookupUpdates[2].Mode);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ProcessUpdateDefinition_RemovedValues_ShouldCreateRemoveUpdates()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "[//WorkflowData/TargetValue]";

            RemovedValuesCollection removedValues =
                new RemovedValuesCollection
                {
            "FirstValue",
            "SecondValue",
            "ThirdValue"
                };

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                removedValues;

            Definition definition =
                new Definition(
                    source,
                    target,
                    false);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ProcessUpdateDefinition",
                definition);

            // Assert
            Assert.AreEqual(
                3,
                activity.LookupUpdates.Count);

            Assert.AreEqual(
                target,
                activity.LookupUpdates[0].TargetLookup);

            Assert.AreEqual(
                "FirstValue",
                activity.LookupUpdates[0].Value);

            Assert.AreEqual(
                UpdateMode.Remove,
                activity.LookupUpdates[0].Mode);

            Assert.AreEqual(
                target,
                activity.LookupUpdates[1].TargetLookup);

            Assert.AreEqual(
                "SecondValue",
                activity.LookupUpdates[1].Value);

            Assert.AreEqual(
                UpdateMode.Remove,
                activity.LookupUpdates[1].Mode);

            Assert.AreEqual(
                target,
                activity.LookupUpdates[2].TargetLookup);

            Assert.AreEqual(
                "ThirdValue",
                activity.LookupUpdates[2].Value);

            Assert.AreEqual(
                UpdateMode.Remove,
                activity.LookupUpdates[2].Mode);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ProcessUpdateDefinition_VariableTarget_ShouldPublishVariable()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "$TargetVariable";

            const string expectedValue =
                "TestValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.ActivityExpressionEvaluator.LookupCache[source] =
                expectedValue;

            activity.ActivityExpressionEvaluator.VariableCache[target] =
                null;

            Definition definition =
                new Definition(
                    source,
                    target,
                    false);

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ProcessUpdateDefinition",
                definition);

            // Assert
            Assert.AreEqual(
                expectedValue,
                activity.ActivityExpressionEvaluator.VariableCache[target]);

            Assert.AreEqual(
                0,
                activity.LookupUpdates.Count);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_PrepareUpdate_ShouldPrepareAndProcessUpdates()
        {
            // Arrange
            const string source =
                "[//WorkflowData/SourceValue]";

            const string target =
                "[//WorkflowData/TargetValue]";

            const string expectedValue =
                "ResolvedValue";

            SendEmailNotification activity =
                new SendEmailNotification();

            // ValueExpressions should be copied to LookupCache.
            activity.ValueExpressions[source] =
                expectedValue;

            // Existing variable values should be cleared.
            activity.ActivityExpressionEvaluator.VariableCache["TestVariable"] =
                "OldVariableValue";

            // Existing update must be discarded.
            List<UpdateLookupDefinition> oldLookupUpdates =
                activity.LookupUpdates;

            oldLookupUpdates.Add(
                new UpdateLookupDefinition(
                    "[//WorkflowData/OldTarget]",
                    "OldValue",
                    UpdateMode.Modify));

            List<Definition> workflowDataVariables =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "workflowDataVariables");

            workflowDataVariables.Add(
                new Definition(
                    source,
                    target,
                    false));

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "PrepareUpdate_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert

            // Value expression was loaded.
            Assert.AreEqual(
                expectedValue,
                activity.ActivityExpressionEvaluator.LookupCache[source]);

            // Variable cache was cleared.
            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache["TestVariable"]);

            // A new update list was created.
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
                expectedValue,
                update.Value);

            Assert.AreEqual(
                UpdateMode.Modify,
                update.Mode);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_CheckEmailTemplateResource_SingleTemplate_ShouldSetEmailTemplateGuid()
        {
            // Arrange
            Guid expected =
                Guid.NewGuid();

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.EmailTemplate =
                "/EmailTemplate[DisplayName='Test']";

            activity.EmailTemplateFoundIds =
                new List<Guid>
                {
            expected
                };

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckEmailTemplateResource_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                expected,
                activity.EmailTemplateGuid);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_CheckEmailTemplateResource_InvalidTemplateCount_ShouldThrowWorkflowActivityLibraryException()
        {
            object[][] testCases =
            {
        new object[]
        {
            "No templates found",
            new List<Guid>()
        },
        new object[]
        {
            "Multiple templates found",
            new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            }
        }
    };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                List<Guid> templateIds =
                    (List<Guid>)testCase[1];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.EmailTemplate =
                    "/EmailTemplate[DisplayName='Test']";

                activity.EmailTemplateFoundIds =
                    templateIds;

                try
                {
                    // Act
                    ActivityTestHelper.InvokePrivateMethod(
                        activity,
                        "CheckEmailTemplateResource_ExecuteCode",
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
                }
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ResolveEmailTemplate_ShouldSetEmailTemplateGuid()
        {
            // Arrange
            Guid expected =
                Guid.NewGuid();

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.EmailTemplate =
                expected.ToString();

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEmailTemplate_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                expected,
                activity.EmailTemplateGuid);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_CheckEmailToRecipientResources_ShouldSetExpectedRecipients()
        {
            Guid firstRecipient =
                Guid.NewGuid();

            Guid secondRecipient =
                Guid.NewGuid();

            object[][] testCases =
            {
        new object[]
        {
            "No recipients",
            new List<Guid>(),
            null
        },
        new object[]
        {
            "Single recipient",
            new List<Guid>
            {
                firstRecipient
            },
            firstRecipient.ToString()
        },
        new object[]
        {
            "Multiple recipients",
            new List<Guid>
            {
                firstRecipient,
                secondRecipient
            },
            firstRecipient.ToString() +
                ";" +
                secondRecipient.ToString()
        }
    };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                List<Guid> recipients =
                    (List<Guid>)testCase[1];

                string expected =
                    (string)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.To =
                    "/Person[AccountName='Test']";

                activity.EmailToRecipientsFoundIds =
                    recipients;

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckEmailToRecipientResources_ExecuteCode",
                    null,
                    EventArgs.Empty);

                // Assert
                Assert.AreEqual(
                    expected,
                    activity.EmailNotificationToRecipients,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ResolveEmailToRecipients_ShouldSetRecipients()
        {
            // Arrange
            const string recipients =
                "user1@contoso.com;user2@contoso.com";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.To =
                recipients;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEmailToRecipients_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                recipients,
                activity.EmailNotificationToRecipients);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_CheckEmailCcRecipientResources_ShouldSetExpectedRecipients()
        {
            Guid firstRecipient =
                Guid.NewGuid();

            Guid secondRecipient =
                Guid.NewGuid();

            object[][] testCases =
            {
        new object[]
        {
            "No recipients",
            new List<Guid>(),
            null
        },
        new object[]
        {
            "Single recipient",
            new List<Guid>
            {
                firstRecipient
            },
            firstRecipient.ToString()
        },
        new object[]
        {
            "Multiple recipients",
            new List<Guid>
            {
                firstRecipient,
                secondRecipient
            },
            firstRecipient.ToString() +
                ";" +
                secondRecipient.ToString()
        }
    };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                List<Guid> recipients =
                    (List<Guid>)testCase[1];

                string expected =
                    (string)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.CC =
                    "/Person[AccountName='Test']";

                activity.EmailCcRecipientsFoundIds =
                    recipients;

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckEmailCcRecipientResources_ExecuteCode",
                    null,
                    EventArgs.Empty);

                // Assert
                Assert.AreEqual(
                    expected,
                    activity.EmailNotificationCcRecipients,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ResolveEmailCcRecipients_ShouldSetRecipients()
        {
            // Arrange
            const string recipients =
                "user1@contoso.com;user2@contoso.com";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.CC =
                recipients;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEmailCcRecipients_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                recipients,
                activity.EmailNotificationCcRecipients);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_CheckEmailBccRecipientResources_ShouldSetExpectedRecipients()
        {
            Guid firstRecipient =
                Guid.NewGuid();

            Guid secondRecipient =
                Guid.NewGuid();

            object[][] testCases =
            {
        new object[]
        {
            "No recipients",
            new List<Guid>(),
            null
        },
        new object[]
        {
            "Single recipient",
            new List<Guid>
            {
                firstRecipient
            },
            firstRecipient.ToString()
        },
        new object[]
        {
            "Multiple recipients",
            new List<Guid>
            {
                firstRecipient,
                secondRecipient
            },
            firstRecipient.ToString() +
                ";" +
                secondRecipient.ToString()
        }
    };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                List<Guid> recipients =
                    (List<Guid>)testCase[1];

                string expected =
                    (string)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.Bcc =
                    "/Person[AccountName='Test']";

                activity.EmailBccRecipientsFoundIds =
                    recipients;

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckEmailBccRecipientResources_ExecuteCode",
                    null,
                    EventArgs.Empty);

                // Assert
                Assert.AreEqual(
                    expected,
                    activity.EmailNotificationBccRecipients,
                    "Failed test case: " + description);
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_ResolveEmailBccRecipients_ShouldSetRecipients()
        {
            // Arrange
            const string recipients =
                "user1@contoso.com;user2@contoso.com";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Bcc =
                recipients;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEmailBccRecipients_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.AreEqual(
                recipients,
                activity.EmailNotificationBccRecipients);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_QueriesHaveNoValueExpressions_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
        new object[]
        {
            "Query resources disabled",
            false,
            new List<Definition>(),
            true
        },
        new object[]
        {
            "No queries",
            true,
            new List<Definition>(),
            true
        },
        new object[]
        {
            "Query without Value expression",
            true,
            new List<Definition>
            {
                new Definition(
                    "TestQuery",
                    "/Person[AccountName='Test']",
                    false)
            },
            true
        },
        new object[]
        {
            "Query containing Value expression",
            true,
            new List<Definition>
            {
                new Definition(
                    "TestQuery",
                    "/Person[AccountName=[//Value/AccountName]]",
                    false)
            },
            false
        }
    };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                bool queryResources =
                    (bool)testCase[1];

                List<Definition> queries =
                    (List<Definition>)testCase[2];

                bool expected =
                    (bool)testCase[3];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.QueryResources =
                    queryResources;

                activity.Queries =
                    queries;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "QueriesHaveNoValueExpressions_Condition",
                    null,
                    arguments);

                // Assert
                Assert.AreEqual(
                    expected,
                    arguments.Result,
                    "Failed test case: " + description);
            }
        }

        public static void SetValueField<T>(
            object instance,
            string fieldName,
            T value)
            where T : struct
        {
            FieldInfo field =
                instance.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.IsNotNull(
                field,
                "Field '{0}' was not found.",
                fieldName);

            field.SetValue(
                instance,
                value);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_EmailTemplateIsXPath_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "XPath",
                    "/EmailTemplate[DisplayName='Test']",
                    true
                },
                new object[]
                {
                    "Guid",
                    Guid.NewGuid().ToString(),
                    false
                },
                new object[]
                {
                    "Expression",
                    "[//WorkflowData/EmailTemplate]",
                    false
                },
                new object[]
                {
                    "Null",
                    null,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string emailTemplate =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.EmailTemplate =
                    emailTemplate;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "EmailTemplateIsXPath_Condition",
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
        public void Activities_SendEmailNotification_EmailToIsXPath_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "XPath",
                    "/Person[AccountName='Test']",
                    true
                },
                new object[]
                {
                    "Single email address",
                    "user@contoso.com",
                    false
                },
                new object[]
                {
                    "Multiple email addresses",
                    "user1@contoso.com;user2@contoso.com",
                    false
                },
                new object[]
                {
                    "Expression",
                    "[//WorkflowData/Recipients]",
                    false
                },
                new object[]
                {
                    "Null",
                    null,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string recipient =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.To =
                    recipient;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "EmailToIsXPath_Condition",
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
        public void Activities_SendEmailNotification_EmailCcIsXPath_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "XPath",
                    "/Person[AccountName='Test']",
                    true
                },
                new object[]
                {
                    "Email address",
                    "user@contoso.com",
                    false
                },
                new object[]
                {
                    "Expression",
                    "[//WorkflowData/Recipients]",
                    false
                },
                new object[]
                {
                    "Null",
                    null,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string recipient =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.CC =
                    recipient;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "EmailCcIsXPath_Condition",
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
        public void Activities_SendEmailNotification_EmailBccIsXPath_ShouldReturnExpectedResult()
        {
            object[][] testCases =
            {
                new object[]
                {
                    "XPath",
                    "/Person[AccountName='Test']",
                    true
                },
                new object[]
                {
                    "Email address",
                    "user@contoso.com",
                    false
                },
                new object[]
                {
                    "Expression",
                    "[//WorkflowData/Recipients]",
                    false
                },
                new object[]
                {
                    "Null",
                    null,
                    false
                }
            };

            foreach (object[] testCase in testCases)
            {
                // Arrange
                string description =
                    (string)testCase[0];

                string recipient =
                    (string)testCase[1];

                bool expected =
                    (bool)testCase[2];

                SendEmailNotification activity =
                    new SendEmailNotification();

                activity.Bcc =
                    recipient;

                ConditionalEventArgs arguments =
                    new ConditionalEventArgs();

                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "EmailBccIsXPath_Condition",
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
        public void Activities_SendEmailNotification_Prepare_MissingToAndSuppressExceptionFalse_ShouldThrowWorkflowActivityLibraryException()
        {
            // Arrange
            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Advanced =
                true;

            activity.To =
                null;

            activity.SuppressException =
                false;

            try
            {
                // Act
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "Prepare_ExecuteCode",
                    null,
                    EventArgs.Empty);

                Assert.Fail(
                    "Expected WorkflowActivityLibraryException was not thrown.");
            }
            catch (TargetInvocationException ex)
            {
                // Assert
                Assert.IsNotNull(
                    ex.InnerException);

                Assert.IsInstanceOfType(
                    ex.InnerException,
                    typeof(WorkflowActivityLibraryException));
            }
        }

        [TestMethod]
        public void Activities_SendEmailNotification_Prepare_MissingToAndSuppressExceptionTrue_ShouldNotThrow()
        {
            // Arrange
            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Advanced =
                true;

            activity.To =
                null;

            activity.SuppressException =
                true;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNull(
                activity.To);

            Assert.IsTrue(
                activity.SuppressException);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_Prepare_QueryResources_ShouldCreateQueryDefinitions()
        {
            // Arrange
            Hashtable queriesTable =
                new Hashtable();

            queriesTable["Count"] =
                2;

            queriesTable["0:0"] =
                "FirstQuery";

            queriesTable["0:1"] =
                "/Person[AccountName='One']";

            queriesTable["0:2"] =
                false;

            queriesTable["1:0"] =
                "SecondQuery";

            queriesTable["1:1"] =
                "/Person[AccountName='Two']";

            queriesTable["1:2"] =
                false;

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Advanced =
                true;

            activity.QueryResources =
                true;

            activity.QueriesTable =
                queriesTable;

            // Required so Prepare_ExecuteCode does not fail
            // recipient validation.
            activity.To =
                "user@contoso.com";

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsNotNull(
                activity.Queries);

            Assert.AreEqual(
                2,
                activity.Queries.Count);

            Assert.AreEqual(
                "FirstQuery",
                activity.Queries[0].Left);

            Assert.AreEqual(
                "/Person[AccountName='One']",
                activity.Queries[0].Right);

            Assert.AreEqual(
                "SecondQuery",
                activity.Queries[1].Left);

            Assert.AreEqual(
                "/Person[AccountName='Two']",
                activity.Queries[1].Right);
        }

        [TestMethod]
        public void Activities_SendEmailNotification_Prepare_WorkflowDataVariables_ShouldCreateDefinitionsAndParseSources()
        {
            // Arrange
            const string firstSource =
                "[//WorkflowData/FirstSource]";

            const string secondSource =
                "[//WorkflowData/SecondSource]";

            Hashtable variablesTable =
                new Hashtable();

            variablesTable["Count"] =
                2;

            variablesTable["0:0"] =
                firstSource;

            variablesTable["0:1"] =
                "[//WorkflowData/FirstTarget]";

            variablesTable["0:2"] =
                false;

            variablesTable["1:0"] =
                secondSource;

            variablesTable["1:1"] =
                "[//WorkflowData/SecondTarget]";

            variablesTable["1:2"] =
                true;

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Advanced =
                true;

            activity.To =
                "user@contoso.com";

            activity.WorkflowDataVariablesTable =
                variablesTable;

            Dictionary<string, object> lookupCache =
                activity.ActivityExpressionEvaluator.LookupCache;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            List<Definition> definitions =
                ActivityTestHelper.GetField<List<Definition>>(
                    activity,
                    "workflowDataVariables");

            Assert.AreEqual(
                2,
                definitions.Count);

            Assert.AreEqual(
                firstSource,
                definitions[0].Left);

            Assert.AreEqual(
                "[//WorkflowData/FirstTarget]",
                definitions[0].Right);

            Assert.IsFalse(
                definitions[0].Check);

            Assert.AreEqual(
                secondSource,
                definitions[1].Left);

            Assert.AreEqual(
                "[//WorkflowData/SecondTarget]",
                definitions[1].Right);

            Assert.IsTrue(
                definitions[1].Check);

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
        public void Activities_SendEmailNotification_Prepare_ShouldParseConfiguredExpressions()
        {
            // Arrange
            const string iteration =
                "[//WorkflowData/IterationValues]";

            const string condition =
                "[//WorkflowData/ExecuteCondition]";

            const string to =
                "[//WorkflowData/ToRecipients]";

            const string cc =
                "[//WorkflowData/CcRecipients]";

            const string bcc =
                "[//WorkflowData/BccRecipients]";

            const string emailTemplate =
                "[//WorkflowData/EmailTemplate]";

            SendEmailNotification activity =
                new SendEmailNotification();

            activity.Advanced =
                true;

            activity.Iteration =
                iteration;

            activity.ActivityExecutionCondition =
                condition;

            activity.To =
                to;

            activity.CC =
                cc;

            activity.Bcc =
                bcc;

            activity.EmailTemplate =
                emailTemplate;

            Dictionary<string, object> lookupCache =
                activity.ActivityExpressionEvaluator.LookupCache;

            // Act
            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "Prepare_ExecuteCode",
                null,
                EventArgs.Empty);

            // Assert
            Assert.IsTrue(
                lookupCache.ContainsKey(iteration));

            Assert.IsTrue(
                lookupCache.ContainsKey(condition));

            Assert.IsTrue(
                lookupCache.ContainsKey(to));

            Assert.IsTrue(
                lookupCache.ContainsKey(cc));

            Assert.IsTrue(
                lookupCache.ContainsKey(bcc));

            Assert.IsTrue(
                lookupCache.ContainsKey(emailTemplate));

            Assert.IsNull(
                lookupCache[iteration]);

            Assert.IsNull(
                lookupCache[condition]);

            Assert.IsNull(
                lookupCache[to]);

            Assert.IsNull(
                lookupCache[cc]);

            Assert.IsNull(
                lookupCache[bcc]);

            Assert.IsNull(
                lookupCache[emailTemplate]);
        }

    }
}
