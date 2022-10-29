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

        public void AddPaxosResultHandlerService(PaxosResultHandlerServiceImpl paxosResultHandler)
        {
            _paxosResultHandler = paxosResultHandler;
        }

        public void handlePaxosResult(CompareAndSwapResp request, string sender)
        {
            if (_paxosResultHandler == null)
            {
                Console.WriteLine("Unexpected behaviour in handlePaxosResult function: _paxosResultHandler == null (QueuedCommandHandler.cs : Line 22)");
                throw new Exception();
            }
            CompareAndSwapResp paxosResultHandlerResp = _paxosResultHandler.doHandlePaxosResult(request);
            /*
            CompareAndSwapService.CompareAndSwapServiceClient _client =
                new CompareAndSwapService.CompareAndSwapServiceClient(GrpcChannel.ForAddress(sender));
            _client.SendHandlePaxosResultResponse(paxosResultHandlerResp);*/
        }
    }
}
