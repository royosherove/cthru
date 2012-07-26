//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection.Emit;
//using ClrTest.Reflection;
//using ReflectionDemos.ILParser;
//using TypeMock.Internal.Hooks;

//namespace CThru
//{
//    public class InterceptionOptimizer
//    {
//        private Aspect aspect;
//        public void IdentifyRulesInAspect(Aspect aspect)
//        {
//            this.aspect = aspect;
//        }

//        public bool HasAspectsThatRequire(TMShouldInterceptEventArgs args)
//        {
//            if (args.TypeName=="")
//            {
//                return false;
//            }

//            var info = aspect.GetType().GetMethod("MethodBehavior");
//            var reader = ILReaderFactory.GetReader(info);
//            string requestedTypename = "";
//            bool isContains = true;
//            IEnumerator<ILInstruction> enu = (IEnumerator<ILInstruction>) ((IEnumerable)reader).GetEnumerator();
//            foreach (var instruction in reader)
//            {
//                if (instruction.OpCode==OpCodes.Ldstr)
//                {
//                    requestedTypename = instruction.ProcessedOperand.Trim('"');
//                    enu.MoveNext();
//                    isContains = enu.Current.ProcessedOperand.Contains("Contains");
//                    break;
//                }
//            }
//            if (isContains)
//            {
//                return args.TypeName.Contains(requestedTypename);
//            }
//            return args.TypeName == requestedTypename;
//        }
//    }
//}
