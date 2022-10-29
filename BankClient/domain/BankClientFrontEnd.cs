using BankClient.utils;
using Grpc.Net.Client;

namespace BankClient.domain
{
    public class BankClientFrontend
    {
        ServerConfiguration _config;
        public BankClientFrontend(ServerConfiguration config)
        {
            _config = config;
        }

        public string Deposit(uint clientID, uint opeSeqNumb,double amount)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            Logger.LogInfo(bankAdresses.ToString());
            foreach (int id in bankAdresses) // TODO: Sees only bank with port 10004 why ??
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                DepositReq request = new DepositReq { Client = protoClient, Amount = amount };
                DepositResp response = client.Deposit(request);
                return response.Response;
            }
            return "";

        }

        public string Withdraw(uint clientID, uint opeSeqNumb, double amount)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                WithdrawReq request = new WithdrawReq { Client = protoClient, Amount = amount };
                WithdrawResp response = client.Withdraw(request);
                return response.Response;
            }
            return "";

        }

        public double ReadBalance(uint clientID, uint opeSeqNumb)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                ReadReq request = new ReadReq { Client = protoClient,};
                ReadResp response = client.ReadBalance(request);
                return response.Balance;
            }
            return -1;
        }
    }
}
