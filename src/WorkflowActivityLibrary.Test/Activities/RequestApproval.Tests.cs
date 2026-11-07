using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities;
using System.Reflection;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System.Collections.Generic;
using System.Workflow.Activities;

namespace WorkflowActivityLibrary.Tests.Activities
{
    [TestClass]
    public class RequestApprovalTests
    {
        [TestMethod]
        public void Activities_RequestApproval_Constructor_Should_InitializeExpressionEvaluator()
        {
            RequestApproval activity = new RequestApproval();

            Assert.IsNotNull(
                activity.ActivityExpressionEvaluator);
        }


        [TestMethod]
        public void Activities_RequestApproval_ResolveThreshold_StringValue_Should_SetThresholdNumber()
        {
            RequestApproval activity = new RequestApproval();

            activity.Threshold = "3";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveThreshold_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                3,
                activity.ThresholdNumber);
        }

        [TestMethod]
        public void Activities_RequestApproval_ResolveThreshold_Expression_Should_SetThresholdNumber()
        {
            RequestApproval activity = new RequestApproval();

            activity.ActivityExpressionEvaluator.VariableCache["$Threshold"] =
                5;

            activity.Threshold = "$Threshold";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Threshold);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveThreshold_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                5,
                activity.ThresholdNumber);
        }

        [TestMethod]
        public void Activities_RequestApproval_ResolveDuration_StringValue_Should_SetDurationTimeSpan()
        {
            RequestApproval activity = new RequestApproval();

            activity.Duration = "01:30:00";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveDuration_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                TimeSpan.FromMinutes(90),
                activity.DurationTimeSpan);
        }


        [TestMethod]
        public void Activities_RequestApproval_ResolveDuration_Expression_Should_SetDurationTimeSpan()
        {
            RequestApproval activity = new RequestApproval();

            TimeSpan expectedDuration =
                TimeSpan.FromMinutes(90);

            activity.ActivityExpressionEvaluator.VariableCache["$Duration"] =
                expectedDuration;

            activity.Duration = "$Duration";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Duration);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveDuration_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedDuration,
                activity.DurationTimeSpan);
        }

        [TestMethod]
        public void Activities_RequestApproval_ResolveDuration_InvalidValue_Should_ThrowException()
        {
            RequestApproval activity = new RequestApproval();

            activity.Duration = "not-a-duration";

            try
            {
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ResolveDuration_ExecuteCode",
                    activity,
                    EventArgs.Empty);

                Assert.Fail(
                    "Expected an exception for an invalid duration.");
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
        public void Activities_RequestApproval_GetEmailTemplateGuid_GuidString_Should_ReturnGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid = Guid.NewGuid();

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    expectedGuid.ToString());

            Guid actualGuid =
                (Guid)result;

            Assert.AreEqual(
                expectedGuid,
                actualGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_GetEmailTemplateGuid_ExpressionReturningGuid_Should_ReturnGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid = Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Template"] =
                expectedGuid;

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                "$Template");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    "$Template");

            Guid actualGuid =
                (Guid)result;

            Assert.AreEqual(
                expectedGuid,
                actualGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_GetEmailTemplateGuid_ExpressionReturningByteArray_Should_ReturnGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid = Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Template"] =
                expectedGuid.ToByteArray();

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                "$Template");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    "$Template");

            Guid actualGuid =
                (Guid)result;

            Assert.AreEqual(
                expectedGuid,
                actualGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_FormatRecipient_EmailAddresses_Should_PreserveValue()
        {
            RequestApproval activity = new RequestApproval();

            string recipients =
                "approver1@example.com;approver2@example.com";

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    recipients);

            string formattedRecipients =
                (string)result;

            Assert.AreEqual(
                recipients,
                formattedRecipients);
        }


        [TestMethod]
        public void Activities_RequestApproval_FormatRecipient_ExpressionReturningGuid_Should_ReturnGuidString()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid = Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Approver"] =
                expectedGuid;

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                "$Approver");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    "$Approver");

            string formattedRecipient =
                (string)result;

            Assert.AreEqual(
                expectedGuid.ToString(),
                formattedRecipient);
        }


        [TestMethod]
        public void Activities_RequestApproval_FormatRecipient_ExpressionReturningGuidList_Should_ReturnSemicolonSeparatedGuids()
        {
            RequestApproval activity = new RequestApproval();

            Guid firstApprover = Guid.NewGuid();
            Guid secondApprover = Guid.NewGuid();

            List<Guid> approvers = new List<Guid>();
            approvers.Add(firstApprover);
            approvers.Add(secondApprover);

            activity.ActivityExpressionEvaluator.VariableCache["$Approvers"] =
                approvers;

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                "$Approvers");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    "$Approvers");

            string formattedRecipients =
                (string)result;

            string expectedRecipients =
                firstApprover.ToString() +
                ";" +
                secondApprover.ToString();

            Assert.AreEqual(
                expectedRecipients,
                formattedRecipients);
        }


        [TestMethod]
        public void Activities_RequestApproval_CheckApproverResources_FoundIds_Should_SetApprovers()
        {
            RequestApproval activity = new RequestApproval();

            Guid firstApprover = Guid.NewGuid();
            Guid secondApprover = Guid.NewGuid();

            object findApprovers =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovers");

            PropertyInfo foundIdsProperty =
                findApprovers.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds = new List<Guid>();
            foundIds.Add(firstApprover);
            foundIds.Add(secondApprover);

            foundIdsProperty.SetValue(
                findApprovers,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckApproverResources_ExecuteCode",
                activity,
                EventArgs.Empty);

            string expectedApprovers =
                firstApprover.ToString() +
                ";" +
                secondApprover.ToString();

            Assert.AreEqual(
                expectedApprovers,
                activity.Approvers);
        }

        [TestMethod]
        public void Activities_RequestApproval_CheckApproverResources_NoFoundIds_Should_ClearApprovers()
        {
            RequestApproval activity = new RequestApproval();

            activity.Approvers =
                "existing-approver@example.com";

            object findApprovers =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovers");

            PropertyInfo foundIdsProperty =
                findApprovers.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            foundIdsProperty.SetValue(
                findApprovers,
                new List<Guid>(),
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckApproverResources_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.IsNull(
                activity.Approvers);
        }

        [TestMethod]
        public void Activities_RequestApproval_CheckApprovalEmailTemplateResource_OneFoundId_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedTemplateGuid =
                Guid.NewGuid();

            activity.ApprovalEmailTemplate =
                "/EmailTemplate[DisplayName='Approval']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovalEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(
                expectedTemplateGuid);

            foundIdsProperty.SetValue(
                findTemplate,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckApprovalEmailTemplateResource_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedTemplateGuid,
                activity.ApprovalEmailTemplateGuid);
        }


        [TestMethod]
        public void Activities_RequestApproval_CheckApprovalEmailTemplateResource_NoFoundIds_Should_ThrowException()
        {
            RequestApproval activity = new RequestApproval();

            activity.ApprovalEmailTemplate =
                "/EmailTemplate[DisplayName='Approval']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovalEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            foundIdsProperty.SetValue(
                findTemplate,
                new List<Guid>(),
                null);

            try
            {
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckApprovalEmailTemplateResource_ExecuteCode",
                    activity,
                    EventArgs.Empty);

                Assert.Fail(
                    "Expected an exception when no approval email template was found.");
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
        public void Activities_RequestApproval_CheckApprovalEmailTemplateResource_MultipleFoundIds_Should_ThrowException()
        {
            RequestApproval activity = new RequestApproval();

            activity.ApprovalEmailTemplate =
                "/EmailTemplate[DisplayName='Approval']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovalEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(Guid.NewGuid());
            foundIds.Add(Guid.NewGuid());

            foundIdsProperty.SetValue(
                findTemplate,
                foundIds,
                null);

            try
            {
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "CheckApprovalEmailTemplateResource_ExecuteCode",
                    activity,
                    EventArgs.Empty);

                Assert.Fail(
                    "Expected an exception when multiple approval email templates were found.");
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
        public void Activities_RequestApproval_ResolveApprovalEmailTemplate_GuidString_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid =
                Guid.NewGuid();

            activity.ApprovalEmailTemplate =
                expectedGuid.ToString();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveApprovalEmailTemplate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedGuid,
                activity.ApprovalEmailTemplateGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_ResolveEscalation_ExpressionReturningGuid_Should_SetEscalation()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid = Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Escalation"] =
                expectedGuid;

            activity.Escalation = "$Escalation";

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                activity.Escalation);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEscalation_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedGuid.ToString(),
                activity.Escalation);
        }

        [TestMethod]
        public void Activities_RequestApproval_ResolveEscalationEmailTemplate_GuidString_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid =
                Guid.NewGuid();

            activity.EscalationEmailTemplate =
                expectedGuid.ToString();

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEscalationEmailTemplate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedGuid,
                activity.EscalationEmailTemplateGuid);
        }


        [TestMethod]
        public void Activities_RequestApproval_CheckEscalationResources_FoundIds_Should_SetEscalation()
        {
            RequestApproval activity = new RequestApproval();

            Guid firstEscalation = Guid.NewGuid();
            Guid secondEscalation = Guid.NewGuid();

            object findEscalation =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindEscalation");

            PropertyInfo foundIdsProperty =
                findEscalation.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(firstEscalation);
            foundIds.Add(secondEscalation);

            foundIdsProperty.SetValue(
                findEscalation,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckEscalationResources_ExecuteCode",
                activity,
                EventArgs.Empty);

            string expectedEscalation =
                firstEscalation.ToString() +
                ";" +
                secondEscalation.ToString();

            Assert.AreEqual(
                expectedEscalation,
                activity.Escalation);
        }

        [TestMethod]
        public void Activities_RequestApproval_ApproversIsXPath_Should_ReturnExpectedResult()
        {
            object[,] testCases = new object[,]
            {
        { "/Person[DisplayName='Test']", true },
        { "approver@example.com", false }
            };

            for (int i = 0; i < testCases.GetLength(0); i++)
            {
                RequestApproval activity = new RequestApproval();

                activity.Approvers =
                    (string)testCases[i, 0];

                bool expected =
                    (bool)testCases[i, 1];

                ConditionalEventArgs args =
                    new ConditionalEventArgs();

                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "ApproversIsXPath_Condition",
                    activity,
                    args);

                Assert.AreEqual(
                    expected,
                    args.Result,
                    "Unexpected result for Approvers '" +
                    activity.Approvers +
                    "'.");
            }
        }

        [TestMethod]
        public void Activities_RequestApproval_ParseExpressions_Should_ParseApprovers()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedApprover = Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Approver"] =
                expectedApprover;

            activity.Approvers = "$Approver";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ParseExpressions_ExecuteCode",
                activity,
                EventArgs.Empty);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveApprovers_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedApprover.ToString(),
                activity.Approvers);
        }

        [TestMethod]
        public void Activities_RequestApproval_GetEmailTemplateGuid_ExpressionReturningGuidString_Should_ReturnGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid =
                Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Template"] =
                expectedGuid.ToString();

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                "$Template");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "GetEmailTemplateGuid",
                    "$Template");

            Guid actualGuid =
                (Guid)result;

            Assert.AreEqual(
                expectedGuid,
                actualGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_ExecutionCondition_Should_ReturnExpectedResult()
        {
            object[,] testCases = new object[,]
            {
                { null, true },
                { string.Empty, true },
                { "true", true },
                { "false", false }
            };

            for (int i = 0; i < testCases.GetLength(0); i++)
            {
                RequestApproval activity =
                    new RequestApproval();

                string condition =
                    (string)testCases[i, 0];

                bool expected =
                    (bool)testCases[i, 1];

                activity.ActivityExecutionCondition =
                    condition;

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
                    Convert.ToString(condition) +
                    "'.");
            }
        }

        [TestMethod]
        public void Activities_RequestApproval_OnSequenceComplete_Should_ClearVariableCacheValues()
        {
            RequestApproval activity = new RequestApproval();

            activity.ActivityExpressionEvaluator.VariableCache["$Approvers"] =
                "approver@example.com";

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "OnSequenceComplete",
                new object[] { null });

            Assert.IsTrue(
                activity.ActivityExpressionEvaluator.VariableCache.ContainsKey(
                    "$Approvers"));

            Assert.IsNull(
                activity.ActivityExpressionEvaluator.VariableCache[
                    "$Approvers"]);
        }

        [TestMethod]
        public void Activities_RequestApproval_ResolveEscalationEmailTemplate_Empty_Should_NotChangeTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid existingGuid =
                Guid.NewGuid();

            activity.EscalationEmailTemplate =
                string.Empty;

            activity.EscalationEmailTemplateGuid =
                existingGuid;

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "ResolveEscalationEmailTemplate_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                existingGuid,
                activity.EscalationEmailTemplateGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_CheckApprovalCompleteEmailTemplateResource_OneFoundId_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedTemplateGuid =
                Guid.NewGuid();

            activity.ApprovalCompleteEmailTemplate =
                "/EmailTemplate[DisplayName='Approval Complete']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovalCompleteEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(
                expectedTemplateGuid);

            foundIdsProperty.SetValue(
                findTemplate,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckApprovalCompleteEmailTemplateResource_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedTemplateGuid,
                activity.ApprovalCompleteEmailTemplateGuid);
        }


        [TestMethod]
        public void Activities_RequestApproval_CheckApprovalDeniedEmailTemplateResource_OneFoundId_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedTemplateGuid =
                Guid.NewGuid();

            activity.ApprovalDeniedEmailTemplate =
                "/EmailTemplate[DisplayName='Approval Denied']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovalDeniedEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(
                expectedTemplateGuid);

            foundIdsProperty.SetValue(
                findTemplate,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckApprovalDeniedEmailTemplateResource_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedTemplateGuid,
                activity.ApprovalDeniedEmailTemplateGuid);
        }

        [TestMethod]
        public void Activities_RequestApproval_CheckApprovalTimeoutEmailTemplateResource_OneFoundId_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedTemplateGuid =
                Guid.NewGuid();

            activity.ApprovalTimeoutEmailTemplate =
                "/EmailTemplate[DisplayName='Approval Timeout']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindApprovalTimeoutEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(
                expectedTemplateGuid);

            foundIdsProperty.SetValue(
                findTemplate,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckApprovalTimeoutEmailTemplateResource_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedTemplateGuid,
                activity.ApprovalTimeoutEmailTemplateGuid);
        }


        [TestMethod]
        public void Activities_RequestApproval_CheckEscalationEmailTemplateResource_OneFoundId_Should_SetTemplateGuid()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedTemplateGuid =
                Guid.NewGuid();

            activity.EscalationEmailTemplate =
                "/EmailTemplate[DisplayName='Escalation']";

            object findTemplate =
                ActivityTestHelper.GetField<object>(
                    activity,
                    "FindEscalationEmailTemplate");

            PropertyInfo foundIdsProperty =
                findTemplate.GetType().GetProperty(
                    "FoundIds",
                    BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(
                foundIdsProperty,
                "FoundIds property was not found.");

            List<Guid> foundIds =
                new List<Guid>();

            foundIds.Add(
                expectedTemplateGuid);

            foundIdsProperty.SetValue(
                findTemplate,
                foundIds,
                null);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "CheckEscalationEmailTemplateResource_ExecuteCode",
                activity,
                EventArgs.Empty);

            Assert.AreEqual(
                expectedTemplateGuid,
                activity.EscalationEmailTemplateGuid);
        }


        [TestMethod]
        public void Activities_RequestApproval_FormatRecipient_ExpressionReturningByteArray_Should_ReturnGuidString()
        {
            RequestApproval activity = new RequestApproval();

            Guid expectedGuid =
                Guid.NewGuid();

            activity.ActivityExpressionEvaluator.VariableCache["$Approver"] =
                expectedGuid.ToByteArray();

            activity.ActivityExpressionEvaluator.ParseIfExpression(
                "$Approver");

            object result =
                ActivityTestHelper.InvokePrivateMethod(
                    activity,
                    "FormatRecipient",
                    "$Approver");

            string formattedRecipient =
                (string)result;

            Assert.AreEqual(
                expectedGuid.ToString(),
                formattedRecipient);
        }

        [TestMethod]
        public void Activities_RequestApproval_TraceCreateApproval_Should_NotThrow()
        {
            RequestApproval activity = new RequestApproval();

            activity.Approvers = "approver@example.com";
            activity.Threshold = "1";
            activity.ThresholdNumber = 1;
            activity.Duration = "01:00:00";
            activity.DurationTimeSpan = TimeSpan.FromHours(1);

            ActivityTestHelper.InvokePrivateMethod(
                activity,
                "TraceCreateApproval_ExecuteCode",
                activity,
                EventArgs.Empty);
        }


    }
}
