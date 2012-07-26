Imports System.Windows
Imports Typemock.Isolator.VisualBasic

Public Class SilverUnitAssert
    Public Sub VisualStatedWasChangedTo(ByVal stateName As String, ByVal control As DependencyObject, ByVal useTransitions As Boolean)
        Using AssertCalls.HappenedWithExactArguments
            VisualStateManager.GoToState(control, stateName, useTransitions)
        End Using
    End Sub

End Class