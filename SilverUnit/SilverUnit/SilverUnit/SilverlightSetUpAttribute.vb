
Imports TypeMock

<AttributeUsage(AttributeTargets.Method)> _
Public Class SilverlightSetUpAttribute
    Inherits DecoratorAttribute

    Shared wasInit As Boolean = False
    Public Overrides Function Execute() As Object
        If Not SilverlightUnitTestAttribute.WasInit Then
            SilverlightUnitTestAttribute.wasInit = True
            SilverlightUnitTestAttribute.WriteHeaderToConsole()

            SilverlightUnitTestAttribute.Init()
        End If

        SilverlightUnitTestAttribute.StartMeasuring()
        SilverlightUnitTestAttribute.StartListening()
        Me.CallDecoratedMethod()
        SilverlightUnitTestAttribute.StopListening()
        Return Nothing
    End Function

End Class
