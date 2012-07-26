using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CThru.BuiltInAspects;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace CThru.Tests
{
    [TestFixture]
    public class CThruEngineTests
    {
        [SetUp]
        public void setup()
        {
            CThruEngine.StopListeningAndReset();
            Assert.IsTrue(CThruEngine.EventsAreClear);
        }
        [TestFixtureTearDown]
        public void teardown()
        {
           CThruEngine.StopListeningAndReset();
           Assert.IsTrue(CThruEngine.EventsAreClear);
        }

        [Test]
        public void DuringMethodCall_WithGenericArgument_EventArgsContainArgumentType()
        {
            var stub = new StubAspect();
            stub.OnMethodBehaviorCallback += e=>
                                                 {
                                                     if(e.MethodName=="SomeMethod2")
                                                     {
                                                         Assert.AreEqual(typeof(string),e.GenericParams[0]);
                                                     }
                                                 };
            CThruEngine.AddAspect(stub);
            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod2<string>(1,"a");
        }
        
        [Test]
        public void DuringMethodCall_WithTwoGenericArguments_EventArgsContainArgumentTypes()
        {
            var stub = new StubAspect();
            stub.OnMethodBehaviorCallback += e=>
                                                 {
                                                     if(e.MethodName=="SomeMethod3")
                                                     {
                                                         Assert.AreEqual(typeof(int),e.GenericParams[0]);
                                                         Assert.AreEqual(typeof(string),e.GenericParams[1]);
                                                         Assert.AreEqual(2, e.GenericParams.Length);
                                                     }
                                                 };
            CThruEngine.AddAspect(stub);
            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod3<int,string>(1,"a",3);
        }
        
        [Test]
        public void DuringMethodCall_WithoutGenericArgument_EventArgsContainNoArgumentTypes()
        {
            var stub = new StubAspect();
            stub.OnMethodBehaviorCallback += e=>
                                                 {
                                                     if(e.MethodName=="SomeMethod")
                                                     {
                                                         Assert.AreEqual(0,e.GenericParams.Length);
                                                     }
                                                 };
            CThruEngine.AddAspect(stub);
            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod();
        }
        [Test]
        public void WorkWithArguments()
        {
            bool wascalled = false;
            var stub = new StubAspect();
            stub.OnMethodBehaviorCallback += args => wascalled = true;
            CThruEngine.AddAspect(stub);

            SomeTargetClass.SomeMethod();
            Assert.IsFalse(wascalled);
        }

        [Test]
        public void DuringMethodCall_RegisteredAndStartedListening_CallbackIsFired()
        {
            List<string> methodNames = new List<string>();
            var stub = new StubAspect();
            stub.OnMethodBehaviorCallback += args => methodNames.Add(args.MethodName);
            
            CThruEngine.AddAspect(stub);
            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod();
            CThruEngine.StopListening();
            CollectionAssert.Contains(methodNames, "SomeMethod");

        }
        
        [Test]
        [ExpectedException(ExceptionType=typeof(ConflictingBehaviorException))]
        public void DuringMethodCall_TwoAspectsHaveDifferentBehaviorRequests_ThrowsException()
        {
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          e.MethodBehavior = MethodBehaviors.CallRealMethod;
                                                  };
            stub2.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod3();
            
        }

		[Test]
		public void ExceptionMessageContainsConflictingAspectName() {
			var stub1 = new StubAspect();
			var stub2 = new StubAspect();
			stub1.OnMethodBehaviorCallback += e => {
														  e.MethodBehavior = MethodBehaviors.CallRealMethod;
												  };
			stub2.OnMethodBehaviorCallback += e => {
														  e.MethodBehavior = MethodBehaviors.SkipActualMethod;
												  };

			CThruEngine.AddAspect(stub1);
			CThruEngine.AddAspect(stub2);

			CThruEngine.StartListening();
			try {							//would be cool to use the new NUnit 2.5 syntax(Assert.That(.., Throws))
				SomeTargetClass.SomeMethod3();
			}
			catch (ConflictingBehaviorException exception) {
				Assert.That(exception.Message, new SubstringConstraint("Aspect StubAspect requested CallRealMethod"));
			}			
		}
	
        
        
        [Test]
        public void DuringMethodCall_InvokedForAllAspects()
        {
            int callCounter = 0;
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          callCounter++;
                                                  };
            stub2.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          callCounter++;
                                                          
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod3();
            CThruEngine.StopListening();
            Assert.AreEqual(2,callCounter);
        }
        
        
        [Test]
        public void DuringCtorCall_InvokedForAllAspects()
        {
            int callCounter = 0;
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnCTorBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == ".ctor" && e.TypeName.Contains("SomeTargetClass"))
                                                          callCounter++;
                                                  };
            stub2.OnCTorBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == ".ctor" && e.TypeName.Contains("SomeTargetClass"))
                                                          callCounter++;
                                                          
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            new SomeTargetClass();
            CThruEngine.StopListening();
            Assert.AreEqual(2,callCounter);
        }
        
        
        [Test]
        public void DuringStaticCtorCall_InvokedForAllAspects()
        {
            int callCounter = 0;
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnStaticCtorBehaviorCallback+= e =>
                                                  {
                                                      if (e.MethodName == ".cctor" && e.TypeName.Contains("SomeTargetClass"))
                                                      {
                                                          callCounter++;
                                                      }
                                                  };
            stub2.OnStaticCtorBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == ".cctor" && e.TypeName.Contains("SomeTargetClass"))
                                                      {
                                                          callCounter++;
                                                      }
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            ConstructorInfo initializer = typeof (SomeTargetClass).TypeInitializer;
            initializer.Invoke(null, null);
            CThruEngine.StopListening();
            
            //since we invoke the ctor directly, 
            //it could be the first time it is invoked, 
            //so the "real" one can also be automatically invoked, 
            //giving us one extra call per aspect. We should have at least 1 call per aspect.
            Assert.LessOrEqual(2,callCounter); 
        }
        
        
        
        [Test]
        public void StopListeningAndReset_Called_ClearsEventHandlers()
        {
            int callCounter = 0;
            var stub1 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          callCounter++;
                                                  };
            
            CThruEngine.AddAspect(stub1);

            CThruEngine.StartListening();
            CThruEngine.StopListeningAndReset();
            SomeTargetClass.SomeMethod3();
            Assert.AreEqual(0,callCounter);
        }
        
        
        [Test]
        public void StopListeningAndReset_CalledAfterExceptionReturned_ClearsEventHandlers()
        {
            int callCounter = 0;
            var stub1 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                      {
                                                          if (callCounter == 0)//throw only once
                                                          {
                                                              e.MethodBehavior = MethodBehaviors.ThrowException;
                                                              e.ReturnValueOrException = new Exception("Fake exception");
                                                          }
                                                          callCounter++;
                                                      }
                                                  };
            
            CThruEngine.AddAspect(stub1);

            CThruEngine.StartListening();
            try
            {
                SomeTargetClass.SomeMethod3();
            }
            catch
            {
                
            }
            CThruEngine.StopListeningAndReset();
            Assert.AreEqual(1,callCounter);
            SomeTargetClass.SomeMethod3();
            Assert.AreEqual(1, callCounter);
        }
        
        
        
        [Test]
        public void DuringMethodCall_AspectThrowsException_EngineCatchesItAndThrows()
        {
            var stub1 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                      {
                                                              throw new Exception("Fake exception");
                                                      }
                                                  };
            
            CThruEngine.AddAspect(stub1);

            CThruEngine.StartListening();
            try
            {
                SomeTargetClass.SomeMethod3();
            }
            catch(Exception ex)
            {
                CThruEngine.StopListeningAndReset();
                Assert.AreEqual("Fake exception", ex.Message);
                return;
            }
            Assert.Fail("Exception was expected");
        }
        
        [Test]
        public void StopListeningAndReset_WithTwoAspects_ClearsEventHandlers()
        {
            int callCounter = 0;
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          callCounter++;
                                                  };
            stub2.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod3")
                                                          callCounter++;
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod3();
            CThruEngine.StopListeningAndReset();
            Assert.AreEqual(2,callCounter);

            SomeTargetClass.SomeMethod3();
            Assert.AreEqual(2, callCounter);

        }
        
        
        [Test]
        public void DuringMethodCall_TwoAspectsHaveDifferentBehaviorRequestsOneDoesNotCare_AllIsOK()
        {
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                          e.MethodBehavior = MethodBehaviors.IDontCare;
                                                  };
            stub2.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                          e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod2();
            
        }
        
        [Test]
        public void SkipMethodCall_Used_ActuallySkipped()
        {
            var stub1 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                          e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                                                  };
            
            CThruEngine.AddAspect(stub1);

            CThruEngine.StartListening();
            SomeTargetClass.CallingSomeMethod2ShouldThrowException = true;
            SomeTargetClass.SomeMethod2();
            
        }
        public void SkipMethodCall_UsedAlongWithIDontCare_ActuallySkipped()
        {
            var stub1 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                          e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                                                  };
            var stub2 = new StubAspect();
            stub2.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                          e.MethodBehavior = MethodBehaviors.IDontCare;
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            SomeTargetClass.CallingSomeMethod2ShouldThrowException = true;
            SomeTargetClass.SomeMethod2();
            
        }
        
        [Test]
        public void DuringMethodCall_OneObjectNoBehaviorChangeButOtherDoes_AllIsOK()
        {
            var stub1 = new StubAspect();
            var stub2 = new StubAspect();
            stub1.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                      {
                                                          //do nothing
                                                      }
                                                  };
            stub2.OnMethodBehaviorCallback += e =>
                                                  {
                                                      if (e.MethodName == "SomeMethod2")
                                                          e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                                                  };
            
            CThruEngine.AddAspect(stub1);
            CThruEngine.AddAspect(stub2);

            CThruEngine.StartListening();
            SomeTargetClass.SomeMethod2();
            
        }

		[Test]
		public void DuringMethodCall_IfShouldInterceptReturnsFalse_CallbackIsNotFired () {
			bool wascalled;
			var stub = new NonInterceptingStubAspect();
			stub.OnMethodBehaviorCallback += args => wascalled = true;
			CThruEngine.AddAspect(stub);
			CThruEngine.StartListening();
			wascalled = false;

			SomeTargetClass.SomeMethod();
			Assert.IsFalse(wascalled);
		}

