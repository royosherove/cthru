Imports CThru

Public Class ControlAspect
    Inherits Aspect

    Public Overrides Function ShouldIntercept(ByVal e As InterceptInfo) As Boolean
        If e.TypeName.Contains("Control") AndAlso e.MethodName = "get_IsEnabled" Then
            Return True
        End If
    End Function
    Public Overrides Sub MethodBehavior(ByVal e As DuringCallbackEventArgs)
        'controls are always enabled
        If e.TypeName.Contains("Control") AndAlso e.MethodName = "get_IsEnabled" Then
            e.MethodBehavior = MethodBehaviors.ReturnsCustomValue
            e.ReturnValueOrException = True
        End If
    End Sub

End Class
