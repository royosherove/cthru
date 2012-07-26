using System;
using System.Collections.Generic;
using System.Reflection;
using CThru.BuiltInAspects;
using TypeMock;
using TypeMock.Internal.Hooks;
using System.Linq;

namespace CThru {
	public enum InterceptionOptions {
		ShouldIntercept,
		ShouldNotIntercept,
		IDontCare
	}

	public enum MethodBehaviors {
		SkipActualMethod,
		CallRealMethod,
		ReturnsCustomValue,
		ThrowException,
		IDontCare
	}


	public class CThruEngine {
		private static readonly Dictionary<String, Aspect> aspects = new Dictionary<String, Aspect>();
		private static bool interceptUsingEvents = false; //set it to true if we intercept using events rather than aspects.
		private static object AspecsSync = new object();
		private static event Action<DuringCallbackEventArgs> DuringConstructorCall;
		private static event Action<DuringCallbackEventArgs> DuringStaticConstructorCall;
		private static event Action<DuringCallbackEventArgs> duringMethodCall;
		public static event Action<DuringCallbackEventArgs> DuringMethodCall {
			add {
				duringMethodCall += value;
				interceptUsingEvents = true;
			}
			remove {
				duringMethodCall -= value;
			}
		}
		private static event Action<DuringCallbackEventArgs> missingMethodCall;
		public static event Action<DuringCallbackEventArgs> MissingMethodCall {
			add {
				missingMethodCall += value;
				interceptUsingEvents = true;
			}
			remove {
				missingMethodCall -= value;
			}
		}


		public static bool EventsAreClear {
			get {
				int onMethod = 0; int onStaticCtor = 0; int onCtor = 0; int onMissing = 0;
				if (duringMethodCall != null) onMethod = duringMethodCall.GetInvocationList().Length;
				if (DuringStaticConstructorCall != null) onStaticCtor = DuringStaticConstructorCall.GetInvocationList().Length;
				if (DuringConstructorCall != null) onCtor = DuringConstructorCall.GetInvocationList().Length;
				if (missingMethodCall != null) onMissing = missingMethodCall.GetInvocationList().Length;
				return (onCtor + onStaticCtor + onMethod + onMissing == 0);
			}
		}

		public static bool Verbose { get; set; }

	    private static bool addedInternalAspects = false;
		public static void StartListening () {

			if(!MockManager.IsInitialized) // init only if needed
				MockManager.Init();
			EventTunnel.ShouldInterceptDecision = CallbackBeforeMethodCall;
			EventTunnel.DuringMethodCall = CallBackDuringMethodCall;

		    if (!addedInternalAspects)
		    {
		        addedInternalAspects = true;
		        AddAspect(new MissingMethodAspect());
		    }
//            AddAspect(new TraceAspect(delegate(InterceptInfo info) {
//                                                                       Console.WriteLine("{0}.{1}", info.TypeName,
//                                                                                         info.MethodName);
//                                                                       return true; },null
//                                                                       ));
		}

		public static void AddAspectsInAssembly (Assembly asm) {
			if (Verbose) {
				Console.WriteLine("Loading Isolation Aspects from assembly: {0}", asm.GetName().FullName);
			}
			foreach (var type in asm.GetTypes()) {
				Type aspectType = typeof(Aspect);
				if (type.IsSubclassOf(aspectType)) {
					AddAspect((Aspect)Activator.CreateInstance(type));
				}
			}
		}

		public static void AddAspect (Aspect newAspect) {
			var key = GetUniqueKey(newAspect.GetType().Name);
			AddAspect(key, newAspect);
		}

		public static void AddAspect(string key, Aspect newAspect) {
			if (Verbose) {
				Console.WriteLine("\t* Loading {0}..", key);
			}
			if (aspects.ContainsKey(key)) aspects[key].Stop();
			aspects[key] = newAspect;

			var handler = new AspectHandler(newAspect);
			DuringConstructorCall +=
				(handler.HandleConstructor);
			DuringStaticConstructorCall +=
				(handler.HandleStaticConstructor);
			duringMethodCall += (handler.HandleMethod);
			missingMethodCall += (handler.HandleMissingMethod);   //NOTE: it is important that we use missingMethodCall and not MissingMethodCall with a capital "M" here
		}

		/// <summary>
		/// 
		/// </summary>
		internal class AspectHandler {
			private readonly Aspect _aspect;
			internal AspectHandler(Aspect aspect) {
				_aspect = aspect;
			}

			public Aspect Aspect {
				get { return _aspect; }
			} 

			internal void HandleConstructor (DuringCallbackEventArgs e) {
				Handle(e, _aspect.ConstructorBehavior);
			}

			internal void HandleStaticConstructor (DuringCallbackEventArgs e) {
				Handle(e, _aspect.StaticConstructorBehavior);
			}

