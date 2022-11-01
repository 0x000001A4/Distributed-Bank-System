using BankClient.utils;
using Grpc.Net.Client;

namespace BankClient.domain
{
    public class BankClientFrontend
    {
        ServerConfiguration _config;
        List<GrpcChannel> _channels;
        public BankClientFrontend(ServerConfiguration config)
        {
            _config = config;

            _channels = new List<GrpcChannel>();
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string host, int port) = _config.GetBankHostnameAndPortByProcess(id);
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + host + ":" + port);
                _channels.Add(channel);
            }
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
            List<DepositResp> responseReceived = new List<DepositResp>();
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                DepositReq request = new DepositReq { Client = protoClient, Amount = amount };
                Task ret = DepositAsync(request,client,responseReceived);
            }
            WaitForDeposit(responseReceived);
            Logger.LogDebug("Deposit Done with: " + responseReceived[0].Response);

        }

        public void Withdraw(uint clientID, uint opeSeqNumb, double amount)
        {
            List<WithdrawResp> responseReceived = new List<WithdrawResp>();
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                WithdrawReq request = new WithdrawReq { Client = protoClient, Amount = amount };
                Task ret = WithdrawAsync(request, client, responseReceived);

            }
            WaitForWithdraw(responseReceived);
            Logger.LogDebug("Withdraw Done with: " + responseReceived[0].Response);

        }

        public void ReadBalance(uint clientID, uint opeSeqNumb)
        {
            List<ReadResp> responseReceived = new List<ReadResp>();
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = clientID, ClientRequestSeqNumb = opeSeqNumb };
                ReadReq request = new ReadReq { Client = protoClient,};
                Task ret = ReadAsync(request, client, responseReceived);

            }
            WaitForRead(responseReceived);
            Logger.LogDebug("Read Done with Balance: " + responseReceived[0].Balance);
            
        }
    }
}
