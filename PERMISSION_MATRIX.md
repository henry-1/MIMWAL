# Permission Requirements
This document describes the necessary permissions for the MIM portal service account as well as for the user account configured in the Powershell workflow activity fpr importsonation.

## Service Account Permissions

| Workflow execution account | Logon as Service | Replace process Level token | Member of Local Administrators group|
| --- | --- | ---- | --- |
| MIM Portal Service account |  ✔ | ❌ | ❌ |
| Impersonated user account| ✔ | ✔ | ❌ |
| Run impersonated as user and load user profile | ✔ | ✔ | ✔ | 

## User Account Permissions

The user account needs one or more of the following permissions according to the setting in the workflow definition:
- Log on as batch job
- Log on as a service
- Log on locally
