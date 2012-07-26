using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CThru.BuiltInAspects;

namespace CThru.Tests
{
	/// <summary>
	/// This aspect is for testing the GetInterceptedMethod and CallOriginalMethod methods.
	/// </summary>
	class PassThroughAspect:Stub
	{
		public PassThroughAspect(Predicate<InterceptInfo> shouldIntercept) : base(shouldIntercept) {}

		private MethodInfo _capturedMethodInfo;

		public MethodInfo CapturedMethodInfo {
			get { return _capturedMethodInfo; }
		}

		public override void MethodBehavior(DuringCallbackEventArgs e)
		{
			_capturedMethodInfo = (MethodInfo) this.GetInterceptedMethod(e);
			this.CallOriginalMethod(e);
			base.MethodBehavior(e);
		}
	}
}
