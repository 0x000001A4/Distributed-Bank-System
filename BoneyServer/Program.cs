
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
            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            uint processID = uint.Parse(args[1]);
            uint maxSlots = (uint)config.GetNumberOfSlots();
            (string hostname, int port) = config.GetBoneyHostnameAndPortByProcess((int)processID);

            BoneySlotManager slotManager = new BoneySlotManager(maxSlots);


            ServerPort serverPort;
            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);

            CompareAndSwapServiceImpl compareAndSwapService = new CompareAndSwapServiceImpl(slotManager);

            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(compareAndSwapService) },
                Ports = { serverPort }
            };

            server.Start();

            string startupMessage = $"Started Boney server {processID} at hostname {hostname}:{port}";
            Console.WriteLine(startupMessage);

            //Configuring HTTP for client connections in Register method
            AppContext.SetSwitch(
  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            while (true) ;
        }

    }

}
