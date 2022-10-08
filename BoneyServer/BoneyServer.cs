
using BoneyServer.domain;
using BoneyServer.services;
using Grpc.Core;

namespace BoneyServer
{
    public class BoneyServer
    {

        public static void Main(string[] args)
        {
            // Will be passed as args from pupetmaster
            const int port = 8001;
            const string hostname = "localhost";
            const uint _maxSlots = 3;


            BoneySlotManager slotManager = new BoneySlotManager(_maxSlots);
            string startupMessage;
            ServerPort serverPort;
            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);
            startupMessage = "Insecure ChatServer server listening on port " + port;

            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(new CompareAndSwapServiceImpl(slotManager)) },
                Ports = { serverPort }
            };

            server.Start();

            Console.WriteLine(startupMessage);
            //Configuring HTTP for client connections in Register method
            AppContext.SetSwitch(
  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            while (true) ;
        }

        
    }

}
