Imports System.Windows

Public Class ArgValuesHolder

    Private mOldValue As Object
    Public ReadOnly Property OldValue() As Object
        Get
            Return mOldValue
        End Get
    End Property


    Private mNewValue As Object
    Public ReadOnly Property NewValue() As Object
        Get
            Return mNewValue
        End Get
    End Property

    Public Sub New(ByVal mOldValue As Object, ByVal mNewValue As Object, ByVal mProperty As DependencyProperty)
        Me.mOldValue = mOldValue
        Me.mNewValue = mNewValue
        Me.mProperty = mProperty
    End Sub


    Public Sub SetValues(ByVal mOldValue As Object, ByVal mNewValue As Object, ByVal mProperty As DependencyProperty)
        Me.mOldValue = mOldValue
        Me.mNewValue = mNewValue
        Me.mProperty = mProperty
    End Sub

    Private mProperty As DependencyProperty
    Public ReadOnly Property [Property]() As DependencyProperty
        Get
            Return mProperty
        End Get
    End Property

End Class