using System;
using Grpc.Net.Client;
using BankServer.utils;



namespace BankServer
{
    public class Program
    {
        public static void Main(string[] args) 
        {

            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            string serverHostname = "localhost";
            uint serverPort = 2;
            GrpcChannel channel = GrpcChannel.ForAddress("http://" + serverHostname + ":" + serverPort.ToString());

            CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
            while (true)
            {
                Console.ReadKey();
                client.CompareAndSwap(new CompareAndSwapRequest { Leader = 1, Slot = 0 }) ;
            }
        }
    }
}
