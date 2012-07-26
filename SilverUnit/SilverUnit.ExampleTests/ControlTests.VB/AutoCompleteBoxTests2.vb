Option Strict Off

Imports System.Windows.Controls
Imports NUnit.Framework
Imports Microsoft.Windows.Controls
Imports System.Reflection
Imports CThru.Silverlight
Imports TypeMock.Isolator.VisualBasic

<TestFixture()> _
Public Class AutoCompleteBoxTests2

    <SilverlightUnitTest(), Test()> _
        Public Sub MinimumPrefixLength_SetToLessThanMinusOne_ResetsToMinusOne()
        Dim box As AutoCompleteBox = New AutoCompleteBox
        box.MinimumPrefixLength = -3
        Assert.AreEqual(-1, box.MinimumPrefixLength)
    End Sub

    <SilverlightUnitTest(), Test()> _
   Public Sub MinimumPrefixLength_DefaultIs_1()
        Dim box As AutoCompleteBox = New AutoCompleteBox()
        Assert.AreEqual(1, box.MinimumPrefixLength)
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub SetTextBox_SetTextOnCOmpleteBox_TextBGoxIsUpdatedWithCompleteBox()
        Dim txt As TextBox = New TextBox()
        Dim box As AutoCompleteBox = New AutoCompleteBox()
        txt.SelectionStart = 0
        txt.SelectionLength = 0

        box.TextBox = txt
        box.Text = "a"
        Assert.AreEqual("a", box.Text)
        Assert.AreEqual(box.Text, txt.Text)

    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub InvalidSearchMode()
        Dim box As AutoCompleteBox = New AutoCompleteBox()
        Dim invalid As AutoCompleteSearchMode = 4321
        Try
            box.SetValue(AutoCompleteBox.SearchModeProperty, invalid)
        Catch ex As TargetInvocationException
            Assert.AreEqual(GetType(ArgumentException), ex.InnerException.GetType)
        End Try
    End Sub


    <SilverlightUnitTest(), Test()> _
    Public Sub SetTextBox_SetTextOnTextBox_ACBoxIsUpdated()
        Dim txt As TextBox = New TextBox()
        Dim box As AutoCompleteBox = New AutoCompleteBox()
        txt.SelectionStart = 0
        txt.SelectionLength = 0
        txt.IsEnabled = True
        txt.Focus()

        box.TextBox = txt
        txt.Text = "a"
        txt.FireEvent("TextChanged", Me, FakeInstance(Of TextChangedEventArgs))

        Assert.AreEqual("a", box.Text)
        Assert.AreEqual(box.Text, txt.Text)

    End Sub


    <SilverlightUnitTest(), Test()> _
    Public Sub MaxDropDownHeight_Default_IsDoubleInfinity()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()

        Assert.AreEqual(Double.PositiveInfinity, txt.MaxDropDownHeight)
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub MaxDropDownHeight_ValueSetToLessThanZero_ResetsToOldValue()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        txt.MaxDropDownHeight = 10
        Try
            txt.MaxDropDownHeight = -10
        Catch ex As Exception
            Assert.AreEqual(10, txt.MaxDropDownHeight)
            Return
        End Try
        Assert.Fail("Exception was expected")
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub IsDropDownOpen_Changed_RaisesDropDownOpeningEvent()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        Dim wasRaised = False
        AddHandler txt.DropDownOpening, Sub() wasRaised = True

        txt.IsDropDownOpen = True

        Assert.IsTrue(wasRaised)
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub DropDownOpeningEvent_Canceled_RevertsBackDropDownState()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        AddHandler txt.DropDownOpening, Sub(sender As Object, e As RoutedPropertyChangingEventArgs(Of Boolean))
                                            e.Cancel = True
                                        End Sub

        txt.IsDropDownOpen = True
        Assert.IsFalse(txt.IsDropDownOpen)
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub IsDropDownOpen_Changed_RaisesDropDownOpenedEvent()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        Dim wasRaised = False
        AddHandler txt.DropDownOpened, Sub() wasRaised = True
        txt.IsDropDownOpen = True
        Assert.IsTrue(wasRaised)
    End Sub


    <SilverlightUnitTest(), Test()> _
    Public Sub IsDropDownOpen_ChangedToFalse_RaisesDropDownClosingEvent()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        Dim wasRaised = False
        AddHandler txt.DropDownClosing, Sub() wasRaised = True
        txt.IsDropDownOpen = True

        txt.IsDropDownOpen = False

        Assert.IsTrue(wasRaised)
    End Sub


    <SilverlightUnitTest(), Test()> _
    Public Sub PopulatingEvent_CanBeCanceled()
        Dim wasPopulated = False
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        txt.MinimumPopulateDelay = 1
        txt.ItemsSource = New String() {"a"}

        AddHandler txt.Populating, Sub(sender As Object, e As PopulatingEventArgs) _
                                       e.Cancel = True

        AddHandler txt.Populated, Sub(sender As Object, e As PopulatedEventArgs) _
                                       wasPopulated = True

        txt.SearchMode = AutoCompleteSearchMode.None
        txt.MinimumPrefixLength = 1

        txt.Text = "a"

        Assert.IsFalse(wasPopulated)
    End Sub


    <SilverlightUnitTest(), Test()> _
    Public Sub PopulatingEvent_RaisedAndNotCanceled_RaisesPopulatedEvent()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        txt.MinimumPopulateDelay = 1
        txt.MinimumPrefixLength = 1
        txt.SearchMode = AutoCompleteSearchMode.None
        AddHandler txt.Populating, Sub(sender As Object, e As PopulatingEventArgs) _
                                       e.Cancel = False

        Dim wasPopulated = False
        AddHandler txt.Populated, Sub(sender As Object, e As PopulatedEventArgs) _
                                    wasPopulated = True

        txt.Text = "a"

        Assert.IsTrue(wasPopulated)
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub DropDownClosingEvent_Canceled_RevertsBackDropDownState()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        AddHandler txt.DropDownClosing, Sub(sender As Object, e As RoutedPropertyChangingEventArgs(Of Boolean))
                                            e.Cancel = True
                                        End Sub
        txt.IsDropDownOpen = True

        txt.IsDropDownOpen = False

        Assert.IsTrue(txt.IsDropDownOpen)
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub IsDropDownOpen_ChangedToFalse_RaisesDropDownClosedEvent()
        Dim txt As AutoCompleteBox = New AutoCompleteBox()
        Dim wasRaised = False
        AddHandler txt.DropDownClosed, Sub() wasRaised = True
        txt.IsDropDownOpen = True
        txt.IsDropDownOpen = False
        Assert.IsTrue(wasRaised)
    End Sub


    <SilverlightUnitTest(), Test()> _
    Public Sub PopulatingEvent_UsedToChangeItemSource_Works()
        Dim box As AutoCompleteBox = New AutoCompleteBox()

        AddHandler box.Populating, Sub(sender As Object, e As PopulatingEventArgs)
                                       Dim boxParam As AutoCompleteBox = DirectCast(sender, AutoCompleteBox)
                                       boxParam.ItemsSource = New Object() {"a"}
                                   End Sub

        Dim wasPopulated = False
        AddHandler box.Populated, Sub(sender As Object, e As PopulatedEventArgs)
                                      wasPopulated = True
                                      Assert.AreEqual("a", e.Data.GetEnumerator.Current)
                                  End Sub

        box.SearchMode = AutoCompleteSearchMode.None
        box.Text = "a"

        Assert.IsTrue(wasPopulated)
    End Sub

    Private Sub OnPopulatingChangeSource(ByVal sender As Object, ByVal e As PopulatingEventArgs)
        Dim box As AutoCompleteBox = DirectCast(sender, AutoCompleteBox)
        box.ItemsSource = New Object() {"a"}
    End Sub

    <SilverlightUnitTest(), Test()> _
    Public Sub TestSearchFilters()

        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Contains)("am", "name"))
        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Contains)("AME", "name"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.Contains)("hello", "name"))

        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.ContainsCaseSensitive)("na", "name"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsCaseSensitive)("AME", "name"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsCaseSensitive)("hello", "name"))

        Assert.IsNull(GetFilter(AutoCompleteSearchMode.Custom))
        Assert.IsNull(GetFilter(AutoCompleteSearchMode.None))

        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Equals)("na", "na"))
        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Equals)("na", "NA"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.Equals)("hello", "name"))

        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive)("na", "na"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive)("na", "NA"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive)("hello", "name"))

        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWith)("na", "name"))
        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWith)("NAM", "name"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWith)("hello", "name"))

        Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWithCaseSensitive)("na", "name"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithCaseSensitive)("NAM", "name"))
        Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithCaseSensitive)("hello", "name"))
    End Sub




    <SilverlightUnitTest(), Test()> _
    Public Sub CheckTextCompletion()
        Dim box = New AutoCompleteBox()

        box.TextBox = New TextBox()
        box.TextBox.SelectionLength = 0
        box.TextBox.SelectionStart = 5

        box.IsTextCompletionEnabled = True
        box.ItemsSource = New String() {"hello", "goodbye"}
        box.Text = "hello"

        Assert.IsNotNull(box.SelectedItem)
    End Sub

    Private Function GetFilter(ByVal mode As AutoCompleteSearchMode) As AutoCompleteSearchPredicate(Of String)
        Dim box = New AutoCompleteBox()
        box.SearchMode = mode
        Return box.TextFilter
    End Function

    Private Sub CancelPopulate(ByVal sender As Object, ByVal e As PopulatingEventArgs)
        e.Cancel = True
    End Sub

End Class



