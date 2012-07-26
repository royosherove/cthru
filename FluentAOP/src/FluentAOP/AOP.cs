using System;
using CThru;

namespace FluentAOP
{
    class AspectHolder:Aspect
    {
        public Action<DuringCallbackEventArgs> OnMethodBehavior=delegate {  };
        public override void MethodBehavior(DuringCallbackEventArgs e)
        {
            OnMethodBehavior(e);
        }

        public override bool ShouldIntercept(InterceptInfo info)
        {
            return true;
        }
    }
    enum AopType
    {
        ReturnValue,
        Skip,
        Custom
    }
    internal class AopParams
    {
        public AopType Behavior { get; set; }
        public object ReturnValue { get; set; }
        public Func<DuringCallbackEventArgs,bool> Filter { get; set; }
        public Action<DuringCallbackEventArgs> CustomBehavior { get; set; }
    }
    public class AOP:IFilter, IDisposable, IReturn
    {
        private AopParams current;
        public IFilter SkipMethod
        {
            get
            {
                current = new AopParams(){Behavior = AopType.Skip};
                return this as IFilter;
            }
        }

        public IReturn Returns
        {
            get
            {
                current = new AopParams() {Behavior = AopType.ReturnValue};
                return this as IReturn;
            }
        }
        
        void IFilter.Whenever(Func<DuringCallbackEventArgs, bool> filter)
        {
            current.Filter = filter;
            AspectHolder ah = new AspectHolder();
            switch (current.Behavior)
            {
                case AopType.ReturnValue:
                    ah.OnMethodBehavior = e =>{if (current.Filter(e))
                        {
                            e.ReturnValueOrException = current.ReturnValue;
                            e.MethodBehavior = MethodBehaviors.ReturnsCustomValue;
                        }};
                    CThruEngine.AddAspect(ah);
                    break;

                case AopType.Skip:
                ah.OnMethodBehavior = e =>{if (filter(e))
                    {
                        e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                    }
                };
                CThruEngine.AddAspect(ah);
                break;
                case AopType.Custom:
                ah.OnMethodBehavior = e =>
                {
                    if (current.Filter(e))
                    {
                        current.CustomBehavior(e);
                    }
                };
                CThruEngine.AddAspect(ah);
                break;
                default:
                    break;
            }
        }

        public void Enable()
        {
            CThruEngine.Verbose = false;
            CThruEngine.StartListening();   
        }
        
        public void Disable()
        {
            CThruEngine.StopListening();
        }

        public void Dispose()
        {
            CThruEngine.StopListeningAndReset();
            
        }

        private AspectHolder aspectBeingBuilt;
        private object pendingReturnValue;
//        private Func<DuringCallbackEventArgs, bool> pendingFilter=args => ;

        IFilter IReturn.Value(object retValue)
        {
            current.ReturnValue = retValue;
            return this as IFilter;
        }
    }

    public interface IReturn
    {
        IFilter Value(object retValue);
    }

    public interface IFilter
    {
        void Whenever(Func<DuringCallbackEventArgs, bool> filter);
    }
}
