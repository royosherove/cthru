using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CThru.BuiltInAspects {
	/// <summary>
	/// This aspect can be used for tracing calls to specified methods, including a part of the stack trace.
	/// </summary>
	/// <example>var aspect = new TraceAspect(info => info.TargetInstance is IHttpHandler, @"C:\log.txt");</example>
	public class TraceAspect : CommonAspect {
		private readonly TextWriter _writer;
		private readonly int _depth;

		public TraceAspect(Predicate<InterceptInfo> shouldIntercept, TextWriter writer) : base(shouldIntercept) {
			_writer = writer;
		}

		public TraceAspect(Predicate<InterceptInfo> shouldIntercept, string logPath) : this(shouldIntercept) {
			_writer = new StreamWriter(logPath);
		}

		public TraceAspect (Predicate<InterceptInfo> shouldIntercept) : this(shouldIntercept, Console.Out) {}

		public TraceAspect(Predicate<InterceptInfo> shouldIntercept, int depth) : this(shouldIntercept) {
			_depth = depth;
		}

		public TraceAspect(Predicate<InterceptInfo> shouldIntercept, TextWriter writer, int depth) : base(shouldIntercept) {
			_writer = writer;
			_depth = depth;
		}

		public override void ConstructorBehavior (DuringCallbackEventArgs e) {
			DoTrace(e);
		}

		public override void MethodBehavior (DuringCallbackEventArgs e) {
			DoTrace(e);
		}

		protected virtual void DoTrace (DuringCallbackEventArgs e) {
			string message = GetMessage(e);
			DoTrace(message);
		}

		protected virtual void DoTrace(string message) {
			if (_writer != null) _writer.WriteLine(message);
		}

		protected virtual string GetMessage(DuringCallbackEventArgs e) {
			string message = string.Empty;
			try {
				string parameters = GetParameters(e);
				message = string.Concat(e.TypeName, ".", e.MethodName, "(", parameters, ")");
				for (int i = 11; i <= 11 + _depth - 1; i++) {
					var frame = new StackFrame(i);
					var method = frame.GetMethod();
					if (method != null) {
						var type = (method.DeclaringType != null ? method.DeclaringType.FullName : "(?)"); // weird, but sometimes method.DeclaringType *is* null (method.Name is "lambda_method")
						message += Environment.NewLine + "--> " + type + "." +  method.Name;
					}
				}
			}
			catch (Exception ex) {
				message += Environment.NewLine + ex.ToString() + Environment.NewLine;
			}
			return message;
		}

		protected virtual string GetParameters (DuringCallbackEventArgs e) {
			var list = new List<string>();
			if (e.ParameterValues != null) {
				foreach (var param in e.ParameterValues) {
					var value = param != null ? param.ToString() : "null";
					list.Add(value);
				}
			}
			return string.Join(", ", list.ToArray());
		}
	}
}
