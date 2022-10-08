
using BoneyServer.domain;
using BoneyServer.services;
using Grpc.Core;
using System.Diagnostics;

namespace BoneyServer
{
    public class Program
    {

        public static void Main(string[] args) // TODO - edit to receive all server state through the config file
        {
            // Will be passed as args from puppetmaster
            const int port = 8001;
            const string hostname = "localhost";
            const uint maxSlots = 3;



            string path = "..\\..\\..\\..\\BankServer\\bin\\Debug\\net6.0\\";
            string appName = "BankServer.exe";
            string arg1 = "bloblo";
            string title = "Bank";
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {arg1}";
            p.Start();


            BoneySlotManager slotManager = new BoneySlotManager(maxSlots);
            string startupMessage;
            ServerPort serverPort;
            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);
            startupMessage = "Insecure ChatServer server listening on port " + port;

            CompareAndSwapServiceImpl compareAndSwapService = new CompareAndSwapServiceImpl(slotManager);

            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(compareAndSwapService) },
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
