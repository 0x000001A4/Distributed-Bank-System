
using BoneyServer.domain;
using System;
using BoneyServer.services;
using BoneyServer.utils;
using Grpc.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BoneyServer
{
    public class BoneyServer
    {

        public static void Main(string[] args) // TODO - edit to receive all server state through the config file
        {
            (Dictionary<int, string>, Dictionary<int, string>, string[,], string[,], List<int>, string, int, int) tuplo;
            //Console.WriteLine(args[0]);

            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            int processID = int.Parse(args[1]);
            (string hostname, int port) = config.GetBankHostnameAndPortByProcess(processID);
            //var expression = new Regex(@"(?<localhost>[^:]+):(?<portnumber>[0-9]+)");
            //var match = expression.Match(config.GetBoneyHostnameByProcess(processID));
            // Will be passed as args from pupetmaster
            const uint maxSlots = config.GetNumberofSlots();

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


            //Console.WriteLine(startupMessage);
            //Configuring HTTP for client connections in Register method
            AppContext.SetSwitch(
  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            while (true) ;
        }



    }

}
