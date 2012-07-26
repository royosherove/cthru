using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CThru.BuiltInAspects
{
	public class Stub: CommonAspect {
		private MethodBehaviors _currentBehavior = MethodBehaviors.SkipActualMethod;
		private Func<DuringCallbackEventArgs, object> _returnAction; 
		public Stub(Predicate<InterceptInfo> shouldIntercept) : base(shouldIntercept) {}

		public override void MethodBehavior(DuringCallbackEventArgs e)
		{
			e.MethodBehavior = _currentBehavior;
			if (_returnAction != null) {
				e.ReturnValueOrException = _returnAction(e);
			}
			//base.MethodBehavior(e);
		}

		public Stub Return(object returnValue) {
			return Return(e => returnValue);
		}

		public Stub Return(Func<DuringCallbackEventArgs, object> returnAction) {
			_currentBehavior = MethodBehaviors.ReturnsCustomValue;
			_returnAction = returnAction;
			return this;
		}

		public static Stub For<TStubbedClass>(string methodName = null) {
			return new Stub(info => {
			                        	return (info.TargetInstance == null && info.TypeName == typeof (TStubbedClass).FullName || info.TargetInstance is TStubbedClass) && (methodName == null || info.MethodName == methodName); });
		}
	}
}
