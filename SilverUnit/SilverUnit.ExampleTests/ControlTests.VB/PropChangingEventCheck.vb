'Imports Microsoft.Windows.Controls
'
'Public Class PropChangingEventCheck(Of T)
'    Public wasRaised As Boolean
'
'    Public WillCancel As Boolean
'
'    Public Sub EventTarget(ByVal sender As Object, ByVal e As RoutedPropertyChangingEventArgs(Of T))
'        wasRaised = True
'
'        If WillCancel Then
'            e.Cancel = True
'        End If
'
'    End Sub
'End Class