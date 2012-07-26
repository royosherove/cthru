'Imports Osherove.Events
'
'Public Class EventsChecker
'    Private verifier As New EventsVerifier
'
'    Public Sub New(ByVal target As Object, ByVal eventName As String)
'        WaitForEvent(eventName, target)
'    End Sub
'
'    Private Sub WaitForEvent(ByVal name As String, ByVal onObject As Object, ByVal ParamArray expectedArgs() As Object)
'        verifier.Expect(onObject, name, expectedArgs)
'    End Sub
'    Public Sub AssertEventFired()
'        verifier.Verify()
'    End Sub
'    Public Sub AssertNotFired()
'
'        If verifier.WasFirstExpectationRaised Then
'            Throw New Exception("Event was raised when not expected")
'        End If
'
'    End Sub
'
'    Public ReadOnly Property PassedInArguments() As Object()
'        Get
'            Return verifier.RaisedEventArgs
'        End Get
'    End Property
'End Class
