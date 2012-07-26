using System;

namespace CThru
{
    public class MissingMethodArgs
    {
        public static MissingMethodArgs FromLateCallArguments(DuringCallbackEventArgs e)
        {
            //method signature:
            //LateCall(Object Instance, Type Type, String MemberName, Object[] Arguments, String[] ArgumentNames, Type[] TypeArguments, Boolean[] CopyBack, Boolean IgnoreReturn)
            //          (0                , 1 NULL         , 2             , 3                 , 4  NULL                    , 5 NULL            , 6 NULL            , 7)

            MissingMethodArgs @new = new MissingMethodArgs()
            {
                Arguments = (object[])e.ParameterValues[3],
                MemberName = (string)e.ParameterValues[2],
                Type = e.ParameterValues[1] == null ? null : e.ParameterValues[1] as Type,
                TargetInstance = e.ParameterValues[0],
                TypeArguments = e.ParameterValues[5] == null ? new Type[0] : (Type[])e.ParameterValues[5],
                ArgumentNames =  e.ParameterValues[4] == null ? new string[0] : (string[])e.ParameterValues[4],
//                IgnoreReturn = Convert.ToBoolean(e.ParameterValues[7] )
            };
            return @new;
        }
        public object TargetInstance { get; set; }
        public Type Type { get; set; }
        public string MemberName { get; set; }
        public object[] Arguments { get; set; }
        public string[] ArgumentNames { get; set; }
        public Type[] TypeArguments { get; set; }
        public bool IgnoreReturn { get; set; }
    }
}
