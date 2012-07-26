using System;

namespace CThru
{
    public static class DelgateExtensions
    {
        public static Delegate ClearSubscribers(this MulticastDelegate del)
        {
            if (del==null)
            {
                return null;
            }
            Delegate result = del;
            foreach (var subscriber in del.GetInvocationList())
            {
                result = Delegate.Remove(result, subscriber);
            }
            return result;
        }
    }
}
