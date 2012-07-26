Imports System.Windows
Imports System.Reflection

Public Module DependencyPropertyExtensions
    Private cache As New Dictionary(Of DependencyProperty, Dictionary(Of String, Object))

    <System.Runtime.CompilerServices.Extension()> _
    Public Function GetPrivateFieldValue(ByVal dp As DependencyProperty, ByVal name As String)
        Return GetPrivateFieldValue(dp, name, False)
    End Function
    Public Function GetPrivateFieldValue(ByVal dp As DependencyProperty, ByVal name As String, ByVal ignoreCache As Boolean)

        If Not ignoreCache AndAlso cache.ContainsKey(dp) Then
            Dim lookup = cache(dp)

            If lookup.ContainsKey(name) Then
                Return lookup(name)
            End If
        End If

        Dim field As FieldInfo = _
            dp.GetType.GetField(name, BindingFlags.NonPublic Or BindingFlags.Instance)


        Dim map = New Dictionary(Of String, Object)
        If field Is Nothing Then
            map(name) = Nothing
            cache(dp) = map
            Return Nothing
        Else
            Dim value As Object = field.GetValue(dp)
            map(name) = value
            If Not ignoreCache Then
                cache(dp) = map
            End If
            Return value
            End If

    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function GetDefaultPropertyValue(ByVal dp As DependencyProperty) As Object
        Return dp.GetPrivateFieldValue("_defaultValue")
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function GetCallback(ByVal dp As DependencyProperty) As PropertyChangedCallback
        Return dp.GetPrivateFieldValue("_propertyChangedCallback")
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function GetName(ByVal dp As DependencyProperty) As String
        Return dp.GetPrivateFieldValue("_name")
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function GetKnownId(ByVal dp As DependencyProperty) As Integer
        If dp.GetType.Name.Contains("CoreDependencyProperty") Then
            Return GetPrivateFieldValue(dp, "m_nKnownId", True)
        End If
        Return -1
    End Function

End Module
