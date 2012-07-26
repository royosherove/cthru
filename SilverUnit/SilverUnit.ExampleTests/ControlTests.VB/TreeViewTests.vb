

Imports Microsoft.Windows.Controls
Imports NUnit.Framework
Imports CThru.Silverlight

<TestFixture()> _
Public Class TreeViewTests

    <SilverlightUnitTest(), Test()> _
    Public Sub ItemsClear_WithOneItem_ClearsItem()
        Dim tv As TreeView = New TreeView
        Dim item As TreeViewItem = New TreeViewItem
        tv.Items.Add(item)
        Assert.AreEqual(1, tv.Items.Count)
        Assert.AreEqual(item, tv.Items(0))
    End Sub

    <SilverlightUnitTest(), Test()> _
        Public Sub SelectedItem_Simple()
        Dim tv As TreeView = New TreeView
        Dim item As TreeViewItem = New TreeViewItem
        tv.Items.Add(item)
    End Sub


End Class
