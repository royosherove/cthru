using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CThru.BuiltInAspects
{
	/// <summary>
	/// Breaks into the method if a debugger is attached.
	/// </summary>
	public class DebugAspect:CommonAspect
	{
		public DebugAspect(Predicate<InterceptInfo> shouldIntercept) : base(shouldIntercept) {}

		public override void MethodBehavior(DuringCallbackEventArgs e)
		{
			if (Debugger.IsAttached) Debugger.Break();
			base.MethodBehavior(e);
		}
	}
}
