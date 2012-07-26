using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CThru.BuiltInAspects
{
	public class TraceResultAspect:TraceAspect
	{
		public TraceResultAspect(Predicate<InterceptInfo> shouldIntercept, TextWriter writer) : base(shouldIntercept, writer) {}
		public TraceResultAspect(Predicate<InterceptInfo> shouldIntercept) : base(shouldIntercept) {}

		public override void MethodBehavior(DuringCallbackEventArgs e)
		{
			this.Result = this.CallOriginalMethod(e);
			base.MethodBehavior(e);
			e.MethodBehavior = MethodBehaviors.ReturnsCustomValue;
			e.ReturnValueOrException = this.Result;
		}

		protected override string GetMessage(DuringCallbackEventArgs e)
		{
			return base.GetMessage(e) + GetResultMessage();
		}

		protected virtual string GetResultMessage() {
			return string.Format(" returned \"{0}\"", this.Result);
		}

		public object Result { get; private set; }
	}
}