			internal void HandleMethod (DuringCallbackEventArgs e) {
				Handle(e, _aspect.MethodBehavior);
			}

			internal void HandleMissingMethod (DuringCallbackEventArgs e) {
				Handle(e, _aspect.MissingMethodBehavior);
			}


			private void Handle(DuringCallbackEventArgs e, Action<DuringCallbackEventArgs> handler) {
				if (_aspect.ShouldIntercept(new InterceptInfo(e)) && _aspect.Active)
					handler(e);
			}
		}

		private static void CallBackDuringMethodCall (TMDuringCallEventArgs eventArgs) {
			Type[] types = new Type[0];
			TypeParams tp = eventArgs.MethodGenericParameters as TypeParams;
			if (tp != null) {
				types = tp.GetTypes();
			}
			DuringCallbackEventArgs e = new DuringCallbackEventArgs() {
				GenericParams = types,
				MethodBehavior = MethodBehaviors.IDontCare,
				MethodName = eventArgs.MethodName,
				TargetInstance = eventArgs.Context,
				TypeName = eventArgs.TypeName,
				ReturnValueOrException = null,
				ParameterValues = eventArgs.Parameters
			};
			Action<DuringCallbackEventArgs> eventTofire = duringMethodCall;
			MethodBehaviors decision = MethodBehaviors.IDontCare;

			if (eventArgs.MethodName == ".ctor") {
				eventTofire = DuringConstructorCall;
			}
			if (eventArgs.MethodName == ".cctor") {
				//                                Console.WriteLine("Checking {0}.{1}", e.TypeName, e.MethodName);
				eventTofire = DuringStaticConstructorCall;
			}


			if (MissingMethodHelper.IsMissing(e)) {
				if (MissingMethodHelper.HasMissingMethodImpl(e)) {
					MissingMethodHelper.InvokeMissingMethod(e);
					decision = e.MethodBehavior;

				}
				else {
					e = MissingMethodHelper.CreateMissingMethodArgsFrom(e);
					decision = InvokeDuringEventWithBehaviorDecision(missingMethodCall, e);
				}
			}
			else {
				decision = InvokeDuringEventWithBehaviorDecision(eventTofire, e);
			}

			//            Console.WriteLine("- Decided {2} on {0}.{1}", typeName, methodName,decision);
			switch (decision) {
				case MethodBehaviors.IDontCare:
					eventArgs.Returns(IsolationBehavior.LetIsolatorDecide);
					break;
				case MethodBehaviors.CallRealMethod:
					//                        Console.WriteLine("INVOKING {0}.{1}", typeName, methodName);
					eventArgs.ReturnsValue(MockManager.CONTINUE_WITH_METHOD);
					break;

				case MethodBehaviors.SkipActualMethod:
					//                    Console.WriteLine("Skipping {0}.{1}" , typeName,methodName);
					eventArgs.Returns(IsolationBehavior.SkipMethod);
					break;

				case MethodBehaviors.ReturnsCustomValue:
					eventArgs.ReturnsValue(e.ReturnValueOrException);
					break;

				case MethodBehaviors.ThrowException:
					eventArgs.ThrowsException(e.ReturnValueOrException as Exception);
					break;
			}
		}




		private static void CallbackBeforeMethodCall (TMShouldInterceptEventArgs eventArgs) {

			eventArgs.Returns(InterceptBehavior.LetIsolatorDecide); //don't intercept by default

			lock (AspecsSync) {
				foreach (var aspect in aspects.Values) 
				{
					if (aspect.Active && aspect.ShouldIntercept(new InterceptInfo(eventArgs))) 
					{
						eventArgs.Returns(InterceptBehavior.Intercept); //only intercept if any of the aspects needs this
						return;
					}
				}				
			}


			// Sometimes we want to add interception without aspects, using the static CThruEngine events directly.
			// This is used in MethodMissingTests, for example.
			// Currently the only public event is MissingMethodCall, but we could implement the rest similarly.
			if (interceptUsingEvents)
				eventArgs.Returns(InterceptBehavior.Intercept); 

			//eventArgs.Returns(InterceptBehavior.Intercept);
			//return;


			//--------------------------------------------------------
			//uncomment the following to find out which namespaces are used in your tests.
			//--------------------------------------------------------
			//            string[] split = typeName.Split('.');
			//            if (!types.Contains(split[0]))
			//            {
			//                types.Add(split[0]);
			//                Console.WriteLine(split[0]);
			//            }
			//            return InterceptBehavior.Intercept;
			//--------------------------------------------------------

			//example of possible optimizations
			//var allow = new string[] { "System", "MS", "ControlTests" };
			//for (int i = 0; i < allow.Length; i++) {
			//    if (eventArgs.TypeName.StartsWith(allow[i])) {
			//        eventArgs.Returns(InterceptBehavior.Intercept);
			//        return;
			//    }
			//}
			//
		}


