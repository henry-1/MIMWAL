
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