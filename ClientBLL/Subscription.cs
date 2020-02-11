using System;

namespace Client
{
    public class Subscription
    {
        public event Action<object[]> Data = (o) => { };

        internal Subscription() { }

        internal void Invoke(params object[] obj)
        {
            Data.Invoke(obj);
        }
    }
}
