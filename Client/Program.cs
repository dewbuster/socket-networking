using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static Settings _settings = new Settings();

        static void Main(string[] args)
        {
            _settings.Set();

            IPAddress ipAddr = IPAddress.Parse(Settings.settings["ipAddress"]);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, int.Parse(Settings.settings["port"]));

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new ServerSession(); });

            while (true)
            {
                
            }

            
        }
    }
}