#region Start/stop behavoir
		[Test]
		public void DuringMethodCall_IfStopped_CallbackIsNotFired () {
			bool wascalled;
			var stub = new StubAspect();
			stub.OnMethodBehaviorCallback += args => wascalled = true;
			CThruEngine.AddAspect(stub);
			CThruEngine.StartListening();
			stub.Stop();
			wascalled = false;

			SomeTargetClass.SomeMethod();
			Assert.IsFalse(wascalled);

		}

		[Test]
		public void IfStoppedAndStartedAgain_CallbackIsFired () {
			bool wascalled;
			var stub = new StubAspect();
			stub.OnMethodBehaviorCallback += args => wascalled = true;
			CThruEngine.AddAspect(stub);
			CThruEngine.StartListening();
			stub.Stop();
			stub.Start();
			wascalled = false;

			SomeTargetClass.SomeMethod();
			Assert.IsTrue(wascalled);
		}
		#endregion

#region Aspects and keys
		
		[Test]
		public void CanAddTwoAspectsOfSameType() {
			CThruEngine.AddAspect(new StubAspect());
			try {
				CThruEngine.AddAspect(new StubAspect());

			}
			catch (ArgumentException) {
				Assert.Fail("Can't add two aspects of the same type");
			}
		}

		[Test]
		public void CanFindAspectByKey() {
			var stub = new StubAspect();
			CThruEngine.AddAspect("stub", stub);
			Aspect found = CThruEngine.FindAspect("stub");
			
			Assert.AreSame(found, stub);
		}

		[Test]
		public void IfAddedTwoAspectsWithTheSameKey_TheSecondIsUsed() {
			bool firstInvoked = false;
			bool secondInvoked = false;
			var stub1 = new StubAspect();
			stub1.OnMethodBehaviorCallback += args => firstInvoked = true;
			var stub2 = new StubAspect();
			stub2.OnMethodBehaviorCallback += args => secondInvoked = true;
			CThruEngine.AddAspect("stub", stub1);
			CThruEngine.AddAspect("stub", stub2);
			CThruEngine.StartListening();

			SomeTargetClass.SomeMethod();

			Assert.IsTrue(secondInvoked, "The second aspect should have been invoked");
			Assert.IsFalse(firstInvoked, "The first aspect shouldn't have been invoked");
		}
 
	#endregion	

	
		[Test]
		public void GetAspect_ReturnsTheSourceAspect() {
			var stub = new StubAspect();
			stub.OnMethodBehaviorCallback += e => Console.WriteLine();
			CThruEngine.AddAspect(stub);
			CThruEngine.StartListening();

			var handlers = CThruEngine.GetInvocationList();
			var handler = handlers.First();
			Aspect aspect = CThruEngine.GetAspectHandledBy(handler);

			Assert.AreSame(stub, aspect);
			
		}

		[Test]
		public void StubAdded_TheActualMethodIsNotCalled() {
			var stub = new Stub(info => info.MethodName == "SomeInstanceMethod");
			CThruEngine.AddAspect(stub);
			CThruEngine.StartListening();
			var target = new SomeTargetClass();

			//Act
			target.SomeInstanceMethod();

			//Assert
			Assert.IsFalse(target.InstanceMethodCalled);
		}

		[Test]
		public void GetInterceptedMethod_ReturnsItsMethodInfo() {
			var aspect = new PassThroughAspect(info => info.MethodName == "SomeInstanceMethod");
			CThruEngine.AddAspect(aspect);
			CThruEngine.StartListening();
			var target = new SomeTargetClass();

			//Act
			target.SomeInstanceMethod();

			//Assert
			var methodInfo = aspect.CapturedMethodInfo;
			Assert.That(methodInfo.Name, Is.EqualTo( "SomeInstanceMethod"));
		}

		[Test]
		public void CallOriginal_InvokesTheInterceptedMethod() {
			var aspect = new PassThroughAspect(info => info.MethodName == "SomeInstanceMethod");
			CThruEngine.AddAspect(aspect);
			CThruEngine.StartListening();
			var target = new SomeTargetClass();

			//Act
			target.SomeInstanceMethod();

			//Assert
			Assert.IsTrue(target.InstanceMethodCalled);
		}
	}

  
    class SomeTargetClass
    {
        private static int someNumber = 1;
        public static bool CallingSomeMethod2ShouldThrowException;

        public static void SomeMethod()
        {
        }
        public static void SomeMethod2<T>(int i,T val)
        {
        }
        public static void SomeMethod3<R,T>(R i,T val,int someNumber)
        {
        }
        
        public static void SomeMethod2()
        {
            if (CallingSomeMethod2ShouldThrowException)
            {
                throw new Exception("Fake exception");
            }
            
        }
        
        public static void SomeMethod3()
        {
            
        }

    	public bool InstanceMethodCalled = false;
		internal void SomeInstanceMethod() {
			InstanceMethodCalled = true;
		}
    }

	//class AspectStubbingSomeTargetClass:StubAspect {
	//    public override bool ShouldIntercept (InterceptInfo info) {
	//        return info.TypeName.Contains("SomeTargetClass");
	//    }
	//}

	//internal class StubAspect:Aspect
	//{
	//    public event Action<DuringCallbackEventArgs> OnMethodBehaviorCallback=delegate {  };
	//    public event Action<DuringCallbackEventArgs> OnCTorBehaviorCallback=delegate {  };
	//    public event Action<DuringCallbackEventArgs> OnStaticCtorBehaviorCallback=delegate {  };

	//    public override void MethodBehavior(DuringCallbackEventArgs e)
	//    {
	//        OnMethodBehaviorCallback(e);
	//    }

	//    public override void ConstructorBehavior(DuringCallbackEventArgs e)
	//    {
	//        OnCTorBehaviorCallback(e);
	//    }

	//    public override void StaticConstructorBehavior(DuringCallbackEventArgs e)
	//    {
	//        OnStaticCtorBehaviorCallback(e);
	//    }
	//    public override bool ShouldIntercept (InterceptInfo info) { return true; }
	//}

	/// <summary>
	/// This aspect doesn't intercept any methods
	/// </summary>
	internal class NonInterceptingStubAspect: StubAspect {
		public override bool ShouldIntercept (InterceptInfo info) { return false; }
	}
}
