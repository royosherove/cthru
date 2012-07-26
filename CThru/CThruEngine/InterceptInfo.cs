using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CThru {
	/// <summary>
	/// This class encapsulates the data that is used to make a decision whether we should intercept this call. inter
	/// </summary>
	public class InterceptInfo {
		public object TargetInstance { get; set; }
		public string TypeName { get; set; }
		public string MethodName { get; set; }
		public Type[] GenericParams { get; set; }

		public InterceptInfo () { }

		public InterceptInfo (DuringCallbackEventArgs e){
			this.TargetInstance = e.TargetInstance;
			this.TypeName = e.TypeName;
			this.MethodName = e.MethodName;
			this.GenericParams = e.GenericParams;
		}

		public InterceptInfo (TypeMock.Internal.Hooks.HookEventArgsBase e){
			this.TargetInstance = e.Context;
			this.TypeName = e.TypeName;
			this.MethodName = e.MethodName;
			var parameters = e.MethodGenericParameters as TypeMock.TypeParams;
			if (parameters != null) this.GenericParams = parameters.GetTypes();
		}
	}
}
