using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CThru
{
	public abstract class CommonAspect:Aspect {
		protected Predicate<InterceptInfo> _shouldIntercept;

		protected CommonAspect(Predicate<InterceptInfo> shouldIntercept) {
			this._shouldIntercept = shouldIntercept;
		}

		public override bool ShouldIntercept (InterceptInfo info) {
			return _shouldIntercept(info);
		}
	}
}
