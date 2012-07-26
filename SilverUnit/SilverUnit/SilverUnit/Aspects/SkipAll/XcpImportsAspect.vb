Imports CThru.BuiltInAspects
Imports CThru

Public Class XcpImportsAspect
    Inherits SkipAllByTypeName

    Public Sub New()
        MyBase.New("XcpImports")
    End Sub


    Public Overrides Sub MethodBehavior(ByVal e As DuringCallbackEventArgs)
        MyBase.MethodBehavior(e)

        If e.MethodName = "Control_Focus" Then
            e.ReturnValueOrException = True
            e.MethodBehavior = MethodBehaviors.ReturnsCustomValue
        End If

    End Sub

End Class
