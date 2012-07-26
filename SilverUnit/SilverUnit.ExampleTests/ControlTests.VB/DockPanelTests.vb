Imports Microsoft.Windows.Controls
Imports NUnit.Framework
Imports CThru.Silverlight

<TestFixture()> _
Public Class DockPanelTests

    <SilverlightUnitTest(), Test()> _
    Public Sub Dock_SetToInvalidValue_ThrowsAndReversBackValue()
        Dim dp As New DockPanel()
        DockPanel.SetDock(dp, Dock.Bottom)
        Try
            dp.SetValue(DockPanel.DockProperty, 130)
        Catch ex As Exception
            Assert.AreEqual(Dock.Bottom, DockPanel.GetDock(dp))
            Return
        End Try
        Assert.Fail("exception was expected")
    End Sub



End Class
