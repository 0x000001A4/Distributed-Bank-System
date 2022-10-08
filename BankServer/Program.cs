using System;
using Grpc.Net.Client;

namespace BankServer
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            string serverHostname = "localhost";
            uint serverPort = 8001;
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
