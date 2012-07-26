using System;

using NUnit.Framework;

namespace FluentAOP.Tests
{
    [TestFixture]
    public class FluentAPITests
    {
        [Test]
        public void SkipMethod_SingleRequest_SkipsCallWithException()
        {
             using(var aop = new AOP())
             {
                 aop.SkipMethod.Whenever((args) => args.TypeName.Contains("UnderTest"));
                 aop.Enable();


                 var test = new UnderTest();
                 test.FooAction = () => { throw new Exception("should not be thrown"); };
                 test.foo();
             }
        }
        
        [Test]
        public void SkipMethod_MultipleRequests_SkipsCallsWithException()
        {
            using(var aop = new AOP())
            {
                aop.SkipMethod.Whenever((args) => args.TypeName.Contains("UnderTest"));
                aop.SkipMethod.Whenever((args) => args.TypeName.Contains("OtherType"));
                aop.Enable();


                var test = new UnderTest();
                test.FooAction = () => { throw new Exception("should not be thrown"); };
                test.foo();

                var test2 = new OtherType();
                test2.FooAction = () => { throw new OutOfMemoryException("should not be thrown"); };
                test2.foo();
            }
        }
        
        [Test]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void SkipMethod_SingleRequestBadFilter_MethodNotSkipped()
        {
            using(var aop = new AOP())
            {
                aop.SkipMethod.Whenever((args) => args.TypeName.Contains("UnderTestBADSTRING"));
                aop.Enable();


                var test = new UnderTest();
                test.FooAction = () => { throw new OutOfMemoryException("should not be thrown"); };
                test.foo();
            }
        }



        [Test]
        public void ReturnCustomValueFromMethod_SingleRequest_ReturnsCustomValue()
        {
            using(var aop = new AOP())
            {
                aop.Returns.Value(2).Whenever((args) => args.MethodName.Contains("ReturnOne"));
                aop.Enable();


                var test = new UnderTest();
                test.RetOneAction = () => { return 1; };
                int result = test.ReturnOne();
                Assert.AreEqual(2, result);
            }
        }
    }


    class UnderTest
    {
        public Action FooAction=delegate {  };
        public Func<int> RetOneAction = delegate { return 1; };
        public void foo()
        {
            FooAction();
        }

        public int ReturnOne()
        {
            return RetOneAction();
        }
    }
    
    class OtherType
    {
        public Action FooAction=delegate {  };
        public void foo()
        {
            FooAction();
        }    
    }

}
