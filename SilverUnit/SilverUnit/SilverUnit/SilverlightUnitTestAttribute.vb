Imports System.Reflection
Imports TypeMock
Imports TypeMock.Isolator.VisualBasic
Imports System.Windows

<AttributeUsage(AttributeTargets.Method)> _
Public Class SilverlightUnitTestAttribute
    Inherits DecoratorAttribute

    Public Shared wasInit As Boolean = False

    Shared sw As New Stopwatch
    Shared lastTypeName As String


    Public Overrides Function Execute() As Object
        If Not wasInit Then
            wasInit = True
            WriteHeaderToConsole()
            Init()
        End If

        StartListening()

        If Not sw.IsRunning Then
            sw.Reset()
            sw.Start()
        End If

        Me.CallDecoratedMethod()
        sw.Stop()
        StopListening()


        Dim typeName As String = OriginalMethod.DeclaringType.FullName
        If typeName <> lastTypeName Then
            Console.WriteLine()
            Console.WriteLine("-- {0} : ", typeName)
            Console.WriteLine(New String("-", typeName.Length + 5))
            lastTypeName = typeName
        End If

        Console.WriteLine("* {1} ms : {0} ", Me.OriginalMethod.Name, sw.ElapsedMilliseconds)
        Return Nothing
    End Function

    Public Shared Sub StopListening()
        CThruEngine.StopListening()
    End Sub

    Public Shared Sub Init()
        CThruEngine.AddAspectsInAssembly(Assembly.GetExecutingAssembly)

        'the first 'using' is a hack. otherwise the folowing 'using' fails. known bug.
        Using rec = RecorderManager.StartRecording
            VisualStateManager.GoToState(Nothing, "", False)
            rec.Return(True)
        End Using

        Using TheseCalls.WillReturn(True)
            VisualStateManager.GoToState(Nothing, "", False)
        End Using

    End Sub

    Private Function GoToState() As Boolean
        Return True
    End Function

    Public Shared Sub WriteHeaderToConsole()
        Console.WriteLine("******************************************")
        Console.WriteLine("*Loading Silverlight Isolation Aspects...*")
        Console.WriteLine("******************************************")
        Console.WriteLine("")
        Console.WriteLine(" TEST RESULTS: ")
        Console.WriteLine("---------------------------------------------")
    End Sub

    Public Shared Sub StartListening()
        CThruEngine.StartListening()
    End Sub

    Public Shared Sub StartMeasuring()
        sw.Reset()
        sw.Start()
    End Sub
End Class
