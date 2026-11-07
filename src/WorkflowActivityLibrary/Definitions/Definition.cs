//-----------------------------------------------------------------------------------------------------------------------
// <copyright file="Definition.cs" company="Microsoft">
//      Copyright (c) Microsoft. All Rights Reserved.
//      Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>
// <summary>
// Definition class
// </summary>
//-----------------------------------------------------------------------------------------------------------------------

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Definitions
{
    #region Namespaces Declarations

    using System;
    using System.Diagnostics.CodeAnalysis;

    #endregion

    /// <summary>
    /// Defines the equation for task to be performed.
    /// </summary>
    [SuppressMessage("SonarAnalyzer.CSharp", "S5766", Justification = "The constructor doesn't really perform strict validation. It only trims strings.")]
    [Serializable]    
    public class Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Definition"/> class.
        /// </summary>
        /// <param name="left">The left-hand side of the task equation.</param>
        /// <param name="right">The right-hand side of the task equation..</param>
        /// <param name="check">if set to true, the right-hand side can be assigned null value.</param>
        public Definition(string left, string right, bool check)
        {
            this.Left = !string.IsNullOrEmpty(left) ? left.Trim() : left;
            this.Right = !string.IsNullOrEmpty(right) ? right.Trim() : right;
            this.Check = check;
        }

        public void OnDeserialization(object sender)
        {
            if (this.Left != null)
            {
                this.Left = this.Left.Trim();
            }

            if (this.Right != null)
            {
                this.Right = this.Right.Trim();
            }
        }

        #region Properties

        /// <summary>
        /// Gets the left-hand side of the task equation.
        /// </summary>
        public string Left { get; private set; }

        /// <summary>
        /// Gets the right-hand side of the task equation.
        /// </summary>
        public string Right { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the right-hand side can be assigned null value.
        /// </summary>
        public bool Check { get; private set; }
        
        #endregion
    }
}