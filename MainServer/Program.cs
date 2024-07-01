using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using ServerCore;

namespace MainServer
{
    class Program
    {
        static Listener _listener = new Listener();
        public static Dictionary<int, ClientSession> clients = new Dictionary<int, ClientSession>();
        static Settings _settings = new Settings();

        static void Main(string[] args)
        {
            _settings.Set();
            
            IPAddress ipAddr = IPAddress.Parse(Settings.settings["ipAddress"]);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, int.Parse(Settings.settings["port"]));

            _listener.Init(endPoint, () => { return new ClientSession(); });

            while (true)
            {

            }
        }


        

    }
}
