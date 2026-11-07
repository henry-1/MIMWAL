# How to test ExpressionFunctions in a MIM WAL activity using Pester

This document describes how I used Pester for end-to-end testing of MIM WAL Expression Functions.

Each test modifies the XOML of a test workflow to insert the Expression Function to be tested. The test then modifies the MiddleName attribute of a user object, which triggers the workflow. The workflow evaluates the Expression Function and writes the result to a file. The Pester test reads the result from this file and verifies that it matches the expected value.

```
Pester
  -> modify workflow XOML
  -> modify user's MiddleName
  -> MIM workflow starts
  -> Expression Function executes
  -> result written to file
  -> Pester reads file
  -> assertion
```

## Prepare Tests

* Create a workflow with two steps
    * Step 1:
    `'And(True,True)'` flows into `[//WorkflowData/Result]`
    * Step 2:
    Use the Powershell Activity to write `[//WorkflowData/Result]` into a file

* Export the XOML into `Initial_XOML.txt` file.
    * The `'And(True,True)'` part will be replaced later in my tests

* Save the [Include.ps1](#includeps1) PowerShell script.
* Save the [Test-ExpressionFunction.Bool.Tests.ps1](#test-expressionfunctionbooltestsps1) PowerShell script.

```
PesterTests
    | Tests
        - Include.ps1
        - Test-ExpressionFunction.Bool.Tests.ps1
        - Initial_XOML.txt
```

* Adjust the workflow ID, user ID, paths, and output file location to match your MIM test environment.
```
$wfID = "8dd7..."
$userID = "c302..."
C:\MIM\MIMWAL_Scripts\...
C:\DEV\Logs\result.txt
```

* Run the tests.

## Include.ps1
```

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

```

## Test-ExpressionFunction.Bool.Tests.ps1

```

Import-Module Pester

Describe 'Test Boolean' {
    BeforeAll {        
        
        Import-Module LithnetRMA -DisableNameChecking -NoClobber -Force
   
        Set-Location C:\MIM\MIMWAL_Scripts\PesterTests\Tests
        . .\include.ps1

        $wfID = "8dd7ca68-00a0-44cb-9177-9dfcbf1e3254"
        $userID = "c302a07d-5400-43af-94dd-83aa65e5aed4"
    }

    BeforeEach {
        $initialXOML = get-content -Path "C:\MIM\MIMWAL_Scripts\PesterTests\Tests\Initial_XOML.txt" -Raw
        $testWorkflow = Get-Resource -ID $wfID -AttributesToGet XOML
        $testWorkflow.XOML = $initialXOML
        $testWorkflow | Save-Resource

        $index = 0
        $lastExpression = 'And(True,True)'

        $user = Get-Resource -ID $userID
        $user.MiddleName = "Bla"
        $user | Save-Resource
    }
    
    Context 'Boolean' {

        It 'Test And() '{
    
            # Arrange

            $tests = @(
                @{
                    Name       = "And_TrueTrue"
                    Expression = 'And(True,True)'
                    Expected   = $true
                },
                @{
                    Name       = "And_TrueFalse"
                    Expression = 'And(True,False)'
                    Expected   = $false
                },
                @{
                    Name       = "And_FalseFalse"
                    Expression = 'And(False,False)'
                    Expected   = $false
                },
                @{
                    Name       = "And_NullFalse"
                    Expression = 'And(Null(),False)'
                    Expected   = $false
                }
                
            )
                       

            # Act
            foreach($test in $tests)
            {
                $result = Test-Boolean -workflow $testWorkflow -newExpression $($test.Expression) -lastExpression $lastExpression -userID $userID -Index $index
                $lastExpression = $Test.Expression
                
                $index++

                # Assert
                [System.Convert]::ToBoolean($result) | Should-Be $test.Expected

            }

        }

        It 'Test Eq() '{
    
            # Arrange

            $tests = @(    
                @{
                    Name       = "Eq_EqualStrings"
                    Expression = 'Eq("eins","eins")'
                    Expected   = $true
                },
                @{
                    Name       = "Eq_DifferentStrings"
                    Expression = 'Eq("eins","zwei")'
                    Expected   = $false
                },
                @{
                    Name       = "Eq_SameStringsDifferentCase"
                    Expression = 'Eq("Eins","eins")'
                    Expected   = $true
                },
                @{
                    Name       = "Eq_NullNotEq0"
                    Expression = 'Eq(Null(),0)'
                    Expected   = $false
                },
                @{
                    Name       = "Eq_Number"
                    Expression = 'Eq(42,42)'
                    Expected   = $true
                }
            )

            # Act
            foreach($test in $tests)
            {
                $result = Test-Boolean -workflow $testWorkflow -newExpression $($test.Expression) -lastExpression $lastExpression -userID $userID -Index $index
                $lastExpression = $Test.Expression
                
                $index++                

                # Assert
                [System.Convert]::ToBoolean($result) | Should-Be $test.Expected

            }

        }
    
        It 'Test IsPresent() '{
    
            # Arrange

            $tests = @(    
                @{
                    Name       = "IsPresent_String"
                    Expression = 'IsPresent("Test")'
                    Expected   = $true
                },
                @{
                    Name       = "IsPresent_Null"
                    Expression = 'IsPresent(Null())'
                    Expected   = $false
                }
            )

            # Act
            foreach($test in $tests)
            {
                $result = Test-Boolean -workflow $testWorkflow -newExpression $($test.Expression) -lastExpression $lastExpression -userID $userID -Index $index
                $lastExpression = $Test.Expression
                
                $index++                

                # Assert
                [System.Convert]::ToBoolean($result) | Should-Be $test.Expected

            }
        }

        It 'Test LessThan() '{
    
            # Arrange

            $tests = @(    
                @{
                    Name       = "LessThan_Int_ShouldBeTrue"
                    Expression = 'LessThan(5,7)'
                    Expected   = $true
                },
                @{
                    Name       = "LessThan_Int_ShouldBeFalse"
                    Expression = 'LessThan(7,5)'
                    Expected   = $false
                },
                @{
                    Name       = "LessThan_NegativeNumber"
                    Expression = 'LessThan(-7,-5)'
                    Expected   = $true
                },
                @{
                    Name       = "LessThan_Null"
                    Expression = 'LessThan(10,Null())'
                    Expected   = $false
                }
            )

            # Act
            foreach($test in $tests)
            {
                $result = Test-Boolean -workflow $testWorkflow -newExpression $($test.Expression) -lastExpression $lastExpression -userID $userID -Index $index
                $lastExpression = $Test.Expression
                
                $index++                

                # Assert
                [System.Convert]::ToBoolean($result) | Should-Be $test.Expected

            }
        }

        It 'Test Not() '{
    
            # Arrange

            $tests = @(    
                @{
                    Name       = "Not_True"
                    Expression = 'Not(True)'
                    Expected   = $false
                },
                @{
                    Name       = "Not_False"
                    Expression = 'Not(False)'
                    Expected   = $true
                },
                @{
                    Name       = "Not_Null"
                    Expression = 'Not(Null())'
                    Expected   = $True
                }
            )

            # Act
            foreach($test in $tests)
            {
                $result = Test-Boolean -workflow $testWorkflow -newExpression $($test.Expression) -lastExpression $lastExpression -userID $userID -Index $index
                $lastExpression = $Test.Expression
                
                $index++                

                # Assert
                [System.Convert]::ToBoolean($result) | Should-Be $test.Expected

            }
        }
    
        It 'Test Or() '{
    
            # Arrange

            $tests = @(    
                @{
                    Name       = "Or_TrueTrue"
                    Expression = 'Or(True,True)'
                    Expected   = $true
                },
                @{
                    Name       = "Or_TrueFalse"
                    Expression = 'Or(True,False)'
                    Expected   = $true
                },
                @{
                    Name       = "Or_FalseFalse"
                    Expression = 'Or(False,False)'
                    Expected   = $False
                },
                @{
                    Name       = "Or_TrueNull"
                    Expression = 'Or(True,Null())'
                    Expected   = $True
                },
                @{
                    Name       = "Or_FalseNull"
                    Expression = 'Or(False,Null())'
                    Expected   = $False
                }
            )

            # Act
            foreach($test in $tests)
            {
                $result = Test-Boolean -workflow $testWorkflow -newExpression $($test.Expression) -lastExpression $lastExpression -userID $userID -Index $index
                $lastExpression = $Test.Expression
                
                $index++                

                # Assert
                [System.Convert]::ToBoolean($result) | Should-Be $test.Expected

            }
        }
    }
}

```