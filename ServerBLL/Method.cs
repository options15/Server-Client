using System;

namespace ServerBLL
{
    class Method
    {
        public event Action<object[]> Data = (o) => { };

        internal Method() { }

        internal void Invoke(params object[] obj)
        {
            Data.Invoke(obj);
        }
    }
}
