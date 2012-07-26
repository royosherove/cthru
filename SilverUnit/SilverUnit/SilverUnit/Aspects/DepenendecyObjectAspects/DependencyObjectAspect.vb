Imports CThru
Imports System.Windows
Imports TypeMock.Isolator.VisualBasic
Public Class DependencyObjectAspect
    Inherits Aspect
    Public Overrides Function ShouldIntercept(ByVal e As InterceptInfo) As Boolean
        If e.MethodName = ".ctor" Or e.MethodName = ".cctor" Then
            Return True
        End If

        If e.MethodName.StartsWith("add_") Then
            Return True
        End If

    End Function


    Public Overrides Sub StaticConstructorBehavior(ByVal e As CThru.DuringCallbackEventArgs)
        If e.TypeName.Contains("DependencyObject") Then
            Write("Skipping CCTOR")
            e.MethodBehavior = MethodBehaviors.SkipActualMethod
        End If
    End Sub

    Dim depObjType As Type = GetType(DependencyObject)
    Dim dispatcherType As Type = GetType(DispatcherTimerAspect)

 
    Public Overrides Sub ConstructorBehavior(ByVal e As CThru.DuringCallbackEventArgs)

        Dim targetType As Type = e.TargetInstance.GetType

        'disable only the direct DependencyObject type constructor but leave other constructors running fine
        If targetType.Equals(depObjType) Then
            Write("Skipping Depenendecy object ctor")
            e.MethodBehavior = MethodBehaviors.SkipActualMethod
            Return
        End If

        If targetType.IsSubclassOf(depObjType) AndAlso Not targetType.Equals(dispatcherType) Then
            If e.IsDefaultConstructor Then 'default ctor
                RedirectCalls(e.TargetInstance, New DependencyObjectRedirects(e.TargetInstance))
                Write("Redirecting DepObj: {0}", e.TypeName)
            End If


            If Not e.IsDefaultConstructor Then
                Write("Skipping ctor with more than one parameter")
                e.MethodBehavior = MethodBehaviors.SkipActualMethod
            End If
        End If





    End Sub
End Class
