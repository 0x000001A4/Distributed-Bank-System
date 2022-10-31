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


        public static async Task DepositAsync(DepositReq request, ClientService.ClientServiceClient client, List<DepositResp> responseReceived)
        {

            DepositResp response = await client.DepositAsync(request);
            responseReceived.Add(response);
 
        }

        public static async Task WithdrawAsync(WithdrawReq request, ClientService.ClientServiceClient client, List<WithdrawResp> responseReceived)
        {

            WithdrawResp response = await client.WithdrawAsync(request);
            responseReceived.Add(response);

        }

        public static async Task ReadAsync(ReadReq request, ClientService.ClientServiceClient client, List<ReadResp> responseReceived)
        {

            ReadResp response = await client.ReadBalanceAsync(request);
            responseReceived.Add(response);

        }

        private static void WaitForDeposit(List<DepositResp> responseReceived)
        {
            while (responseReceived.Count() == 0) ;
        }

        private static void WaitForWithdraw(List<WithdrawResp> responseReceived)
        {
            while (responseReceived.Count() == 0) ;
        }

        private static void WaitForRead(List<ReadResp> responseReceived)
        {
            while (responseReceived.Count() == 0) ;
        }
        public void Deposit(uint clientID, uint opeSeqNumb,double amount)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            List<DepositResp> responseReceived = new List<DepositResp>();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                string address = "http://" + bankHost + ":" + bankPort;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                DepositReq request = new DepositReq { Client = protoClient, Amount = amount,Address = address };
                Task ret = DepositAsync(request,client,responseReceived);
            }
            WaitForDeposit(responseReceived);
            Logger.LogDebug("Deposit Done with: " + responseReceived[0].Response);

        }

        public void Withdraw(uint clientID, uint opeSeqNumb, double amount)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            List<WithdrawResp> responseReceived = new List<WithdrawResp>();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                string address = "http://" + bankHost + ":" + bankPort;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                WithdrawReq request = new WithdrawReq { Client = protoClient, Amount = amount };
                WithdrawAsync(request, client, responseReceived);

            }
            WaitForWithdraw(responseReceived);
            Logger.LogDebug("Withdraw Done with: " + responseReceived[0].Response);

        }

        public void ReadBalance(uint clientID, uint opeSeqNumb)
        {
            List<int> bankAdresses = _config.GetBankServerIDs();
            List<ReadResp> responseReceived = new List<ReadResp>();
            foreach (int id in bankAdresses)
            {
                (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                string address = "http://" + bankHost + ":" + bankPort;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                ReadReq request = new ReadReq { Client = protoClient,};
                ReadAsync(request, client, responseReceived);

            }
            WaitForRead(responseReceived);
            Logger.LogDebug("Read Done with Balance: " + responseReceived[0].Balance);
            
        }
    }
}
