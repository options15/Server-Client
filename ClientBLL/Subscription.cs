using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientBLL
{
    public class Subscription
    {
        public event Action<object[]> Data;

        internal Subscription() {}

        internal void Invoke( params object[] obj )
        {
            Data.Invoke(obj);
        }
    }
}
