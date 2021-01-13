using ServerBLL;

namespace ServerPL
{
    class MyHub : Hub
    {
        public void SendMessage(object[] obj)
        {
            Clients.All.Invoke(obj);
        }
    }
}
