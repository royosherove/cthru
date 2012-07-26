//using System;
//using NUnit.Framework;
//using TypeMock.Internal.Hooks;

//namespace CThru.Tests
//{
//    [TestFixture]
//    public class InterceptionOptimizerTests
//    {
//        [Test]
//        public void HasAspectsThatRequire_TMArgsWithEmptyNamespace_False()
//        {
//            var args = new TMShouldInterceptEventArgs(null, "", "", null, false);
//            var o = new InterceptionOptimizer();
//            o.IdentifyRulesInAspect(new AspectThatRequiresNamespaceContaining_A_InMethodBehavior());
            
//            Assert.IsFalse(o.HasAspectsThatRequire(args));
//        }
        
//        [Test]
//        public void HasAspectsThatRequire_TMArgsWithCorrectNamespace_True()
//        {
//            var args = new TMShouldInterceptEventArgs(null, "A", "", null, false);
//            var o = new InterceptionOptimizer();
//            o.IdentifyRulesInAspect(new AspectThatRequiresNamespaceContaining_A_InMethodBehavior());
            
//            Assert.IsTrue(o.HasAspectsThatRequire(args));
//        }
        
//        [Test]
//        public void HasAspectsThatRequire_TMArgsWithCorrectCONTAINSNamespace_True()
//        {
//            var args = new TMShouldInterceptEventArgs(null, "AB", "", null, false);
//            var o = new InterceptionOptimizer();
//            o.IdentifyRulesInAspect(new AspectThatRequiresNamespaceContaining_A_InMethodBehavior());

//            Assert.IsTrue(o.HasAspectsThatRequire(args));
//        }
        
//        [Test]
//        public void HasAspectsThatRequire_TMArgsWithInCorrectEXACTNamespace_FALSE()
//        {
//            var args = new TMShouldInterceptEventArgs(null, "AB", "", null, false);
//            var o = new InterceptionOptimizer();
//            o.IdentifyRulesInAspect(new AspectThatRequiresNamespaceEXACTLY_B_InMethodBehavior());

//            Assert.IsFalse(o.HasAspectsThatRequire(args));
//        }
   
//        [Test]
//        public void HasAspectsThatRequire_TMArgsWithInCorrectNamespace_False()
//        {
//            var args = new TMShouldInterceptEventArgs(null, "B", "", null, false);
//            var o = new InterceptionOptimizer();
//            o.IdentifyRulesInAspect(new AspectThatRequiresNamespaceContaining_A_InMethodBehavior());
            
//            Assert.IsFalse(o.HasAspectsThatRequire(args));
//        }
//    }

//    internal class AspectThatRequiresNamespaceContaining_A_InMethodBehavior : Aspect
//    {
//        public override void MethodBehavior(DuringCallbackEventArgs e)
//        {
//            if (e.TypeName.Contains("A"))
//            {
//                Console.WriteLine("A! Found!");
//            }
//        }

//        public override bool ShouldIntercept(InterceptInfo info) {
//            return true; // perhaps should be: return info.TypeName.Contains("A")
//        }
//    }
    
//    internal class AspectThatRequiresNamespaceEXACTLY_B_InMethodBehavior : Aspect
//    {
//        public override void MethodBehavior(DuringCallbackEventArgs e)
//        {
//            if (e.TypeName== "B")
//            {
//                Console.WriteLine("B! Found!");
//            }
//        }

//        public override bool ShouldIntercept(InterceptInfo info) {
//            return true; // perhaps should be: return info.TypeName == "B"
//        }
//    }
//}
