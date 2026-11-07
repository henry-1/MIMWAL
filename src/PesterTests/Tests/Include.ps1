

function Set-TestExpression
{
    param(
        $Workflow,
        [string]$NewExpression,
        [string]$LastExpression
    )

    $xoml = $Workflow.XOML

    $xoml = $xoml.Replace(
        $LastExpression,
        $NewExpression)

    $Workflow.XOML = $xoml
    $Workflow | Save-Resource
}

function Update-User {
    param(
        $ObjectID,
        $NewValue
    )

    $user = Get-Resource -ID $ObjectID
    $user.MiddleName = $NewValue
    $user | Save-Resource

}

function Test-Boolean {
    param(        
        $workflow,
        [string]$newExpression,
        [string]$lastExpression,
        $userID,
        $index
    )

    Write-Host $newExpression
    Set-TestExpression -Workflow $workflow -NewExpression $newExpression -LastExpression $lastExpression
    
    Update-User -ObjectID $userID -NewValue $index.ToString()

    # Allow time for the asynchronous MIM workflow to complete.
    Start-Sleep -Milliseconds 500

    $content = Get-Content -Path "C:\DEV\Logs\result.txt"
    return $content

}