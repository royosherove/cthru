Imports System.Windows
Imports System.Windows.Controls
Imports TypeMock.Isolator.VisualBasic

Public Class DependencyObjectRedirects
    Dim propValues As New Dictionary(Of DependencyProperty, Object)
    Private Target As DependencyObject

    Public Sub New(ByVal redirectTarget As DependencyObject)
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
        Dim id = [property].GetKnownId()

        If handler.Method.Name = EventFlag.FLAG_METHOD_NAME Then
            'means we are sending in an order to actually invoke the event
            If id = -1 Then
                If propertyEvents.ContainsKey([property]) Then
                    Dim existing As [Delegate] = propertyEvents([property])
                    '                    Console.WriteLine("INVOKING EVENT {0} WITH {1}", [property].GetName, handler)
                    existing.DynamicInvoke(handler)
                    Return
                End If
            Else
                If propertyEvents.ContainsKey(id) Then
                    Dim existing As [Delegate] = propertyEvents(id)
                    '                    Console.WriteLine("INVOKING EVENT {0} WITH {1}", [property].GetName, handler)
                    existing.DynamicInvoke(EventFlag.ParametersToRaiseOnFakeEvent)
                    Return
                End If

            End If
        End If


        If id = -1 Then
            '            Console.WriteLine("adding dep property {0} WITH {1}", [property].GetName, handler)
            If propertyEvents.ContainsKey([property]) Then
                Dim existing As [Delegate] = propertyEvents([property])
                propertyEvents([property]) = [Delegate].Combine(existing, handler)
            Else
                propertyEvents([property]) = handler
            End If
        Else
            '            Console.WriteLine("adding CORE property {0} WITH {1}", id, handler)
            If propertyEvents.ContainsKey(id) Then
                Dim existing As [Delegate] = propertyEvents(id)
                propertyEvents(id) = [Delegate].Combine(existing, handler)
            Else
                propertyEvents(id) = handler
            End If

        End If

    End Sub


    Friend Sub RemoveEventListener(ByVal [property] As DependencyProperty, ByVal handler As [Delegate])
        Dim id = [property].GetKnownId()
        Dim propertyEvents = EventBucket.Map

        If id - 1 Then
            Dim existing As [Delegate] = propertyEvents([property])
            propertyEvents([property]) = [Delegate].Remove(existing, handler)
        Else
            Dim existing As [Delegate] = propertyEvents(id)
            propertyEvents(id) = [Delegate].Remove(existing, handler)
        End If

    End Sub


#End Region


    Public Function GetTemplateChild(ByVal partName As String) As DependencyObject
        Return TemplateChildFaker.GetPartByName(Target, partName)
    End Function


    Private m_children As UIElementCollection
    Public ReadOnly Property Children() As UIElementCollection
        Get

            If m_children Is Nothing Then
                m_children = FakeInstance(Of UIElementCollection)(Members.CallOriginal)
                RedirectCalls(m_children, New ItemCollectionRedirects(m_children, Target))
            End If

            Return m_children
        End Get
    End Property

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

    '    Dim propertyEvents = New Dictionary(Of DependencyProperty, [Delegate])

End Class

