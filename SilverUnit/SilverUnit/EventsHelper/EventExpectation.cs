using System;
using System.Reflection;
using TeamAgile.NUnitExtensions.EventsTesting.CommonEventHandler;

namespace Osherove.Events
{
    public class EventExpectation : IDisposable
    {
        private bool callbackCalled = false;
        private string expectedEventName = String.Empty;
        private string expectedTargetName = String.Empty;
        private object[] expectedArgs = new object[] { };
        private object[] actualArgs = new object[] { };
        private bool expectNoFire = false;

        public void Verify()
        {
            if (expectNoFire)
            {
                expectNoFire = false;
                return;
            }

            if (!callbackCalled)
            {
                string message = "Event '{0}' was expected from class '{1}'";
                throw new Exception(string.Format(message, expectedEventName, expectedTargetName));
            }

        }

        public bool WasRaised
        {
            get { return callbackCalled; }
        }

        public void ExpectNoFire(object target, string eventName)
        {
            expectNoFire = true;
            subscribeToEvent(target, eventName);
        }

        public void ExpectEvent(object target, string eventName, params object[] args)
        {
            expectNoFire = false;
            expectedArgs = args;
            subscribeToEvent(target, eventName);

        }

        private void subscribeToEvent(object target, string eventName)
        {
            Type targetType = target.GetType();

            expectedTargetName = targetType.Name;
            expectedEventName = eventName;
            EventInfo eventInfo = targetType.GetEvent(eventName);

            registerLocallyForEvent(eventInfo, target);
        }


        public void MyEventCallback(Type eventType, object[] args)
        {
            callbackCalled = true;
            actualArgs = args;

            if (expectNoFire)
            {
                expectNoFire = false;
                throw new Exception(string.Format("Unexpected event was fired:{0}.{1}"
                                                  , expectedTargetName, expectedEventName));
            }


            for (int i = 0; i < expectedArgs.Length; i++)
            {
                if (expectedArgs[i]!= actualArgs[i])
                {
                    throw new Exception(string.Format("Wrong argument[{0}] value passed by event",i));
                }
            }

        }

        public object[] ActualArgs
        {
            get { return actualArgs; }
        }

        #region registerForEvent

        private MethodInfo registerLocallyForEvent(EventInfo eventInfo, object target)
        {
            EventHandlerFactory factory = new EventHandlerFactory("testing");

            object handler = factory.GetEventHandler(eventInfo);

            // Create a delegate, which points to the custom event handler
            Delegate customEventDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler, "CustomEventHandler");
            // Link event handler to event
            eventInfo.AddEventHandler(target, customEventDelegate);

            // Map our own event handler to the common event
            EventInfo commonEventInfo = handler.GetType().GetEvent("CommonEvent");
            Delegate commonDelegate = Delegate.CreateDelegate(commonEventInfo.EventHandlerType, this, "MyEventCallback");
            commonEventInfo.AddEventHandler(handler, commonDelegate);

            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Verify();
        }

        #endregion
    }
}