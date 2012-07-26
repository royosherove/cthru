Public Class EventFlag


    Private Shared nextEventParams As Object()
    Public Shared Property ParametersToRaiseOnFakeEvent() As Object()
        Get
            Return nextEventParams
        End Get
        Set(ByVal value As Object())
            nextEventParams = value
        End Set
    End Property

    Public Shared ReadOnly Property FLAG_METHOD_NAME() As String
        Get
            Return "CthruSilverlightRaiseFakeEvent"
        End Get
    End Property
End Class