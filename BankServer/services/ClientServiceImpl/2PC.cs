using Grpc.Net.Client;
using BankServer.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;

namespace BankServer.domain
{
    public class Propose2PC
    {

        public static void executePropose(uint processID, uint slot, ServerConfiguration config, int seqNumber, string address)
        {
            List<int> bankAdresses = config.GetBankServerIDs();
            List<AcceptResp> responsePropose = new List<AcceptResp>();
            foreach (int id in bankAdresses)
            {

                (string bankHost, int bankPort) = config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                BankService.BankServiceClient client = new BankService.BankServiceClient(channel);


                ProposeReq request = new ProposeReq { Slot = processID, ProcessId = processID, SeqNumber = (uint)seqNumber };
                ProposeAsync(request, client, responsePropose);



            }
            WaitForAccepts(responsePropose, config.GetNumberOfBankServers());
            sendCommit(config, seqNumber, address);


        }


        private static async Task ProposeAsync(ProposeReq request, BankService.BankServiceClient client, List<AcceptResp> responsePropose)
        {

            AcceptResp response = await client.ProposeAsync(request);
            responsePropose.Add(response);

        }

        private static void WaitForAccepts(List<AcceptResp> responsePropose,int numberOfBanks)
        {
            while (responsePropose.Count() < Math.Ceiling((decimal)numberOfBanks / 2)) ;
        }

        private static void sendCommit(ServerConfiguration config,int seqNumber, string Address)
        {
            List<int> bankAdresses = config.GetBankServerIDs();
            foreach (int id in bankAdresses) 
            {

                (string bankHost, int bankPort) = config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                BankService.BankServiceClient client = new BankService.BankServiceClient(channel);
                CommitReq request = new CommitReq { SeqNumber = (uint)seqNumber, Address = Address};
                try
                {
                    client.CommitAsync(request);
                }
                catch (Exception e)
                {
                    throw new Exception();
                }
            }
        }
    }
}

