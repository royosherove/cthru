Imports System.Windows

Friend Class TemplateChildFaker
    Public Shared Sub reset()
        objectToFakesMap.Clear()
    End Sub
    Private Shared objectToFakesMap As New Dictionary(Of DependencyObject,  _
        Dictionary(Of String, DependencyObject))
    Public Shared Sub SetTemplateChild(ByVal control As DependencyObject, ByVal partName As String, ByVal childObject As DependencyObject)

        If Not objectToFakesMap.ContainsKey(control) Then
            objectToFakesMap.Add(control, New Dictionary(Of String, DependencyObject))
        End If
        objectToFakesMap(control)(partName) = childObject
    End Sub

    Public Shared Function GetPartByName(ByVal control As DependencyObject, ByVal partName As String)
        If Not objectToFakesMap.ContainsKey(control) Then
            Return Nothing
        End If
        If Not objectToFakesMap(control).ContainsKey(partName) Then
            Return Nothing
        End If

        Return objectToFakesMap(control)(partName)
    End Function

End Class