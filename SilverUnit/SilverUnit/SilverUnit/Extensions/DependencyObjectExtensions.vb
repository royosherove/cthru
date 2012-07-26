Imports System.Windows
Imports System.Reflection

Public Enum UIEvents
    GotFocus
    KeyDown
    KeyUp
    LostFocus
    LostMouseCapture
    MouseEnter
    MouseLeave
    MouseLeftButtonDown
    MouseLeftButtonUp
    MouseMove
End Enum

Public Module DependencyObjectExtensions


    <System.Runtime.CompilerServices.Extension()> _
    Public Sub FireEvent(ByVal depo As DependencyObject, ByVal selectedEvent As UIEvents, ByVal ParamArray args() As Object)

        EventFlag.ParametersToRaiseOnFakeEvent = args
        Dim name = [Enum].GetName(GetType(UIEvents), selectedEvent).ToString
        Dim method As MethodInfo = depo.GetType.GetEvent(name).GetAddMethod
        Dim eventParams As Object() = CreateParametersForEventSignature(method)
        method.Invoke(depo, eventParams)
    End Sub

    <System.Runtime.CompilerServices.Extension()> _
    Public Sub FireEvent(ByVal depo As DependencyObject, ByVal eventName As String, ByVal ParamArray args() As Object)

        EventFlag.ParametersToRaiseOnFakeEvent = args

        Dim method As MethodInfo = depo.GetType.GetEvent(eventName).GetAddMethod
        Dim eventParams As Object() = CreateParametersForEventSignature(method)
        method.Invoke(depo, eventParams)
    End Sub

    Private Function CreateParametersForEventSignature(ByVal method As MethodInfo) As Object()
        Dim eventParams As New List(Of Object)
        Debug.Assert("CthruSilverlightRaiseFakeEvent" = EventFlag.FLAG_METHOD_NAME)

        Dim parameters As ParameterInfo() = method.GetParameters()
        For Each param As ParameterInfo In parameters
            If param.ParameterType.IsSubclassOf(GetType([Delegate])) Then
                Dim delegateToSend = CreateDelegateMatchingEventSignature(param)
                eventParams.Add(delegateToSend)
            Else
                eventParams.Add(Nothing) 'send in an empty parameter
            End If
        Next
        Return eventParams.ToArray
    End Function

    Private Function CreateDelegateMatchingEventSignature(ByVal param As ParameterInfo) As [Delegate]

        Dim specialMethodTarget As MethodInfo = GetType(DependencyObjectExtensions).GetMethod(EventFlag.FLAG_METHOD_NAME)
        Return [Delegate].CreateDelegate(param.ParameterType, specialMethodTarget)
    End Function

    Public Sub CthruSilverlightRaiseFakeEvent(ByVal o1 As Object, ByVal o2 As Object)
        'this is a special method with a special name.
        ' the name is looked for in the DependencyObjectRedirects.vb class
        ' in the "AddEventHandler" override we provide, we will check if this is the name of the event's target method.
        ' if that is indeed the name, instead of adding it to the event subscriber list,
        ' we will instead *invoke* the event and trigger all the subscribers.
        'the reason we do it this way, is because we need to know :
        ' - on which DependencyProperty is this event associated with
        ' - get the event that matches that property
        ' -trigger it's invocation list one by one
        '
        'since the DependencyProperty is sent in during the event_add method call, 
        ' this is the only "simple" way we have of getting it.
    End Sub
End Module