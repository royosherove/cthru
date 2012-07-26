Imports CThru
Imports System.Windows.Threading
Imports TypeMock.Isolator.VisualBasic
Public Class DispatcherTimerAspect
    Inherits Aspect

    Public Overrides Function ShouldIntercept(ByVal info As InterceptInfo) As Boolean
        Return info.MethodName = ".ctor" AndAlso info.TypeName.EndsWith("DispatcherTimer")
    End Function

    Dim dispatcherType As Type = GetType(DispatcherTimer)

    Public Overrides Sub ConstructorBehavior(ByVal e As DuringCallbackEventArgs)
        If e.TargetInstance.GetType.Equals(dispatcherType) Then
            If e.ParameterValues.Length = 0 Then
                Write("redirecting DispatcherTimer")
                RedirectCalls(e.TargetInstance, New DispatcherTimerRedirects(e.TargetInstance))
            Else
                Write("Skipping ctor with more than one argument")
                e.MethodBehavior = MethodBehaviors.SkipActualMethod
            End If

        End If
    End Sub

End Class