		private static MethodBehaviors InvokeDuringEventWithBehaviorDecision (Action<DuringCallbackEventArgs> call, DuringCallbackEventArgs args) {

			if (call == null) {
				return MethodBehaviors.IDontCare;
			}
			bool wasLastOptionChangedAtLeastOnce = false;
			MethodBehaviors lastOption = args.MethodBehavior;
			Aspect lastAspect = null;

			Delegate[] list = call.GetInvocationList();
			foreach (Delegate d2 in list) {
				try {
					Action<DuringCallbackEventArgs> invokeEvent = d2 as Action<DuringCallbackEventArgs>;
					args.MethodBehavior = MethodBehaviors.IDontCare;
					args.WasBehaviorSet = false;

					invokeEvent(args);
					bool wasRealBehaviorSet = args.WasBehaviorSet && args.MethodBehavior != MethodBehaviors.IDontCare;
					bool isNewBehaviorConflictingWithOlderBehaviorRequest = wasRealBehaviorSet && lastOption != MethodBehaviors.IDontCare;
					bool isThisTheFirstRealBehaviorSelection = wasRealBehaviorSet && !wasLastOptionChangedAtLeastOnce;

					//var aspect = GetAspectHandledBy(invokeEvent);

					if (isThisTheFirstRealBehaviorSelection) {
						wasLastOptionChangedAtLeastOnce = true;
						lastAspect = GetAspectHandledBy(invokeEvent);
						lastOption = args.MethodBehavior;
						continue;
					}

					if (isNewBehaviorConflictingWithOlderBehaviorRequest) {
						Aspect currentAspect = GetAspectHandledBy(invokeEvent);
						string msg = BuildConflictMessage(args, lastOption, lastAspect, currentAspect);
						throw new ConflictingBehaviorException(msg);
					}
					if (args.MethodBehavior != MethodBehaviors.IDontCare) {
						lastAspect = GetAspectHandledBy(invokeEvent);
						lastOption = args.MethodBehavior;
					}
				}

				catch (Exception e) {
					args.MethodBehavior = MethodBehaviors.ThrowException;
					args.ReturnValueOrException = e;
					return MethodBehaviors.ThrowException;
				}
			}

			return lastOption;
		}

		private static string BuildConflictMessage (DuringCallbackEventArgs args, MethodBehaviors lastOption, Aspect lastAspect, Aspect currentAspect) {
			return string.Format(
				@"Two aspects requested conflicting behavior.
Method:{0}.{1}
Aspect {2} requested {3}
Aspect {4} requested {5}",
				args.TypeName, args.MethodName,
				lastAspect.GetType().Name, lastOption,
				currentAspect.GetType().Name, args.MethodBehavior);
		}


		public static void StopListening () {
			EventTunnel.ShouldInterceptDecision = null;
			EventTunnel.DuringMethodCall = null;
		}

		public static void StopListeningAndReset () {
			StopListening();
			DuringConstructorCall = null;
			DuringStaticConstructorCall = null;
			duringMethodCall = null;
			missingMethodCall = null;
			interceptUsingEvents = false;
			aspects.Clear();
            addedInternalAspects = false;


		}

		private static string GetUniqueKey(string suggested) {
			if (!aspects.ContainsKey(suggested)) return suggested;
			var i = 1;
			while (aspects.ContainsKey(suggested + i.ToString())) {i++;}
			return suggested + i.ToString();
		}

		/// <summary>
		/// Finds an aspect by its key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static Aspect FindAspect (string key) {
			if (aspects.ContainsKey(key)) 
				return aspects[key];
			return null;
		}

		/// <summary>
		/// Returns the list of all invocations.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Action<DuringCallbackEventArgs>> GetInvocationList () {
			IEnumerable<Action<DuringCallbackEventArgs>> result = duringMethodCall.GetInvocationList().Cast<Action<DuringCallbackEventArgs>>();
			result = result.Concat(DuringStaticConstructorCall.GetInvocationList().Cast<Action<DuringCallbackEventArgs>>());
			result = result.Concat(DuringConstructorCall.GetInvocationList().Cast<Action<DuringCallbackEventArgs>>());
			result = result.Concat(missingMethodCall.GetInvocationList().Cast<Action<DuringCallbackEventArgs>>());
			return result;
		}


		public static Aspect GetAspectHandledBy (Action<DuringCallbackEventArgs> handler) {
			var aspectHandler = handler.Target as AspectHandler;
			if (aspectHandler != null) return aspectHandler.Aspect;
			return null;
		}
	}
}
