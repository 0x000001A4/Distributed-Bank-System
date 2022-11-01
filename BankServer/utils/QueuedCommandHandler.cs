using BankServer.services;
using BankServer.domain;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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
                new CompareAndSwapService.CompareAndSwapServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckHandlePaxosResult(response);
        }

        public void handleDepositReq(DepositReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            DepositResp response = _clientService.doDeposit(request);
            ClientService.ClientServiceClient _client = new ClientService.ClientServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckDeposit(response);
        }

        public void handleWithdrawReq(WithdrawReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            WithdrawResp response = _clientService.doWithdraw(request);
            ClientService.ClientServiceClient _client = new ClientService.ClientServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckWithdraw(response);
        }

        public void handleReadReq(ReadReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            ReadResp response = _clientService.doRead(request);
            ClientService.ClientServiceClient _client = new ClientService.ClientServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckReadBalance(response);
        }

        public void handleListPendingRequestsReq(ListPendingRequestsReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            ListPendingRequestsResp response = _bankService.doListPendingRequests(request);
            BankService.BankServiceClient _client = new BankService.BankServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckListPendingRequests(response);
        }

        public void handleProposeReq(ProposeReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            ProposeResp response = _bankService.doPropose(request);
            BankService.BankServiceClient _client = new BankService.BankServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckPropose(response);
        }

        public void handleCommitReq(CommitReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour");
                throw new Exception();
            }
            CommitResp response = _bankService.doCommit(request);
            BankService.BankServiceClient _client = new BankService.BankServiceClient(GrpcChannel.ForAddress(sender));
            _client.AckCommit(response);
        }
    }
}
