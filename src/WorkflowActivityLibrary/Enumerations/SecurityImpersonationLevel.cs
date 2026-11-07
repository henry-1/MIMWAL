using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations
{
    public enum SecurityImpersonationLevel
    {
        SecurityAnonymous = 0,
        SecurityIdentification = 1,
        SecurityImpersonation = 2,
        SecurityDelegation = 3
    }
}
