using System;
using System.Diagnostics;
using System.Reflection;

namespace CThru 
{
	public abstract class Aspect
	{
//        public virtual void InterceptionBehavior(BeforeCallbackEventArgs e)
//        {
//            
//        }

		public virtual void MethodBehavior(DuringCallbackEventArgs e)
        {

		}
		public virtual void MissingMethodBehavior(DuringCallbackEventArgs e)
        {

		}

		public virtual void ConstructorBehavior(DuringCallbackEventArgs e)
        {

		}

		public virtual void StaticConstructorBehavior(DuringCallbackEventArgs e)
        {

		}

		protected void Write (string text, params string[] format)
        {
			return;
			string typeFrom = new StackFrame(1).GetMethod().DeclaringType.Name;
			string theText = string.Format(text, format);
			Console.WriteLine("{0}: {1}", typeFrom, theText);

		}

		public abstract bool ShouldIntercept (InterceptInfo info);

		private bool _active = true;
		public bool Active {
			get { return _active; }
			set { _active = value; }
		}

		public virtual void Start () { Active = true; }
		public virtual void Stop () { Active = false; }

		protected MethodBase GetInterceptedMethod(DuringCallbackEventArgs e) {
			for (int i = 0; i <= 20; i++) {
				MethodBase method = new StackFrame(i).GetMethod();
				if (method.Name == e.MethodName)
					return method;
			}
			return null;
		}

		protected object CallOriginalMethod(DuringCallbackEventArgs e) {
			return this.GetInterceptedMethod(e).Invoke(e.TargetInstance, e.ParameterValues);
		}
	}
}
