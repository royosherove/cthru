using System;

namespace CThru.Tests
{
    public class StubAspect:Aspect
    {
        public event Action<DuringCallbackEventArgs> OnMethodBehaviorCallback=delegate {  };
        public event Action<DuringCallbackEventArgs> OnMissingMethodBehaviorCallback = delegate { };
        public event Action<DuringCallbackEventArgs> OnCTorBehaviorCallback=delegate {  };
        public event Action<DuringCallbackEventArgs> OnStaticCtorBehaviorCallback=delegate {  };

        public override void MethodBehavior(DuringCallbackEventArgs e)
        {
            OnMethodBehaviorCallback(e);
        }
        
        public override void MissingMethodBehavior(DuringCallbackEventArgs e)
        {
            OnMissingMethodBehaviorCallback(e);
        }

        public override void ConstructorBehavior(DuringCallbackEventArgs e)
        {
            OnCTorBehaviorCallback(e);
        }

        public override void StaticConstructorBehavior(DuringCallbackEventArgs e)
        {
            OnStaticCtorBehaviorCallback(e);
        }

		public override bool ShouldIntercept (InterceptInfo info) {
			return info.TypeName.EndsWith("SomeTargetClass") && (info.MethodName.StartsWith("SomeMethod") || info.MethodName.EndsWith("ctor")) || info.MethodName == "MethodThatDoesNotExist";
		}
	}
}