using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CThru.BuiltInAspects;
using NUnit.Framework;

namespace CThru.Tests
{
	[TestFixture]
	public class TracerTests
	{
		[Test]
		public void ShouldOutputToTheSpecifiedWriter()
		{
			using (var writer = new StringWriter())
			{
				CThruEngine.AddAspect(new TraceAspect(info => info.MethodName == "SomeMethod", writer));
				CThruEngine.StartListening();
				this.SomeMethod();
				Assert.AreEqual("CThru.Tests.TracerTests.SomeMethod()\r\n", writer.ToString());

			}
		}

		[Test]
		public void ShouldOutputStackTrace() {
			using (var writer = new StringWriter())
			{
				CThruEngine.AddAspect(new TraceAspect(info => info.MethodName == "SomeMethod", writer, 1));
				CThruEngine.StartListening();
				this.SomeMethod();
                StringAssert.Contains("CThru.Tests.TracerTests.SomeMethod()", writer.ToString());

			}
			
		}

		[Test]
		public void ShouldOutputTheResult() {
			using (var writer = new StringWriter()) {
				CThruEngine.AddAspect(new TraceResultAspect(info => info.MethodName == "SomeMethod", writer));
				CThruEngine.StartListening();
				this.SomeMethod();
				Assert.AreEqual("CThru.Tests.TracerTests.SomeMethod() returned \"42\"\r\n", writer.ToString());				
			}
		}

		[TearDown]
		public void Reset()
		{
			CThruEngine.StopListeningAndReset();
		}

		public int SomeMethod() {
			return 42;
		}

	}
}
