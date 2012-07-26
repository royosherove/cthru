Imports System.Windows
Imports Typemock.Isolator.VisualBasic

Public Class RoutedEventArgsRedirects

    Private Shared UniqueID As Integer = 0
    Private InstanceID As Integer
    Private Shared valueMap As New Dictionary(Of Integer, Object)
    Public Sub New()
        UniqueID += 1
        Me.InstanceID = UniqueID
        valueMap(Me.InstanceID) = Nothing
    End Sub

    Public Property OriginalSource() As Object
        Get
            Return valueMap(Me.InstanceID)
        End Get
        Set(ByVal value As Object)
            valueMap(Me.InstanceID) = value
        End Set
    End Property

End Class