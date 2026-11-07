//-----------------------------------------------------------------------------------------------------------------------
// <copyright file="RunPowerShellScript.cs" company="Microsoft">
//      Copyright (c) Microsoft. All Rights Reserved.
//      Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>
// <summary>
// RunPowerShellScript Activity 
// </summary>
//-----------------------------------------------------------------------------------------------------------------------

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Activities
{
    #region Namespaces Declarations

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;    
    using System.Security;
    using System.Text;
    using System.Workflow.Activities;
    using System.Workflow.ComponentModel;
    using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
    using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
    using System.Net;
    using System.Web.Script.Serialization;
    using System.Security.Cryptography;

    using System.Runtime.InteropServices;

    #endregion

    /// <summary>
    /// Runs PowerShell Script
    /// </summary>
    [SuppressMessage("SonarAnalyzer.CSharp", "S2223", Justification = "DependencyProperty")]
    [SuppressMessage("SonarAnalyzer.CSharp", "S1104", Justification = "DependencyProperty")]
    public partial class RunPowerShellScript : SequenceActivity
    {
        #region Windows Workflow Designer generated code

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty PowerShellTimeoutProperty =
            DependencyProperty.Register("PowerShellTimeout", typeof(int), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty PowerShellVersionProperty =
            DependencyProperty.Register("PowerShellVersion", typeof(string), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ActivityDisplayNameProperty =
            DependencyProperty.Register("ActivityDisplayName", typeof(string), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ActivityExecutionConditionProperty =
            DependencyProperty.Register("ActivityExecutionCondition", typeof(string), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ScriptLocationProperty =
            DependencyProperty.Register("ScriptLocation", typeof(PowerShellScriptLocation), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ScriptProperty =
            DependencyProperty.Register("Script", typeof(string), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty FailOnMissingProperty =
            DependencyProperty.Register("FailOnMissing", typeof(bool), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType", typeof(PowerShellInputType), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ParametersTableProperty =
            DependencyProperty.Register("ParametersTable", typeof(Hashtable), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ArgumentsProperty =
            DependencyProperty.Register("Arguments", typeof(ArrayList), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ReturnTypeProperty =
            DependencyProperty.Register("ReturnType", typeof(PowerShellReturnType), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ReturnValueLookupProperty =
            DependencyProperty.Register("ReturnValueLookup", typeof(string), typeof(RunPowerShellScript));

        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty AdvancedProperty =
            DependencyProperty.Register("Advanced", typeof(bool), typeof(RunPowerShellScript));

        /// <summary>
        /// DependencyProperty for PowerShell User
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty PowerShellUserProperty = DependencyProperty.Register("PowerShellUser", typeof(string), typeof(RunPowerShellScript));

        /// <summary>
        /// DependencyProperty for PowerShell Password
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty PowerShellUserPasswordProperty = DependencyProperty.Register("PowerShellUserPassword", typeof(string), typeof(RunPowerShellScript));

        /// <summary>
        /// DependencyProperty for Impersonate PowerShell User
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ImpersonatePowerShellUserProperty = DependencyProperty.Register("ImpersonatePowerShellUser", typeof(bool), typeof(RunPowerShellScript));

        /// <summary>
        /// DependencyProperty for Impersonate PowerShell User Logon Type
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ImpersonatePowerShellUserLogOnTypeProperty = DependencyProperty.Register("ImpersonatePowerShellUserLogOnType", typeof(LogOnType), typeof(RunPowerShellScript));

        /// <summary>
        /// DependencyProperty for Impersonate PowerShell User Load User Profile
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "DependencyProperty")]
        public static DependencyProperty ImpersonatePowerShellUserLoadUserProfileProperty = DependencyProperty.Register("ImpersonatePowerShellUserLoadUserProfile", typeof(bool), typeof(RunPowerShellScript));

        #endregion Windows Workflow Designer generated code

        #region Declarations

        /// <summary>
        /// The expression evaluator.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Reviewed. VS designer renders public properties as dependency property.")]
        public ExpressionEvaluator ActivityExpressionEvaluator;

        /// <summary>
        /// The parameter definitions.
        /// </summary>
        private List<Definition> parameters = new List<Definition>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPowerShellScript"/> class.
        /// </summary>
        public RunPowerShellScript()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptConstructor);

            try
            {
                this.InitializeComponent();

                if (this.ActivityExpressionEvaluator == null)
                {
                    this.ActivityExpressionEvaluator = new ExpressionEvaluator();
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptConstructor);
            }
        }

        #region Dependancy Property Properties

        [Description("Timeout value for the powershell script")]
        [Category("Settings")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public int PowerShellTimeout
        {
            get
            {
                return ((int)(this.GetValue(PowerShellTimeoutProperty)));
            }

            set
            {
                this.SetValue(PowerShellTimeoutProperty, value);
            }
        }

        [Description("Determines whether to run the script using Windows PowerShell or PowerShell 7.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string PowerShellVersion
        {
            get {
                return ((string)(this.GetValue(PowerShellVersionProperty)));
            }

            set {
                this.SetValue(PowerShellVersionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the display name of the activity.
        /// </summary>
        [Description("ActivityDisplayName")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ActivityDisplayName
        {
            get
            {
                return (string)this.GetValue(ActivityDisplayNameProperty);
            }

            set
            {
                this.SetValue(ActivityDisplayNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        [Description("Activity Execution Condition")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ActivityExecutionCondition
        {
            get
            {
                return (string)this.GetValue(ActivityExecutionConditionProperty);
            }

            set
            {
                this.SetValue(ActivityExecutionConditionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the script location.
        /// </summary>
        [Description("ScriptLocation")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PowerShellScriptLocation ScriptLocation
        {
            get
            {
                return (PowerShellScriptLocation)this.GetValue(ScriptLocationProperty);
            }

            set
            {
                this.SetValue(ScriptLocationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        [Description("Script")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Script
        {
            get
            {
                return (string)this.GetValue(ScriptProperty);
            }

            set
            {
                this.SetValue(ScriptProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to fail on missing script.
        /// </summary>
        [Description("FailOnMissing")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool FailOnMissing
        {
            get
            {
                return (bool)this.GetValue(FailOnMissingProperty);
            }

            set
            {
                this.SetValue(FailOnMissingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the input to the script.
        /// </summary>
        [Description("InputType")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PowerShellInputType InputType
        {
            get
            {
                return (PowerShellInputType)this.GetValue(InputTypeProperty);
            }

            set
            {
                this.SetValue(InputTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the script parameters table.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed.")]
        [Description("ParametersTable")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Hashtable ParametersTable
        {
            get
            {
                return (Hashtable)this.GetValue(ParametersTableProperty);
            }

            set
            {
                this.SetValue(ParametersTableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the script arguments.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed.")]
        [Description("Arguments")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ArrayList Arguments
        {
            get
            {
                return (ArrayList)this.GetValue(ArgumentsProperty);
            }

            set
            {
                this.SetValue(ArgumentsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the script return output.
        /// </summary>
        [Description("ReturnType")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PowerShellReturnType ReturnType
        {
            get
            {
                return (PowerShellReturnType)this.GetValue(ReturnTypeProperty);
            }

            set
            {
                this.SetValue(ReturnTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the return value lookup.
        /// </summary>
        [Description("ReturnValueLookup")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ReturnValueLookup
        {
            get
            {
                return (string)this.GetValue(ReturnValueLookupProperty);
            }

            set
            {
                this.SetValue(ReturnValueLookupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether advanced checkbox is checked.
        /// </summary>
        [Description("Advanced")]
        [Category("Settings")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Advanced
        {
            get
            {
                return (bool)this.GetValue(AdvancedProperty);
            }

            set
            {
                this.SetValue(AdvancedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the return value lookup.
        /// </summary>
        [Description("PowerShellUser")]
        [Category("Alternate Credentials")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string PowerShellUser
        {
            get
            {
                return (string)this.GetValue(PowerShellUserProperty);
            }

            set
            {
                this.SetValue(PowerShellUserProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the return value lookup.
        /// </summary>
        [Description("PowerShellUserPassword")]
        [Category("Alternate Credentials")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string PowerShellUserPassword
        {
            get
            {
                return (string)this.GetValue(PowerShellUserPasswordProperty);
            }

            set
            {
                this.SetValue(PowerShellUserPasswordProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to impersonate the PowerShell user or not.
        /// </summary>
        [Description("ImpersonatePowerShellUser")]
        [Category("Alternate Credentials")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ImpersonatePowerShellUser
        {
            get
            {
                return (bool)this.GetValue(ImpersonatePowerShellUserProperty);
            }

            set
            {
                this.SetValue(ImpersonatePowerShellUserProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the return value lookup.
        /// </summary>
        [Description("ImpersonatePowerShellUserLogOnType")]
        [Category("Alternate Credentials")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public LogOnType ImpersonatePowerShellUserLogOnType
        {
            get
            {
                return (LogOnType)this.GetValue(ImpersonatePowerShellUserLogOnTypeProperty);
            }

            set
            {
                this.SetValue(ImpersonatePowerShellUserLogOnTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to load user profile or not when impersonating user.
        /// </summary>
        [Description("ImpersonatePowerShellUserLoadUserProfile")]
        [Category("Alternate Credentials")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ImpersonatePowerShellUserLoadUserProfile
        {
            get
            {
                return (bool)this.GetValue(ImpersonatePowerShellUserLoadUserProfileProperty);
            }

            set
            {
                this.SetValue(ImpersonatePowerShellUserLoadUserProfileProperty, value);
            }
        }

        #endregion Dependancy Property Properties


        #region Methods

        /// <summary>
        /// Executes the activity.
        /// </summary>
        /// <param name="executionContext">The execution context of the activity.</param>
        /// <returns>The <see cref="ActivityExecutionStatus"/> of the activity after executing the activity.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptExecute);

            try
            {
                // Ideally we would set CallContext in OnActivityExecutionContextLoad instead here in Execute
                // as OnActivityExecutionContextLoad gets called on each hydration and rehydration of the workflow instance
                // but looks like it's invoked on a different thread context than the rest of the workflow instance execution.
                // To minimize the loss of the CallContext on rehydration, we'll set it in the Execute of every WAL child activities.
                // It will still get lost (momentarily) when the workflow is persisted in the middle of the execution of a replicator activity, for example.
                Logger.SetContextItem(this, this.WorkflowInstanceId);

                return base.Execute(executionContext);
            }
            catch (Exception ex)
            {
                throw Logger.Instance.ReportError(EventIdentifier.RunPowerShellScriptExecuteError, ex);
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptExecute);
            }
        }

        /// <summary>
        /// Determines the action taken when the activity has completed execution.
        /// </summary>
        /// <param name="executionContext">The execution context of the activity.</param>
        protected override void OnSequenceComplete(ActivityExecutionContext executionContext)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptOnSequenceComplete);

            try
            {
                // Clear the variable cache for the expression evaluator
                // so that any variables, such as SqlParameter, not marked as serializable does not cause dehyration issues.
                if (this.ActivityExpressionEvaluator != null
                    && this.ActivityExpressionEvaluator.VariableCache != null
                    && this.ActivityExpressionEvaluator.VariableCache.Keys != null)
                {
                    List<string> variables = this.ActivityExpressionEvaluator.VariableCache.Keys.ToList();
                    foreach (string variable in variables)
                    {
                        this.ActivityExpressionEvaluator.VariableCache[variable] = null;
                    }
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptOnSequenceComplete);
            }
        }

        /// <summary>
        /// Handles the ExecuteCode event of the Prepare CodeActivity.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Prepare_ExecuteCode(object sender, EventArgs e)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptPrepareExecuteCode);

            try
            {
                // If advanced options were not specified,
                // clear any supplied advanced settings so they do not impact activity execution
                if (!this.Advanced)
                {
                    this.ActivityExecutionCondition = null;
                    this.PowerShellUser = null;
                    this.PowerShellUserPassword = null;
                    this.ImpersonatePowerShellUser = false;
                    this.ImpersonatePowerShellUserLoadUserProfile = false;
                    this.ImpersonatePowerShellUserLogOnType = LogOnType.None;
                }

                // If the script needs to be resolved, parse the expression via the
                // expression evaluator to prepare for lookup resolution
                if (this.ScriptLocation == PowerShellScriptLocation.Resource)
                {
                    this.ActivityExpressionEvaluator.ParseExpression(this.Script);
                }

                // If the activity is configured for conditional execution, parse the associated expression
                this.ActivityExpressionEvaluator.ParseIfExpression(this.ActivityExecutionCondition);

                if (!string.IsNullOrEmpty(this.PowerShellUser))
                {
                    this.ActivityExpressionEvaluator.ParseIfExpression(this.PowerShellUser);
                }

                if (!string.IsNullOrEmpty(this.PowerShellUserPassword))
                {
                    this.ActivityExpressionEvaluator.ParseIfExpression(this.PowerShellUserPassword);
                }

                if (this.InputType == PowerShellInputType.Arguments &&
                    this.Arguments != null &&
                    this.Arguments.Count > 0)
                {
                    // If we're supplying arguments to the PowerShell script,
                    // parse each argument in the list to facilitate resolution
                    foreach (string s in this.Arguments)
                    {
                        this.ActivityExpressionEvaluator.ParseExpression(s);
                    }
                }
                else if (this.InputType == PowerShellInputType.Parameters &&
                         this.ParametersTable != null &&
                         this.ParametersTable.Count > 0)
                {
                    // If we're supplying named parameters ot the PowerShell script,
                    // parse the right side of the parameter listing which represents the value
                    DefinitionsConverter parametersConverter = new DefinitionsConverter(this.ParametersTable);
                    this.parameters = parametersConverter.Definitions;
                    foreach (Definition parameter in this.parameters)
                    {
                        this.ActivityExpressionEvaluator.ParseExpression(parameter.Right);
                    }
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptPrepareExecuteCode);
            }
        }

        /// <summary>
        /// Handles the ExecuteCode event of the Run CodeActivity.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Run_ExecuteCode(object sender, EventArgs e)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptRunExecuteCode, "Script Location: '{0}'.", this.ScriptLocation);

            try
            {
                // The value supplied for the script property may represent a number of things
                // Depending on the supplied script location, determine if the value represents a path to a script file,
                // a lookup to be resolved, or the script itself stored in the workflow definition
                
                var script = GetPowershellScript();

                if (string.IsNullOrEmpty(script))
                    return;
                
                ArrayList arguments = null;
                Dictionary<string, object> scriptParameters = null;
                switch (this.InputType)
                {
                    case PowerShellInputType.Parameters:
                        // If named parameters were supplied as input, resolve each value expression
                        // and add them to the PowerShell instance
                        scriptParameters = GetScriptParameters();
                        break;

                    case PowerShellInputType.Arguments:
                        // If arguments were supplied as input, resolve each argument value
                        // and add them to the PowerShell instance in the order in which they were supplied
                        arguments = GetScriptArguments();
                        break;
                }

                // Run the PowerShell script and capture the results
                object result = this.RunScript(script, arguments, scriptParameters);
                this.PublishOutput.UpdateLookupDefinitions = new List<UpdateLookupDefinition>();
                switch (this.ReturnType)
                {
                    case PowerShellReturnType.Explicit:
                        // If publishing an explicit result, publish the returned value to the specified lookup
                        if (result == null)
                        {
                            Logger.Instance.WriteWarning(EventIdentifier.RunPowerShellScriptRunExecuteCodeNullReturnValueWarning, "PowerShell script did not return any value for Return Value Lookup: {0}.", this.ReturnValueLookup);
                        }

                        this.PublishOutput.UpdateLookupDefinitions.Add(new UpdateLookupDefinition(this.ReturnValueLookup, result, UpdateMode.Modify));
                        break;
                    case PowerShellReturnType.Table:
                        // If publishing a table of results, ensure a hash table was returned by the script
                        // and publish the value for each key to the workflow dictionary using the hash table key as the dictionary key
                        if (result != null && result.GetType() == typeof(Hashtable))
                        {
                            Hashtable results = (Hashtable)result;
                            foreach (string key in results.Keys)
                            {
                                object value = results[key];

                                this.PublishOutput.UpdateLookupDefinitions.Add(new UpdateLookupDefinition(string.Format(CultureInfo.InvariantCulture, "[//WorkflowData/{0}]", key), value, UpdateMode.Modify));
                            }
                        }
                        else
                        {
                            Logger.Instance.WriteWarning(EventIdentifier.RunPowerShellScriptRunExecuteCodeNullHashtableReturnValueWarning, "PowerShell script returned a '{0}' value when the activity was expecting a Hashtable.", result);
                        }

                        break;
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptRunExecuteCode, "Script Location: '{0}'.", this.ScriptLocation);
            }
        }

        private ArrayList GetScriptArguments()
        {
            var arguments = new ArrayList();
            foreach (string s in this.Arguments)
            {
                arguments.Add(this.ActivityExpressionEvaluator.ResolveExpression(s));
            }

            return arguments;
        }

        private Dictionary<string, object> GetScriptParameters()
        {
            var scriptParameters = new Dictionary<string, object>();
            foreach (Definition parameter in this.parameters)
            {
                object resolved = this.ActivityExpressionEvaluator.ResolveExpression(parameter.Right);
                if (resolved != null)
                {
                    scriptParameters.Add(parameter.Left, resolved);
                }
            }

            return scriptParameters;
        }

        private string GetPowershellScript()
        {
            var script = string.Empty;

            switch (this.ScriptLocation)
            {
                case PowerShellScriptLocation.Resource:
                    // If the script needs to be resolved, the supplied script value will be the lookup from which the script should be read
                    // Resolve it using the expression evaluator and throw an error, if configured to do so, when it resolves to null
                    object resolved = this.ActivityExpressionEvaluator.ResolveExpression(this.Script);
                    if (resolved != null)
                    {
                        script = resolved.ToString();
                    }
                    else if (this.FailOnMissing)
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.RunPowerShellScriptRunExecuteCodeScriptNotFoundError, new WorkflowActivityLibraryException(Messages.RunPowerShellActivity_ScriptNotFoundError, this.Script));
                    }

                    break;
                case PowerShellScriptLocation.Disk:
                    // If the script needs to be read from disk, the supplied script value will be the script file path
                    // Attempt to read it and throw an error if it is not found
                    if (File.Exists(this.Script))
                    {
                        using (StreamReader reader = new StreamReader(this.Script))
                        {
                            script = reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        throw Logger.Instance.ReportError(EventIdentifier.RunPowerShellScriptRunExecuteCodeScriptNotFoundError, new WorkflowActivityLibraryException(Messages.RunPowerShellActivity_ScriptNotFoundError, this.Script));
                    }

                    break;
                case PowerShellScriptLocation.WorkflowDefinition:
                    // When the script is stored in the workflow definition, the supplied script value will be the script itself
                    script = this.Script;
                    break;
            }

            return script;
        }

        private static bool IsConsistentType(
            List<object> resultValues)
        {
            Type type = null;
            bool typeInitialized = false;

            foreach (object item in resultValues)
            {
                Type itemType =
                    item == null
                        ? null
                        : item.GetType();

                if (!typeInitialized)
                {
                    type = itemType;
                    typeInitialized = true;
                }
                else if (itemType != type)
                {
                    return false;
                }
            }

            return true;
        }

        private static object GetListResults(object results)
        {
            var objectArray = results as object[];
            List<object> valueList = new List<object>(objectArray);

            var consistentType = IsConsistentType(valueList);

            // If we have multiple values of an inconsistent type, there is a problem
            // which needs to be addressed by the administrator
            if (!consistentType)
            {
                throw new FormatException("Inconsistent Type");
            }

            return valueList;
        }

        private static object GetDictionaryResults(object results)
        {
            var dictionary = (Dictionary<string, object>)results;
            Hashtable table = new Hashtable();

            foreach (string key in dictionary.Keys)
            {
                table.Add(key, dictionary[key]);
            }

            return table;
        }


        private NetworkCredential GetCredential()
        {
            if (string.IsNullOrEmpty(this.PowerShellUser) ||
                string.IsNullOrEmpty(this.PowerShellUserPassword))
            {
                return null;
            }

            string userName = this.PowerShellUser;
            string domain = string.Empty;

            if (userName.Contains('\\'))
            {
                var parts = userName.Split('\\');
                userName = parts[1];
                domain = parts[0];
            }
            string password = GetPowershellUserPassword();

            if (!string.IsNullOrEmpty(domain))
                return new NetworkCredential(userName, password, domain);

            return new NetworkCredential(userName, password);
        }

        private string GetPowershellUserPassword()
        {
            if (string.IsNullOrEmpty(this.PowerShellUserPassword))
                return string.Empty;

            // 1. Base64 decode
            byte[] encryptedBytes = Convert.FromBase64String(this.PowerShellUserPassword);

            // 2. DPAPI decrypt (MIMWAL uses LocalMachine)
            byte[] decryptedBytes = System.Security.Cryptography.ProtectedData.Unprotect(
                encryptedBytes,
                null,
                DataProtectionScope.LocalMachine);

            // 3. Convert bytes → plain password (Unicode = UTF-16LE)
            string password = Encoding.Unicode.GetString(decryptedBytes);

            return password;
        }

        /// <summary>
        /// Runs the PowerShell script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="scriptArguments">The PowerShell script arguments.</param>
        /// <param name="scriptParameters">The PowerShell script parameters.</param>
        /// <returns>The output of the script.</returns>        
        private object RunScript(string script, IEnumerable scriptArguments, Dictionary<string, object> scriptParameters)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptRunScript);

            JavaScriptSerializer serializer = new JavaScriptSerializer();


            NetworkCredential credential = null;
            var psExeName = GetPSExecutableName();
            var psExePath = GetPSExePath(psExeName);

            LogOnType logonType = this.ImpersonatePowerShellUserLogOnType;
            bool loadUserProfile = this.ImpersonatePowerShellUserLoadUserProfile;

            try
            {
                if (this.ImpersonatePowerShellUser)
                {
                    credential = this.GetCredential();
                }

                string programDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                // ------------------------------------------------------------
                // Build environment (replacement for InitialSessionState)
                // ------------------------------------------------------------

                /*
                   Values are set in Microsoft.ResourceManagement.Service.exe.config
                   <system.diagnostics><switches>
                        <add name="AllSwitch" value="Information" />
                        Information: Evaluates Logger.Instance.ShouldTrace(TraceEventType.Information) to true. This sets your progressPreference to Continue.
                        Verbose: Evaluates TraceEventType.Verbose to true. This sets both debugPreference and verbosePreference to Continue.
                        Warning / Error / Critical: Evaluates Information and Verbose to false. This sets those preferences back to SilentlyContinue, hiding trace progression logs.
                */
                var pwd = GetPowershellUserPassword();
                var preferences = CreatePreferences();
                var environment = CreateEnvironment(pwd);    
                var finalScriptToExecute = CreatePowershellExecutionString(script, scriptArguments, scriptParameters, environment, preferences);

                // 5. Convert to Base64 (Unicode)
                var bytes = System.Text.Encoding.Unicode.GetBytes(finalScriptToExecute);
                var encodedCommand = Convert.ToBase64String(bytes);

                PowerShellResult powerShellResult;

                if (this.ImpersonatePowerShellUser)
                {
                    // ------------------------------------------------------------
                    // Impersonate a user and run a process executing Powershell
                    // ------------------------------------------------------------

                    powerShellResult = MimProcessLauncher.RunPowerShellAsUser(credential.UserName, credential.Domain, pwd,
                       psExePath, encodedCommand,
                       logonType, loadUserProfile,
                       environment, this.PowerShellTimeout);
                }
                else
                {
                    // ------------------------------------------------------------
                    // Run Powershell as service account
                    // ------------------------------------------------------------
                    powerShellResult = MimProcessLauncher.RunPowershellAsService(psExePath, programDataFolder, encodedCommand, this.PowerShellTimeout);
                }

                //Handle possible errors
                if (powerShellResult.TimedOut)
                    throw new ExternalException(powerShellResult.StandardError);

                if (powerShellResult.ExitCode != 0)
                {
                    var msg = string.Format("Powershell script ended with one or more error(s): {0}", powerShellResult.StandardError);
                    throw new ExternalException(msg);
                }

                // Handle possible results
                if (string.IsNullOrEmpty(powerShellResult.StandardOutput))
                {
                    return null;
                }
                else
                {
                    object results = serializer.DeserializeObject(powerShellResult.StandardOutput);

                    if (results == null)
                        return results;

                    if (results.GetType().ToString().Contains("Dictionary"))
                        return GetDictionaryResults(results);

                    if (results.GetType().ToString().Contains("Object[]"))
                        return GetListResults(results);

                    return results;
                }
            }
            catch (Exception ex)
            {
                throw Logger.Instance.ReportError(
                    EventIdentifier.RunPowerShellScriptRunScriptExecutionFailedError,
                    ex);
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptRunScript);
            }
        }

        private static Dictionary<string, string> CreatePreferences()
        {
            var preferences = new Dictionary<string, string>();

            bool progressPreference = Logger.Instance.ShouldTrace(TraceEventType.Information);
            bool debugPreference = Logger.Instance.ShouldTrace(TraceEventType.Information);
            bool verbosePreference = Logger.Instance.ShouldTrace(TraceEventType.Verbose);

            // When using ErrorActionPreference Stop, Powershell always returns an error code 
            // other then zero to the calling process also for non-terminating errors.
            // Users can always overwrite the preference in their scripts.
            preferences["ErrorActionPreference"] = "Stop";
            preferences["ProgressPreference"] = progressPreference ? "Continue" : "SilentlyContinue";
            preferences["DebugPreference"] = debugPreference ? "Continue" : "SilentlyContinue";
            preferences["VerbosePreference"] = verbosePreference ? "Continue" : "SilentlyContinue";

            return preferences;
        }

        private Dictionary<string, string> CreateEnvironment(string pwd)
        {
            // ------------------------------------------------------------
            // Build Powershell Environment
            // ------------------------------------------------------------

            var env = new Dictionary<string, string>();            

            env["AECWorkflowInstanceId"] = Logger.GetContextItem("WorkflowInstanceId");
            env["AECRequestId"] = Logger.GetContextItem("RequestId");
            env["AECActorId"] = Logger.GetContextItem("ActorId");
            env["AECTargetId"] = Logger.GetContextItem("TargetId");
            env["AECWorkflowDefinitionId"] = Logger.GetContextItem("WorkflowDefinitionId");

            // Credential cannot be passed directly → pass as placeholder
            env["CredentialUser"] = string.IsNullOrEmpty(this.PowerShellUser) ? string.Empty : this.PowerShellUser;
            env["CredentialPassword"] = pwd;

            return env;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S125", Justification = "Provide a comment which includes Powershell")]
        private static string CreatePowershellExecutionString(string script, IEnumerable scriptArguments, Dictionary<string, object> scriptParameters, Dictionary<string, string> env, Dictionary<string, string> preferences)
        {   
            StringBuilder scriptBuilder = new StringBuilder();
            

            foreach (KeyValuePair<string, string> p in preferences)
            {
                string preferenceValue = p.Value ?? string.Empty;
                string safeValue = preferenceValue.Replace("'", "''");  // Escape single quotes
                scriptBuilder.AppendLine(string.Format("${0} = '{1}';", p.Key, safeValue));
            }

            foreach (KeyValuePair<string, string> e in env)
            {
                string environmentValue = e.Value ?? string.Empty;
                string safeValue = environmentValue.Replace("'", "''");  // Escape single quotes
                                                                            // Generates: $env:AECWorkflowInstanceId = 'WF-99999';
                scriptBuilder.AppendLine(string.Format("$env:{0} = '{1}';", e.Key, safeValue));
            }

            if (scriptArguments != null && scriptArguments.Cast<object>().Any())
            {
                // ------------------------------------------------------------
                //  Powershell Arguments
                // ------------------------------------------------------------
                scriptBuilder.AppendLine("$args = @(");
                foreach (object arg in scriptArguments)
                {
                    scriptBuilder.AppendLine(string.Format("    {0}", ToPowerShellLiteral(arg)));
                }
                scriptBuilder.AppendLine(");");
            }
            else if (scriptParameters != null && scriptParameters.Count > 0)
            {
                // ------------------------------------------------------------
                //  Powershell Parameters
                // ------------------------------------------------------------
                scriptBuilder.AppendLine("$params = @{");
                foreach (KeyValuePair<string, object> param in scriptParameters)
                {
                    scriptBuilder.AppendLine(string.Format("    {0} = {1}", param.Key, ToPowerShellLiteral(param.Value)));
                }
                scriptBuilder.AppendLine("};");
            }


            // 2. Define an inline ScriptBlock wrapper that receives the splatted parameters
            scriptBuilder.AppendLine("$scriptBlock = {");
            scriptBuilder.AppendLine(script);
            scriptBuilder.AppendLine("}");

            // 4. Capture execution to $result and implement a wrapper for JSON serialization logic
            if (scriptArguments != null && scriptArguments.Cast<object>().Any())
            {
                scriptBuilder.AppendLine("$result = & $scriptBlock @args;");
            }
            else if (scriptParameters != null && scriptParameters.Count > 0)
            {
                scriptBuilder.AppendLine("$result = & $scriptBlock @params;");
            }            
            else
            {
                scriptBuilder.AppendLine("$result = & $scriptBlock;");
            }
            scriptBuilder.AppendLine("if ($null -eq $result) { return $null };");

            // to get a predictable result, I convert it into JSON
            scriptBuilder.AppendLine("return ($result | ConvertTo-Json -Compress);");

            return scriptBuilder.ToString();
        }


        private static string ToPowerShellLiteral(object value)
        {
            if (value == null)
                return "$null";

            Type type = value.GetType();

            if (type == typeof(string))
            {
                return "'" + ((string)value).Replace("'", "''") + "'";
            }

            if (type == typeof(bool))
            {
                return ((bool)value) ? "$true" : "$false";
            }

            if (type == typeof(char))
            {
                return "'" + value.ToString().Replace("'", "''") + "'";
            }

            if (type.IsEnum)
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture)
                    .ToString(CultureInfo.InvariantCulture);
            }

            if (type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal))
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            if (type == typeof(DateTime))
            {
                return "[datetime]'" +
                    ((DateTime)value).ToString("o", CultureInfo.InvariantCulture) +
                    "'";
            }

            if (type == typeof(Guid))
            {
                return "[guid]'" + value.ToString() + "'";
            }

            if (value is IEnumerable && !(value is string))
            {
                IEnumerable enumerable = (IEnumerable)value;
                var items = enumerable.Cast<object>().Select(item => ToPowerShellLiteral(item));
                return "@(" + string.Join(", ", items.ToArray()) + ")";
            }

            throw new NotSupportedException(
                "Unsupported parameter type: " + type.FullName);
        }
                
        private string GetPSExecutableName()
        {
            var selectedVersion = string.IsNullOrEmpty(this.PowerShellVersion) ? PowershellVersion.WindowsPowershell.ToString() : this.PowerShellVersion;
            return selectedVersion == PowershellVersion.Powershell.ToString() ? "pwsh.exe" : "powershell.exe";
        }

        private static string GetPSExePath(string psExeName)
        {
            string fullPath = string.Empty;
            string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

            foreach (var path in pathEnv.Split(Path.PathSeparator))
            {
                fullPath = Path.Combine(path, psExeName);
                if (File.Exists(fullPath))
                {
                    return fullPath;                    
                }
            }

            var msg = string.Format("Unable to find path to {0}", psExeName);
            throw new FileNotFoundException(msg);
        }

        #endregion Methods

        #region Conditions

        /// <summary>
        /// Handles the Condition event of the ActivityExecutionConditionSatisfied condition.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ConditionalEventArgs"/> instance containing the event data.</param>
        private void ActivityExecutionConditionSatisfied_Condition(object sender, ConditionalEventArgs e)
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.RunPowerShellScriptActivityExecutionConditionSatisfiedCondition, "Condition: '{0}'.", this.ActivityExecutionCondition);

            try
            {
                // Determine if requests should be submitted based on whether or not a condition was supplied
                // and if that condition resolves to true
                if (string.IsNullOrEmpty(this.ActivityExecutionCondition))
                {
                    e.Result = true;
                }
                else
                {
                    object resolved = this.ActivityExpressionEvaluator.ResolveExpression(this.ActivityExecutionCondition);
                    if (resolved is bool && (bool)resolved)
                    {
                        e.Result = true;
                    }
                }
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.RunPowerShellScriptActivityExecutionConditionSatisfiedCondition, "Condition: '{0}'. Condition evaluated '{1}'.", this.ActivityExecutionCondition, e.Result);
            }
        }

        #endregion Conditions

    }
}