Imports System.Reflection

Public Class ReflectCache

    Private Shared emptyCtorMap As New Dictionary(Of Type, ConstructorInfo)
    Private Shared typeDeclaredMethodsMap As New Dictionary(Of Type, MethodInfo())
    Private Shared typeInstanceMethodsMap As New Dictionary(Of Type, MethodInfo())
    Private Shared assemblyTypesMap As New Dictionary(Of Assembly, Type())
    Private Shared typePropertiesMap As New Dictionary(Of Type, PropertyInfo())

    Public Shared Function GetEmptyCtorForType(Of T)(ByVal control As T) As ConstructorInfo
        Return GetAndCacheGenericResult(Of Type, ConstructorInfo) _
                                                    (control.GetType, _
                                                     emptyCtorMap, _
                                                     AddressOf GetEmptyCtorForTypeUnCached)
    End Function

    Public Shared Function GetTypeDeclaredMethods(Of T)(ByVal control As T) As MethodInfo()

        Return GetAndCacheGenericResult(Of Type, MethodInfo()) _
                                                            (control.GetType, _
                                                             typeDeclaredMethodsMap, _
                                                             AddressOf GetTypeDeclaredMethodsUnCached)

    End Function
    Public Shared Function GetTypeAllInstanceMethods(ByVal control As Object) As MethodInfo()

        Return GetAndCacheGenericResult(Of Type, MethodInfo()) _
                                                    (control.GetType, _
                                                     typeInstanceMethodsMap, _
                                                     AddressOf GetTypeAllInstanceMethodsUnCached)

    End Function

    Public Shared Function GetAndCacheGenericResult(Of KEY, VALUE)(ByVal theKey As KEY, _
                                                                 ByVal map As Dictionary(Of KEY, VALUE), _
                                                                 ByVal InitialFetcher As Func(Of KEY, VALUE)) As VALUE

        Dim result As VALUE

        If map.ContainsKey(theKey) Then
            result = map(theKey)
            Return result
        Else
            result = InitialFetcher(theKey)
            map(theKey) = result
            Return result
        End If

    End Function
    Public Shared Function GetNonPublicResourcePropertiesForType(ByVal theType As Type) As PropertyInfo()
        Return GetAndCacheGenericResult(Of Type, PropertyInfo())(theType, _
                                                                 typePropertiesMap, _
                                                                 AddressOf GetNonPublicResourcePropertiesForTypeUnCached)
    End Function

    Public Shared Function GetAllTypesInAssembly(ByVal asm As Assembly) As Type()
        Return GetAndCacheGenericResult(Of Assembly, Type()) _
                                                    (asm, _
                                                     assemblyTypesMap, _
                                                     AddressOf GetAllTypesInAssemblyUnCached)
    End Function

    Private Shared Function GetTypeDeclaredMethodsUnCached(ByVal arg As Type) As MethodInfo()
        Dim flags = BindingFlags.NonPublic Or _
                BindingFlags.Public Or _
                BindingFlags.Static Or _
                BindingFlags.Instance Or _
                BindingFlags.DeclaredOnly

        Dim methods = arg.GetMethods(flags)
        Return methods
    End Function

    Private Shared Function GetTypeAllInstanceMethodsUnCached(ByVal arg As Type) As MethodInfo()
        Dim methods = arg.GetMethods(BindingFlags.NonPublic Or _
                                     BindingFlags.Public Or _
                                     BindingFlags.Instance)
        Return methods

    End Function

    Private Shared Function GetAllTypesInAssemblyUnCached(ByVal arg As Assembly) As Type()
        Return arg.GetTypes
    End Function

    Private Shared Function GetEmptyCtorForTypeUnCached(ByVal theType As Type) As ConstructorInfo
        Dim ctor = theType.GetConstructor(Type.EmptyTypes)
        Return ctor
    End Function


    Private Shared Function GetNonPublicResourcePropertiesForTypeUnCached(ByVal arg As Type) As PropertyInfo()
        Dim properties As PropertyInfo() = arg.GetProperties(BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.DeclaredOnly)
        Return properties
    End Function
End Class
