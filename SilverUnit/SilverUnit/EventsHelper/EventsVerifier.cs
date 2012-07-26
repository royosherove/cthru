//EventsVerifier for NUnit by Roy Osherove.
//Team Agile Consulting, www.TeamAgile.com
//blog: www.iserializable.com
//for questions and comments: eventsTesting@TeamAgile.com

using System;
using System.Collections;
using System.Collections.Generic;

namespace Osherove.Events
{
    public class EventsVerifier : IDisposable
    {
        public object[] RaisedEventArgs
        {
            get
            {
                if (expectations.Count==0)
                {
                    return new object[] {};
                }
                return expectations[0].ActualArgs;
            }
        }

        public bool WasFirstExpectationRaised
        {
            get
            {
                if (expectations.Count==0)
                {
                    return false;
                }
                if (expectations[0].WasRaised)
                {
                    return true;
                }
                return false;
            }
        }

        private List<EventExpectation> expectations = new List<EventExpectation>();

        public void Expect(object target, string eventName, params object[] args)
        {
            EventExpectation expectation = new EventExpectation();
            expectation.ExpectEvent(target, eventName, args);
            expectations.Add(expectation);
        }

        public void Verify()
        {
            foreach (EventExpectation expectation in expectations)
            {
                expectation.Verify();
            }


        }

        public void ExpectNoEvent(object target, string eventName)
        {
            EventExpectation expectation = new EventExpectation();
            expectation.ExpectNoFire(target, eventName);
            expectations.Add(expectation);
        }
        #region IDisposable Members

        public void Dispose()
        {
            Verify();
        }

        #endregion
    }
}


