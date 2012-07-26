Imports System.Windows
Imports System.Collections.Specialized
Imports System.Reflection
Imports TypeMock.Isolator.VisualBasic

Public Class ItemCollectionRedirects
    Private col As New List(Of Object)

    Dim propValues As New Dictionary(Of DependencyProperty, Object)
    Private Target As DependencyObject

    Public Sub New(ByVal redirectTarget As DependencyObject, collectionHolder As Object)
        Me.Target = redirectTarget
        Me.collectionHolder = collectionHolder
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
        If propertyEvents.ContainsKey([property]) Then
            Dim existing As [Delegate] = propertyEvents([property])
            propertyEvents([property]) = [Delegate].Combine(existing, handler)
        Else
            propertyEvents([property]) = handler
        End If

    End Sub


    Friend Sub RemoveEventListener(ByVal [property] As DependencyProperty, ByVal handler As [Delegate])
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

    Dim propertyEvents = New Dictionary(Of DependencyProperty, [Delegate])
    Private ReadOnly collectionHolder As Object

    Public Sub Add(ByVal value As Object)
        col.Add(value)
    End Sub
    Public Sub Add(ByVal value As UIElement)
        col.Add(value)
    End Sub
    Public Function Remove(ByVal value As Object) As Boolean
        Return col.Remove(value)
    End Function
    Public Function Remove(ByVal value As UIElement) As Boolean
        Return col.Remove(value)
    End Function
    Public Sub Clear()
        col.Clear()
    End Sub
    Public Function Contains(ByVal value As Object) As Boolean
        Return col.Contains(value)
    End Function
    Public Function Contains(ByVal value As UIElement) As Boolean
        Return col.Contains(value)
    End Function
    Public ReadOnly Property Count() As Integer
        Get
            '            Console.WriteLine("getting count")
            Return col.Count
        End Get
    End Property

    Default Public Property Item(ByVal index As Integer) As Object
        Get
            Return col(index)
        End Get
        Set(ByVal value As Object)
            col(index) = value
        End Set
    End Property





End Class