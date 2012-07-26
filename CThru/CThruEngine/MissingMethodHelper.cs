using System;
using System.Diagnostics;
using System.Reflection;

namespace CThru
{
    class MissingMethodHelper
    {
        public static bool IsMissing(DuringCallbackEventArgs e)
        {
            if (e.TypeName == "Microsoft.VisualBasic.CompilerServices.NewLateBinding" && e.MethodName.StartsWith("Late")  )
            {
                object theInstance = e.ParameterValues[0];
                string memberName = (string) e.ParameterValues[2];

                return theInstance != null && theInstance.GetType().GetMember(memberName).Length == 0;
            }
            //method signature:
            //LateCall(Object Instance, Type Type, String MemberName, Object[] Arguments, String[] ArgumentNames, Type[] TypeArguments, Boolean[] CopyBack, Boolean IgnoreReturn)
            //          (0                , 1 NULL         , 2             , 3                 , 4  NULL                    , 5 NULL            , 6 NULL            , 7)
            return false;
        }

        public static DuringCallbackEventArgs CreateMissingMethodArgsFrom(DuringCallbackEventArgs e)
        {
            //Debug.Assert(e.ParameterValues.Length==7,"Am not dealing with a LateBind call");
            //method signature:
            //LateCall(Object Instance, Type Type, String MemberName, Object[] Arguments, String[] ArgumentNames, Type[] TypeArguments, Boolean[] CopyBack, Boolean IgnoreReturn)
            //          (0                , 1 NULL         , 2             , 3                 , 4  NULL                    , 5 NULL            , 6 NULL            , 7)

            DuringCallbackEventArgs newArgs = GetMissingMethodArgsFromLateCallArgs(e);

            return newArgs;
        }

        private static DuringCallbackEventArgs GetMissingMethodArgsFromLateCallArgs(DuringCallbackEventArgs e)
        {
            return new DuringCallbackEventArgs()
                       {
                           ParameterValues = (object[])e.ParameterValues[3],
                           MethodName = (string)e.ParameterValues[2],
                           TypeName = e.ParameterValues[1]== null ? "":e.ParameterValues[1].ToString(),
                           TargetInstance = e.ParameterValues[0],
                           GenericParams = e.ParameterValues[5] == null ? new Type[0] : (Type[])e.ParameterValues[5]
                       };
        }

//
//        public static MethodBehaviors NotifyParties(Action<MissingMethodEventArgs> action)
//        {
//            return MethodBehaviors.IDontCare;
//        }

        public static bool HasMissingMethodImpl(DuringCallbackEventArgs e)
        {
            var missingArgs = GetMissingMethodArgsFromLateCallArgs(e);
            if (missingArgs.TargetInstance!=null )
            {
                MethodInfo method = missingArgs.TargetInstance.GetType().GetMethod("missing_method",missingSearchFlags);
                if (method !=null)
                {
                    return true;                }
            }
            return false;
        }
        
        private const BindingFlags missingSearchFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public static void InvokeMissingMethod(DuringCallbackEventArgs e)
        {
            var missingArgs = GetMissingMethodArgsFromLateCallArgs(e);
            MethodInfo method = missingArgs.TargetInstance.GetType().GetMethod("missing_method", missingSearchFlags);
            MissingMethodArgs args = MissingMethodArgs.FromLateCallArguments(e);
            var retVal = method.Invoke(missingArgs.TargetInstance, new object[] {args});
            if (false)
            {
                e.MethodBehavior=MethodBehaviors.SkipActualMethod;
            }
            else
            {
                e.MethodBehavior= MethodBehaviors.ReturnsCustomValue;
                e.ReturnValueOrException = retVal;
            }
        }
    }
}
