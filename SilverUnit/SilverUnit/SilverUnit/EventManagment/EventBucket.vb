Imports System.Windows

Public Class EventBucket
    Shared propertyEvents = New Dictionary(Of Object, [Delegate])

    Public Shared ReadOnly Property Map() As Dictionary(Of Object, [Delegate])
        Get
            Return propertyEvents
        End Get
    End Property
End Class
