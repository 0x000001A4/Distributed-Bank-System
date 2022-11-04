using BankClient.utils;
using Grpc.Net.Client;

namespace BankClient.domain
{
    public class BankClientFrontend
    {
        ServerConfiguration _config;
        List<GrpcChannel> _channels;
        int _clientId;
        string _clientHostname;
        List<DepositResp> _depositResponseReceived = new List<DepositResp>();
        List<WithdrawResp> _withdrawResponseReceived = new List<WithdrawResp>();
        List<ReadResp> _readBalanceResponseReceived = new List<ReadResp>();

        public void AddDepoitResponse(DepositResp response)
        {
            _depositResponseReceived.Add(response);
        }

        public void AddWithdrawResponse(WithdrawResp response)
        {
            _withdrawResponseReceived.Add(response);
        }

        public void AddReadBalanceResponse(ReadResp response)
        {
            _readBalanceResponseReceived.Add(response);
        }

        public BankClientFrontend(ServerConfiguration config, int clientId)
        {
            _config = config;
            _clientId = clientId;
            (string clienthost, int clientport) = _config.GetClientHostnameAndPortByProcess(_clientId);
            _clientHostname = clienthost + ":" + clientport;

            _channels = new List<GrpcChannel>();
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string host, int port) = _config.GetBankHostnameAndPortByProcess(id);
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + host + ":" + port);
                _channels.Add(channel);
            }
        }


        public async Task DepositAsync(DepositReq request, ClientService.ClientServiceClient client)
        {

            DepositResp response = await client.DepositAsync(request);
            lock (this)
            {
                _depositResponseReceived.Add(response);
                Monitor.Pulse(this);
            }
 
        }

        public async Task WithdrawAsync(WithdrawReq request, ClientService.ClientServiceClient client)
        {

            WithdrawResp response = await client.WithdrawAsync(request);
            lock(this)
            {
                _withdrawResponseReceived.Add(response);
                Monitor.Pulse(this);
            }

        }

        public async Task ReadAsync(ReadReq request, ClientService.ClientServiceClient client)
        {

            ReadResp response = await client.ReadBalanceAsync(request);
            lock(this)
            {
                _readBalanceResponseReceived.Add(response);
                Monitor.Pulse(this);
            }

        }


        public void Deposit(uint clientID, uint opeSeqNumb,double amount)
        {
            
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);
                Client protoClient = new Client { ClientID = (int)clientID, ClientRequestSeqNumb = opeSeqNumb };
                DepositReq request = new DepositReq { Client = protoClient, Amount = amount , Sender = _clientHostname };
                Task ret = DepositAsync(request,client);
            }
            if (!WaitForResponse<DepositResp>(_depositResponseReceived))
            {
                throw new Exception("Timed out. A majority of Boney servers is frozen, please try again later.");
            }
            else
            {
                Logger.LogDebug("Deposit Done with: " + _depositResponseReceived[0].Response);
                _depositResponseReceived.Clear();
            }
        }

        public void Withdraw(uint clientID, uint opeSeqNumb, double amount)
        {
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = (int)clientID, ClientRequestSeqNumb = opeSeqNumb };
                WithdrawReq request = new WithdrawReq { Client = protoClient, Amount = amount, Sender = _clientHostname };
                Task ret = WithdrawAsync(request, client);

            }
            if (!WaitForResponse<WithdrawResp>(_withdrawResponseReceived))
            {
                throw new Exception("Timed out. A majority of Boney servers is frozen, please try again later.");
            }
            else
            {
                Logger.LogDebug("Withdraw Done with: " + _withdrawResponseReceived[0].Response);
                _withdrawResponseReceived.Clear();
            }
            

        }



        public void ReadBalance(uint clientID, uint opeSeqNumb)
        {
            foreach (GrpcChannel channel in _channels)
            {
                Logger.LogDebug($"Sending to {channel.Target}");
                ClientService.ClientServiceClient client = new ClientService.ClientServiceClient(channel);

                Client protoClient = new Client { ClientID = (int)clientID, ClientRequestSeqNumb = opeSeqNumb };
                ReadReq request = new ReadReq { Client = protoClient, Sender = _clientHostname };
                Task ret = ReadAsync(request, client);

            }
            if (!WaitForResponse<ReadResp>(_readBalanceResponseReceived))
            {
                throw new Exception("Timed out. A majority of Bank servers is frozen, please try again later.");
            }
            else
            {
                Logger.LogDebug("Read Done with Balance: " + _readBalanceResponseReceived[0].Balance);
                _readBalanceResponseReceived.Clear();
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
