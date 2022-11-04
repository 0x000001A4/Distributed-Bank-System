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


        public async Task DepositAsync(DepositReq request, ClientService.ClientServiceClient client, List<DepositResp> responseReceived)
        {

            DepositResp response = await client.DepositAsync(request);
            lock (this)
            {
                responseReceived.Add(response);
                Monitor.Pulse(this);
            }
 
        }

        public async Task WithdrawAsync(WithdrawReq request, ClientService.ClientServiceClient client, List<WithdrawResp> responseReceived)
        {

            WithdrawResp response = await client.WithdrawAsync(request);
            lock(this)
            {
                responseReceived.Add(response);
                Monitor.Pulse(this);
            }

        }

        public async Task ReadAsync(ReadReq request, ClientService.ClientServiceClient client, List<ReadResp> responseReceived)
        {

            ReadResp response = await client.ReadBalanceAsync(request);
            lock(this)
            {
                responseReceived.Add(response);
                Monitor.Pulse(this);
            }

        }


        public void Deposit(uint clientID, uint opeSeqNumb,double amount)
        {
            List<DepositResp> responseReceived = new List<DepositResp>();
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = (int)clientID, ClientRequestSeqNumb = opeSeqNumb };
                DepositReq request = new DepositReq { Client = protoClient, Amount = amount };
                Task ret = DepositAsync(request,client,responseReceived);
            }
            if (!WaitForResponse<DepositResp>(responseReceived))
            {
                throw new Exception("Timed out. A majority of Boney servers is frozen, please try again later.");
            }
            else
            {
                Logger.LogDebug("Deposit Done with: " + responseReceived[0].Response);
            }
        }

        public void Withdraw(uint clientID, uint opeSeqNumb, double amount)
        {
            List<WithdrawResp> responseReceived = new List<WithdrawResp>();
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = (int)clientID, ClientRequestSeqNumb = opeSeqNumb };
                WithdrawReq request = new WithdrawReq { Client = protoClient, Amount = amount };
                Task ret = WithdrawAsync(request, client, responseReceived);

            }
            if (!WaitForResponse<WithdrawResp>(responseReceived))
            {
                throw new Exception("Timed out. A majority of Boney servers is frozen, please try again later.");
            }
            else
            {
                Logger.LogDebug("Withdraw Done with: " + responseReceived[0].Response);
            }
            

        }



        public void ReadBalance(uint clientID, uint opeSeqNumb)
        {
            List<ReadResp> responseReceived = new List<ReadResp>();
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = (int)clientID, ClientRequestSeqNumb = opeSeqNumb };
                ReadReq request = new ReadReq { Client = protoClient,};
                Task ret = ReadAsync(request, client, responseReceived);

            }
            if (!WaitForResponse<ReadResp>(responseReceived))
            {
                throw new Exception("Timed out. A majority of Bank servers is frozen, please try again later.");
            }
            else
            {
                Logger.LogDebug("Read Done with Balance: " + responseReceived[0].Balance);
            }
            
            
        }

        private bool WaitForResponse<TResponse>(List<TResponse> responseReceived)
        {
            lock (this)
            {
                while (responseReceived.Count() == 0)
                {
                    Monitor.Wait(this);
                }
                return true;
            }
        }
    }
}
