using System;
using Grpc.Net.Client;
using BankServer.domain;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using BankServer.utils;

namespace BankServer
{
    public class Program
    {

        

        public static void Main(string[] args) 
        {
            Logger.DebugOn();
            Logger.LogInfo("Initializing Bank server ... ");
            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            BankManager bankManager = new BankManager();
            BankSlotManager bankSlotManager = new BankSlotManager(config);
            SlotTimer sloTimer = new SlotTimer(bankSlotManager,(uint)config.GetSlotDuration(),config.GetSlotFisrtTime());
            sloTimer.Execute();
            

            string serverHostname = "localhost";
            uint serverPort = 2;
            GrpcChannel channel = GrpcChannel.ForAddress("http://" + serverHostname + ":" + serverPort.ToString());

            CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
            while (true)
            {
                Console.ReadKey();
                Logger.LogDebug("CompareAndSwap sent");
                client.CompareAndSwapAsync(new CompareAndSwapRequest { Leader = 1, Slot = 0 }) ;
            }
        }
    }
}
