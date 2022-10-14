using Grpc.Net.Client;
using BankServer.domain;
using BankServer.utils;

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
            //sloTimer.Execute();
            

            List<string> servers = config.GetBoneyServersPortsAndAddresses();
            List<GrpcChannel> channels = new List<GrpcChannel>();
            foreach(string address in servers)
            {
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + address);
            }
            
            while (true)
            {
                Console.ReadKey();
                (string address, int port) = config.GetBoneyHostnameAndPortByProcess(1);
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + address + ":" + port);
                CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                client.CompareAndSwapAsync(new CompareAndSwapReq { Leader = 1, Slot = 0 });

                //foreach(var channel in channels)
                //{
                //    CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                //    client.CompareAndSwapAsync(new CompareAndSwapRequest { Leader = 1, Slot = 0 });
                //}

                Logger.LogDebug("CompareAndSwap sent");
            }
        }
    }
}
