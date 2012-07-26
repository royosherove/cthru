using System;
using System.Collections.Generic;

namespace CThru.BuiltInAspects {
	public class Spy<TResult> : CommonAspect {
		private readonly Func<DuringCallbackEventArgs, TResult> _query;

		private readonly List<TResult> _results = new List<TResult>();
		public IEnumerable<TResult> Results {
			get { return _results; }
		}

		public Spy(Predicate<InterceptInfo> shouldIntercept, Func<DuringCallbackEventArgs, TResult> query) : base(shouldIntercept) {
			_query = query;
		}

		public override void MethodBehavior(DuringCallbackEventArgs e) {
			_results.Add(_query(e));
		}
	}
}