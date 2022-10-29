using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankClient.utils;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace BankClient.domain
{
    internal class BankClientFrontEnd
    {
        ServerConfiguration _config;
        public BankClientFrontEnd(ServerConfiguration config)
        {
            _config = config;
        }

        public string ExecuteDeposit(int clientID,int opeSeqNumb,double amount)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                //Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Logger.LogDebug("CompareAndSwap sent");
                Client protoClient = new Client { ClientID = (uint)clientID, ClientRequestSeqNumb = (uint) opeSeqNumb };
                DepositReq request = new DepositReq { Client = protoClient, Amount = amount };
                DepositResp response = client.Deposit(request);
                return response.Response;
            }
            return "";

        }

        public string ExecuteWithDraw(int clientID, int opeSeqNumb, double amount)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                //Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Logger.LogDebug("Withdraw sent");
                Client protoClient = new Client { ClientID = (uint)clientID, ClientRequestSeqNumb = (uint)opeSeqNumb };
                WithdrawReq request = new WithdrawReq { Client = protoClient, Amount = amount };
                WithdrawResp response = client.Withdraw(request);
                return response.Response;
            }
            return "";

        }

        public double ExecuteRead(int clientID, int opeSeqNumb)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                //Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Logger.LogDebug("Withdraw sent");
                Client protoClient = new Client { ClientID = (uint)clientID, ClientRequestSeqNumb = (uint)opeSeqNumb };
                ReadReq request = new ReadReq { Client = protoClient,};
                ReadResp response = client.Read(request);
                return response.Balance;
            }
            return -1;
        }
    }
}
