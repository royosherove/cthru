namespace CThru.BuiltInAspects
{
    public class MissingMethodAspect:Aspect
    {
        const string LATE_BINDING = "Microsoft.VisualBasic.CompilerServices.NewLateBinding";

        public override bool ShouldIntercept(InterceptInfo e)
        {
            if (e.TypeName == LATE_BINDING && e.MethodName.StartsWith("Late"))
            {
                return true;
            }
            return false;
        }
    }
}
