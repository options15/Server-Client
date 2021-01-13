using System;
using System.Reflection;

namespace ServerBLL
{
    internal class HubInvoker
    {
        internal HubInvoker() { }

        internal bool TryInvoke(object[] message)
        {
            try
            {
                Assembly asm = Assembly.GetEntryAssembly();

                Type type = asm.GetType(asm.GetName().Name + "." + message[0].ToString(), false, false);
                object obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod(message[1].ToString());

                method.Invoke(obj, new object[] { message });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
