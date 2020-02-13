using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
