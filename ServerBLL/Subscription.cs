using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public class Subscription
    {
        public Action<object[]> OnTrigger;

        internal Subscription() {}

        internal void DoEvent( params object[] obj )
        {
            OnTrigger.Invoke(obj);
        }
    }
}
