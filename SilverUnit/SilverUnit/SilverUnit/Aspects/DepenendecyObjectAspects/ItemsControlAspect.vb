Imports System.Windows.Controls
Imports CThru
Imports Typemock.Isolator.VisualBasic
Public Class ItemsControlAspect
    Inherits Aspect
    Public Overrides Function ShouldIntercept(ByVal info As InterceptInfo) As Boolean
        Return info.MethodName = ".ctor"
    End Function

    Shared map As New Hashtable()
    Public Overrides Sub ConstructorBehavior(ByVal e As CThru.DuringCallbackEventArgs)

        Dim itemsType As Type = GetType(ItemsControl)
        If e.TargetInstance.GetType.IsSubclassOf(itemsType) Then
            make_Items_propertyReturnAWorkingCollection(e)
        End If


    End Sub

    Private Sub make_Items_propertyReturnAWorkingCollection(ByVal e As DuringCallbackEventArgs)

        'make sure you only redirect once on the same object instance
        Dim code As Integer = e.TargetInstance.GetHashCode
        If map.ContainsKey(code) Then
            Return
        Else
            map(code) = True
        End If
        Write("Redirecting collection items on {0}", e.TypeName)
        Dim col = FakeInstance(Of ItemCollection)(Members.CallOriginal)
        Dim target As ItemCollectionRedirects = New ItemCollectionRedirects(col, e.TargetInstance)

        RedirectCalls(col, target)
        NonPublicWillReturn(e.TargetInstance, "get_Items", col)
    End Sub
End Class
