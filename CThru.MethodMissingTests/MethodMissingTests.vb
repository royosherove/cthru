'Option Strict On
Imports NUnit.Framework
Imports CThru.Tests

<TestFixture()> _
Public Class MethodMissingTests
    Dim GotNotification = False
    Private PropertySetValue As Object = Nothing
    Private IndexerSetValue As Object = Nothing
    Private IndexerID As Object = Nothing

    <SetUp()> _
    Public Sub setup()
        GotNotification = False
        PropertySetValue = Nothing
        IndexerSetValue = Nothing
        IndexerID = Nothing
    End Sub

    <Test()> _
    Public Sub OnMissingMethod_MethodNotFound_IsSkipped()
        Dim stub As New StubAspect()
        AddHandler stub.OnMissingMethodBehaviorCallback, AddressOf MissingMethodHandler

        CThruEngine.AddAspect(stub)
        CThruEngine.StartListening()

        Dim underTest As New TypeUnderTest
        Dim o As Object = underTest
        o.MethodThatDoesNotExist("test")
        RemoveHandler stub.OnMissingMethodBehaviorCallback, AddressOf MissingMethodHandler
        CThruEngine.StopListeningAndReset()

        Assert.IsTrue(GotNotification)
    End Sub

    Private Sub MissingMethodHandler(ByVal e As DuringCallbackEventArgs)

        If e.MethodName = "MethodThatDoesNotExist" And e.ParameterValues(0) = "test" Then
            Console.WriteLine("GOT IT")
            Me.GotNotification = True
            e.MethodBehavior = MethodBehaviors.SkipActualMethod
        End If

    End Sub


    <Test()> _
    Public Sub OnMissingMethod_UsingEventMethodNotFound_IsSkipped()
        AddHandler CThruEngine.MissingMethodCall, AddressOf MissingMethodHandler

        CThruEngine.StartListening()

        Dim o As Object = New TypeUnderTest
        o.MethodThatDoesNotExist("test")

        RemoveHandler CThruEngine.MissingMethodCall, AddressOf MissingMethodHandler
        CThruEngine.StopListeningAndReset()

        Assert.IsTrue(GotNotification)
    End Sub


    <Test()> _
      Public Sub OnMissingMethod_SettingMissingPropertyValue_IsSkipped()
        AddHandler CThruEngine.MissingMethodCall, AddressOf MissingPropertySetHandler

        CThruEngine.StartListening()

        Dim o As Object = New TypeUnderTest
        o.PropertyThatDoesNotExist = 1

        RemoveHandler CThruEngine.MissingMethodCall, AddressOf MissingPropertySetHandler
        CThruEngine.StopListeningAndReset()

        Assert.AreEqual(1, PropertySetValue)
    End Sub
    Private Sub MissingPropertySetHandler(ByVal e As DuringCallbackEventArgs)
        If e.MethodName = "PropertyThatDoesNotExist" Then
            e.MethodBehavior = MethodBehaviors.SkipActualMethod
            PropertySetValue = e.ParameterValues(0)
        End If
    End Sub


    Private Sub MissingIndexerSetHandler(ByVal e As DuringCallbackEventArgs)
        If e.MethodName = "IndexerThatDoesNotExist" Then
            e.MethodBehavior = MethodBehaviors.SkipActualMethod
            IndexerID = e.ParameterValues(0)
            IndexerSetValue = e.ParameterValues(1)
        End If
    End Sub

    <Test()> _
     Public Sub OnMissingMethod_SettingMissingIndexValue_IsSkipped()
        AddHandler CThruEngine.MissingMethodCall, AddressOf MissingIndexerSetHandler

        CThruEngine.StartListening()

        Dim o As Object = New TypeUnderTest
        o.IndexerThatDoesNotExist("a") = 1

        RemoveHandler CThruEngine.MissingMethodCall, AddressOf MissingIndexerSetHandler
        CThruEngine.StopListeningAndReset()

        Assert.AreEqual(1, IndexerSetValue)
        Assert.AreEqual("a", IndexerID)
    End Sub


    <Test()> _
    Public Sub OnMissingMethodWithRetVal_MethodNotFound_ReturnsCustomValue()
        Dim stub As New StubAspect()
        AddHandler stub.OnMissingMethodBehaviorCallback, AddressOf MissingMethodHandlerWithRetVal

        CThruEngine.AddAspect(stub)
        CThruEngine.StartListening()

        Dim underTest As New TypeUnderTest
        Dim o As Object = underTest
        Dim actual As Integer = o.MethodThatDoesNotExist("test")
        RemoveHandler stub.OnMissingMethodBehaviorCallback, AddressOf MissingMethodHandlerWithRetVal
        CThruEngine.StopListeningAndReset()

        Assert.AreEqual(1, actual)
    End Sub

    Private Sub MissingMethodHandlerWithRetVal(ByVal e As DuringCallbackEventArgs)

        If e.MethodName = "MethodThatDoesNotExist" And e.ParameterValues(0) = "test" Then
            Console.WriteLine("GOT IT with ret val")
            e.MethodBehavior = MethodBehaviors.ReturnsCustomValue
            e.ReturnValueOrException = 1
        End If

    End Sub


End Class


Class TypeUnderTest
   

End Class

<TestFixture()> _
Public Class TypeUnderTestWithMissingMethodImpl
    <SetUp()> _
     Public Sub setup()
        wasCalled = False
    End Sub

    Dim wasCalled As Boolean = False
    <Test()> _
    Public Sub OnMissingMethodWithRetVal_MethodNotFoundButMissingMethodImpl_MissingMethodImplCalled()
        CThruEngine.StartListening()
        Dim o As Object = Me
        Dim result As Integer = o.MethodThatDoesNotExist("test")
        CThruEngine.StopListeningAndReset()

        Assert.AreEqual(1, result)
    End Sub


    <Test()> _
    Public Sub OnMissingMethodWithRetVal_MethodNotFoundInCSharpClassWithMissingMethodImpl_MissingMethodImplCalled()
        CThruEngine.StartListening()
        Dim o As Object = New ClassWithMethodMissingImplCSharp
        Dim result As Integer = o.MethodThatDoesNotExist("test")
        CThruEngine.StopListeningAndReset()

        Assert.AreEqual(42, result)
    End Sub


    <Test()> _
        Public Sub OnMissingMethodWithNoReturnValue_MethodNotFoundButMissingMethodImpl_MissingMethodImplCalled()
        CThruEngine.StartListening()
        Dim o As Object = Me

        o.MethodThatDoesNotExist("test")
        CThruEngine.StopListeningAndReset()

        Assert.AreEqual(True, wasCalled)
    End Sub

    Private Function missing_method(ByVal e As MissingMethodArgs) As Object

        If e.MemberName = "MethodThatDoesNotExist" Then
            wasCalled = True
            Return 1
        End If

        If e.MemberName = "MethodWithNoRetVal" Then
            wasCalled = True
        End If

    End Function


End Class



