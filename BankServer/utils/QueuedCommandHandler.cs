using BankServer.services;
using Grpc.Net.Client;

namespace BankServer.utils
{
    public class QueuedCommandHandler
    {
        PaxosResultHandlerServiceImpl? _paxosResultHandlerService;
        ClientServiceImpl? _clientService;
        BankServiceImpl? _bankService; 

        public void AddPaxosResultHandlerService(PaxosResultHandlerServiceImpl paxosResultHandler)
        {
            _paxosResultHandlerService = paxosResultHandler;
        }

        public void AddClientService(ClientServiceImpl clientService)
        {
            _clientService = clientService;
        }

        public void AddBankService(BankServiceImpl bankService)
        {
            _bankService = bankService;
        }

        public void handlePaxosResult(CompareAndSwapResp paxosResult, string sender)
        {
            if (_paxosResultHandlerService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            HandlePaxosResultResp response = _paxosResultHandlerService.doHandlePaxosResult(paxosResult);
            CompareAndSwapService.CompareAndSwapServiceClient _client =
                new CompareAndSwapService.CompareAndSwapServiceClient(GrpcChannel.ForAddress("http://" + sender));
            //_client.AckHandlePaxosResultAsync(response);
        }

        public void handleDepositReq(DepositReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            Thread thread = new Thread(() => doDeposit(request, sender));
            thread.Start();
        }

        private void doDeposit(DepositReq request, string sender)
        {
            DepositResp response = _clientService.doDeposit(request);
            ClientService.ClientServiceClient _client = new ClientService.ClientServiceClient(GrpcChannel.ForAddress("http://" + sender));
            _client.AckDeposit(response);
        }

        public void handleWithdrawReq(WithdrawReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            Thread thread = new Thread(() => doWithdraw(request, sender));
            thread.Start();
        }

        private void doWithdraw(WithdrawReq request, string sender)
        {
            WithdrawResp response = _clientService.doWithdraw(request);
            ClientService.ClientServiceClient _client = new ClientService.ClientServiceClient(GrpcChannel.ForAddress("http://" + sender));
            Logger.LogDebug("sender is: " + sender);
            _client.AckWithdraw(response);
        }

        public void handleReadReq(ReadReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            Thread thread = new Thread(() => doRead(request, sender));
            thread.Start();
        }
        private void doRead(ReadReq request, string sender)
        {
            ReadResp response = _clientService.doRead(request);
            ClientService.ClientServiceClient _client = new ClientService.ClientServiceClient(GrpcChannel.ForAddress("http://" + sender));
            _client.AckReadBalance(response);
        }

        public void handleListPendingRequestsReq(ListPendingRequestsReq request, string sender)
        {
            if (_bankService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            ListPendingRequestsResp response = _bankService.doListPendingRequests(request);
            BankService.BankServiceClient _client = new BankService.BankServiceClient(GrpcChannel.ForAddress("http://" + sender));
            _client.AckListPendingRequests(response);
        }

        public void handleProposeReq(ProposeReq request, string sender)
        {
            if (_bankService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            ProposeResp response = _bankService.doPropose(request);
            BankService.BankServiceClient _client = new BankService.BankServiceClient(GrpcChannel.ForAddress("http://" + sender));
            _client.AckPropose(response);
        }

        public void handleCommitReq(CommitReq request, string sender)
        {
            if (_bankService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            CommitResp response = _bankService.doCommit(request);
            BankService.BankServiceClient _client = new BankService.BankServiceClient(GrpcChannel.ForAddress("http://" + sender));
            _client.AckCommit(response);
        }
    }
}
