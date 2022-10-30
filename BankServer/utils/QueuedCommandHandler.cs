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
        PaxosResultHandlerServiceImpl? _paxosResultHandler;
        ClientServiceImpl? _clientService;

        public void AddPaxosResultHandlerService(PaxosResultHandlerServiceImpl paxosResultHandler)
        {
            _paxosResultHandler = paxosResultHandler;
        }

        public void AddClientService(ClientServiceImpl clientService)
        {
            _clientService = clientService;
        }

        public void handlePaxosResult(CompareAndSwapResp paxosResult, string sender)
        {
            if (_paxosResultHandler == null)
            {
                Console.WriteLine("Unexpected behaviour in handlePaxosResult function: _paxosResultHandler == null (QueuedCommandHandler.cs : Line 22)");
                throw new Exception();
            }
            _ = _paxosResultHandler.doHandlePaxosResult(paxosResult);
        }

        public void handleDepositReq(DepositReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour in handlePaxosResult function: _paxosResultHandler == null (QueuedCommandHandler.cs : Line 22)");
                throw new Exception();
            }
            _ = _clientService.doDeposit(request);
        }

        public void handleWithdrawReq(WithdrawReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour in handlePaxosResult function: _paxosResultHandler == null (QueuedCommandHandler.cs : Line 22)");
                throw new Exception();
            }
            _ = _clientService.doWithdraw(request);
        }

        public void handleReadReq(ReadReq request, string sender)
        {
            if (_clientService == null)
            {
                Console.WriteLine("Unexpected behaviour in handlePaxosResult function: _paxosResultHandler == null (QueuedCommandHandler.cs : Line 22)");
                throw new Exception();
            }
            _ = _clientService.doRead(request);
        }
    }
}
