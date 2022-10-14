using Grpc.Net.Client;
using BankServer.domain;
using BankServer.utils;
using BankServer.services;
using Grpc.Core;

namespace BankServer
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            Logger.DebugOn();
            Logger.LogInfo("Bank Server started");
            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            BankManager bankManager = new BankManager();
            

            BankSlotManager bankSlotManager = new BankSlotManager(config, int.Parse(args[1]));
            SlotTimer sloTimer = new SlotTimer(bankSlotManager,(uint)config.GetSlotDuration(),config.GetSlotFisrtTime());
            sloTimer.Execute();
            

            List<string> servers = config.GetBoneyServersPortsAndAddresses();


            int processID = int.Parse(args[1]);
            (string hostname, int portNum) = config.GetBankHostnameAndPortByProcess(processID);
            Logger.LogDebug(hostname + ":" + portNum);
            ServerPort serverPort;
            serverPort = new ServerPort(hostname, portNum, ServerCredentials.Insecure);
            Server server = new Server
            {
                Services = {
                  CompareAndSwapService.BindService(new PaxosResultHandlerServiceImpl())

				         },
                Ports = { serverPort }
            };
            server.Start();





            //List<GrpcChannel> channels = new List<GrpcChannel>();
            //foreach(string address in servers)
            //{
            //    GrpcChannel channel = GrpcChannel.ForAddress("http://" + address);
            //    channels.Add(channel);
            //}
            while (true)
            {
                //var key = Console.ReadKey().Key;
                //(string address, int port) = config.GetBoneyHostnameAndPortByProcess(1);
                //uint leader = 0;
                //uint slot = 0;
                //if (key == ConsoleKey.A)
                //{
                //    leader = 1;
                //    slot = 0;
                //}
                //else if (key == ConsoleKey.B)
                //{
                //    leader = 2;
                //    slot = 1;
                //}

                //foreach (var channel in channels)
                //{
                //    CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                //    client.CompareAndSwapAsync(new CompareAndSwapReq { Leader = leader, Slot = slot, Address = hostname + ":" + portNum });
                //}
            }
        }
    }
}
