namespace CThru.BuiltInAspects
{
    public class SkipAllByTypeName:Aspect
    {

        public override void MethodBehavior(DuringCallbackEventArgs e)
        {
            Write("skipping method {0}", e.MethodName);
            e.MethodBehavior=MethodBehaviors.SkipActualMethod;
        }

        public override void ConstructorBehavior(DuringCallbackEventArgs e)
        {
            Write("skipping ctor {0}", e.TypeName);
            e.MethodBehavior = MethodBehaviors.SkipActualMethod;
        }

        public override void StaticConstructorBehavior(DuringCallbackEventArgs e)
        {
            Write("skipping static ctor {0}", e.TypeName);
            e.MethodBehavior = MethodBehaviors.SkipActualMethod;
        }

        private string lookFor = "";

        public SkipAllByTypeName(string lookFor)
        {
            this.lookFor = lookFor;
        }

        public string Find
        {
            get { return lookFor; }
            set { lookFor = value; }
        }

		public override bool ShouldIntercept (InterceptInfo info) {
			return info.TypeName.Contains(Find);
		}
    }
}
