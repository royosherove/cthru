Imports TypeMock.Isolator.VisualBasic
Imports System.Windows

''' <summary>
''' This fake timer will fire only once
''' </summary>
''' <remarks></remarks>
Public Class DispatcherTimerRedirects
'    Private enabled As Boolean
    Private events As New List(Of EventHandler)()
    Public Custom Event Tick As EventHandler
        AddHandler(ByVal value As EventHandler)
'                        Console.WriteLine("Added Timer handler")
            events.Add(value)
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            events.Remove(value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)

        End RaiseEvent
    End Event


    Public Sub Start()
        '        Console.WriteLine("Timer started")
        m_isEnabled = True
        For Each ev As EventHandler In events
            ev(Me, New EventArgs)
        Next
    End Sub

    Public Sub [Stop]()
        '        Console.WriteLine("Timer Stopped")
        m_isEnabled = False
    End Sub



#Region "Dependency object - until we can get inherited redirects"
    Dim propValues As New Dictionary(Of DependencyProperty, Object)
    Private Target As Object

    Public Sub New(ByVal redirectTarget As Object)
        Me.Target = redirectTarget
    End Sub

#Region "   Redirected methods"


    ''' <summary>
    ''' This SetValue behavior is based on the description provided here:
    ''' http://blog.ningzhang.org/2008/11/dependencyproperty-validation-coercion_10.html
    ''' yes. It is totally open to recursive failures and allows invalid values.
    ''' </summary>
    ''' <param name="dp"></param>
    ''' <param name="value"></param>
    ''' <remarks></remarks>
    Friend Sub SetValue(ByVal dp As DependencyProperty, ByVal value As Object)
        '        Console.WriteLine("Setting value for {0} {1}", dp.GetName, value)

        Dim oldValue As Object = GetValue(dp)
        propValues(dp) = value
        TriggerPropertyCallback(dp, value, oldValue)
    End Sub


    Friend Function GetValue(ByVal dp As DependencyProperty) As Object

        'if dp is null it means that we should have invoked some static ctors down the tree of DependencyObject and failed to do it.
        If propValues.ContainsKey(dp) Then
            Return propValues(dp)
        End If


        Return dp.GetDefaultPropertyValue
    End Function

    Private m_isEnabled As Boolean
    Public Property IsEnabled() As Boolean
        Get
            Return m_isEnabled
        End Get
        Set(ByVal value As Boolean)
            m_isEnabled = value
        End Set
    End Property


    Friend Sub AddEventListener(ByVal [property] As DependencyProperty, ByVal handler As [Delegate])
        Dim propertyEvents = EventBucket.Map
        If propertyEvents.ContainsKey([property]) Then
            Dim existing As [Delegate] = propertyEvents([property])
            propertyEvents([property]) = [Delegate].Combine(existing, handler)
        Else
            propertyEvents([property]) = handler
        End If

    End Sub


    Friend Sub RemoveEventListener(ByVal [property] As DependencyProperty, ByVal handler As [Delegate])
        Dim propertyEvents = EventBucket.Map
        Dim existing As [Delegate] = propertyEvents([property])
        propertyEvents([property]) = [Delegate].Remove(existing, handler)
    End Sub


#End Region


    Dim CurrentArgValues As ArgValuesHolder
    Private Sub TriggerPropertyCallback(ByVal dp As DependencyProperty, ByVal newValue As Object, ByVal oldValue As Object)
        Dim args As DependencyPropertyChangedEventArgs

        'we can only redirect structs once using isolator, 
        'once redirected, we just need to change the values that it redirects to.
        If CurrentArgValues Is Nothing Then
            CurrentArgValues = New ArgValuesHolder(oldValue, newValue, dp)
        Else
            CurrentArgValues.SetValues(oldValue, newValue, dp)
        End If

        Try
            args = New DependencyPropertyChangedEventArgs()
            RedirectCalls(args, CurrentArgValues)
        Catch
        End Try
        Dim callback As PropertyChangedCallback = dp.GetCallback

        If Not callback Is Nothing Then
            callback.Invoke(Target, args)
        End If

    End Sub


#End Region


End Class

