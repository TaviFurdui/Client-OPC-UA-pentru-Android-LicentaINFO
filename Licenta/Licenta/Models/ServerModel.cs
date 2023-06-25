using System.Collections.Generic;

namespace Licenta.Models
{
    public static class ServerModel
    {
        public static string ServerName { get; set; }
        public static string ServerIP { get; set; }
        public static bool Connected { get; set; } = false;
        public static Server Server { get; set; }
    }

    public class Server
    {
        public Server()
        {

        }
        public Server(string serverName, string serverIP, bool connected)
        {
            ServerName = serverName;
            ServerIP = serverIP;
            Connected = connected;
        }
        public string ServerName { get; set; }
        public string ServerIP { get; set; }
        public bool Connected { get; set; } = false;
    }
    public class Servers
    {
        public List<Server> servers { get; set; } = new List<Server>();
    }
}
