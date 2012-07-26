Imports System.Windows
Imports System.Reflection

Public Module DependencyPropertyChangedEventArgsExtensions
    <System.Runtime.CompilerServices.Extension()> _
    Public Function SetValues(ByRef args As DependencyPropertyChangedEventArgs, ByVal oldValue As Object, ByVal newValue As Object, ByVal dp As DependencyProperty)
        args.SetPrivateField("_oldValue", oldValue)
        args.SetPrivateField("_newValue", newValue)
        args.SetPrivateField("_property", dp)
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Sub SetPrivateField(ByRef args As DependencyPropertyChangedEventArgs, ByVal name As String, ByVal value As Object)
        Dim field As FieldInfo = args.GetType.GetField(name, BindingFlags.NonPublic Or BindingFlags.Instance) '
        field.SetValue(args, value)
    End Sub

End Module
