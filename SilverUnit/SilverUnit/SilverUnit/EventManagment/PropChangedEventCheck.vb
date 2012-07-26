'Imports System.Windows
'
'Public Class PropChangedEventCheck(Of T)
'    Public ArgsSent As RoutedPropertyChangedEventArgs(Of T)
'    Public wasRaised As Boolean
'
'    Public WillCancel As Boolean
'
'    Public Sub EventTarget(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of T))
'        wasRaised = True
'        ArgsSent = e
'    End Sub
'End Class