using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations
{
    public enum TokenInformationClass
    {
        TokenUser = 1,
        TokenGroups = 2,
        TokenPrivileges = 3,
        TokenOwner = 4,
        TokenPrimaryGroup = 5,
        TokenDefaultDacl = 6,
        TokenSource = 7,
        TokenType = 8,
        TokenImpersonationLevel = 9,
        // ... other values if needed
        TokenStatistics = 10,
        TokenSessionId = 12,
        TokenElevationType = 18,
        TokenElevation = 20
    }
}
