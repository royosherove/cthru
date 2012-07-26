Imports TypeMock.ArrangeActAssert
Imports System.Windows

Public Class SilverUnit
    Private Shared _assert As New SilverUnitAssert


    Public Shared ReadOnly Property Assert() As SilverUnitAssert
        Get
            Return New SilverUnitAssert
        End Get
    End Property

    Public Shared Function MakeEventArgs(Of T As EventArgs)() As T
        Return Isolate.Fake.Instance(Of T)()
    End Function

    Public Shared Sub SetTemplateChild(ByVal control As DependencyObject, ByVal partName As String, ByVal childObject As DependencyObject)
        TemplateChildFaker.SetTemplateChild(control, partName, childObject)
    End Sub

    Public Shared Sub FireEvent(ByVal fromControl As DependencyObject, ByVal selectedEvent As UIEvents, ByVal ParamArray args() As Object)
        fromControl.FireEvent(selectedEvent, args)
    End Sub

    Public Shared Sub FireEvent(ByVal fromControl As DependencyObject, ByVal eventName As String, ByVal ParamArray args() As Object)
        fromControl.FireEvent(eventName, args)
    End Sub

End Class