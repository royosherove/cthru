using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CThru.BuiltInAspects;
using NUnit.Framework;

namespace CThru.Tests {
	[TestFixture]
	public class StubTests {
		const string FAKE = "fake value";
		[Test]
		public void CanStubStaticMethods() {
			CThruEngine.AddAspect(Stub.For<Sut>("StaticMethod").Return(FAKE));
			CThruEngine.StartListening();
			var result = Sut.StaticMethod();
			Assert.AreEqual(FAKE, result);
		}

		[Test]
		public void MethodNameMatters() {
			CThruEngine.AddAspect(Stub.For<Sut>("StaticMethod").Return(FAKE));
			CThruEngine.StartListening();
			var result = Sut.AnotherMethod();
			Assert.AreNotEqual(FAKE, result);
		}

		[Test]
		public void CanStubInstanceMethodsUsingBaseClass() {
			CThruEngine.AddAspect(Stub.For<IAmAnInterface>("InstanceMethod").Return(FAKE));
			CThruEngine.StartListening();
			var result = new Sut().InstanceMethod();
			Assert.AreEqual(FAKE, result);
		}

		public class Sut : IAmAnInterface {
			private const string REAL_VALUE = "real value";

			public static string StaticMethod() {
				return REAL_VALUE;
			} 

			public static string AnotherMethod() {
				return REAL_VALUE;
			}

			public string InstanceMethod() {
				return REAL_VALUE;
			}
		}
	}

	public interface IAmAnInterface {}
}
