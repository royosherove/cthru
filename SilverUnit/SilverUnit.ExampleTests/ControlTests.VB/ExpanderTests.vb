Imports Microsoft.Windows.Controls
Imports NUnit.Framework
Imports CThru.Silverlight
Imports System.Windows
Imports TypeMock.ArrangeActAssert

<TestFixture()> _
Public Class ExpanderTests
    <SetUp()> _
    Public Sub setup()
        wasRaised = False
    End Sub
    Dim wasRaised = False

    <SilverlightUnitTest(), Test()> _
    Public Sub ExpandDirection_SetToInvalidValue_ThrowsAndReversBackValue()
        Dim ex As New Expander
        ex.ExpandDirection = ExpandDirection.Down
        Try
            ex.SetValue(Expander.ExpandDirectionProperty, 130)
        Catch e As Exception
            Assert.AreEqual(ExpandDirection.Down, ex.ExpandDirection)
            Return
        End Try
        Assert.Fail("exception was expected")
    End Sub


    <SilverlightUnitTest(), Test()> _
       Public Sub IsExpanded_SetTrhe_RaisesExpandedEvent()
        Dim ex As New Expander
        AddHandler ex.Expanded, AddressOf OnExpand
        ex.IsExpanded = True

        Assert.IsTrue(wasRaised)
    End Sub


    <SilverlightUnitTest(), Test()> _
       Public Sub IsExpanded_SetFalse_RaisesCollapsedEvent()
        Dim ex As New Expander
        AddHandler ex.Collapsed, AddressOf OnExpand
        ex.IsExpanded = False

        Assert.IsTrue(wasRaised)
    End Sub


    Private Sub OnExpand(ByVal sender As Object, ByVal e As RoutedEventArgs)
        wasRaised = True
    End Sub
End Class
