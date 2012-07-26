Imports CThru.BuiltInAspects
Imports CThru

Public Class TracingServicesAspect
    Inherits SkipAllByTypeName

    Public Sub New()
        MyBase.New("TracingServices")
    End Sub

End Class
