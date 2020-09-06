using System.Net;
using System.Net.Sockets;

namespace Ruqqus.Security
{
    public class LocalAuthorizationServer
    {
        private readonly TcpListener server;
        
        public LocalAuthorizationServer(int port, string csrf = null)
        {
            server = new TcpListener(IPAddress.Loopback, port);
            
        }

        public void Start()
        {
            server.Start();
            
        }
        
        
    }
}