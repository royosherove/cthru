using System;

namespace CThru
{
    public class DuringCallbackEventArgs : EventArgs
    {
        public bool WasBehaviorSet { get; set; }
        private MethodBehaviors methodBehavior;

        public MethodBehaviors
            MethodBehavior
        {
            get { return methodBehavior; }
            set
            {
                methodBehavior = value;
                WasBehaviorSet = true;
            }
        }

        public object TargetInstance { get; set; }
        public object ReturnValueOrException { get; set; }
        public string TypeName { get; set; }
        public string MethodName { get; set; }
        public Type[] GenericParams { get; set; }
        public object[] ParameterValues { get; set; }

        public bool IsDefaultConstructor
        {
            get { return ParameterValues.Length == 0; }
        }

        public void SetExceptionAsReturnValue(Exception exception)
        {
            MethodBehavior = MethodBehaviors.ThrowException;
            ReturnValueOrException = exception;
        }
    }
}