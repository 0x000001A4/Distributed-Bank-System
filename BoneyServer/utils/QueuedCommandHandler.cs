using BoneyServer.services;
using BoneyServer.domain;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using BoneyServer.domain.paxos;

namespace BoneyServer.utils
{
    public class QueuedCommandHandler {
        CompareAndSwapServiceImpl? _casService;
        PaxosAcceptorServiceImpl? _paxosAcceptorService;
        PaxosLearnerServiceImpl? _paxosLearnerService;
        IMultiPaxos _multiPaxos;

        public QueuedCommandHandler(IMultiPaxos multiPaxos) {
            _multiPaxos = multiPaxos;
        }


        public void AddCompareAndSwapService(CompareAndSwapServiceImpl casService) {
            _casService = casService;
        }

        public void AddPaxosAcceptorService(PaxosAcceptorServiceImpl paxosAcceptorService) {
            _paxosAcceptorService = paxosAcceptorService;
        }

        public void AddPaxosLearnerService(PaxosLearnerServiceImpl paxosLearnerService) {
            _paxosLearnerService = paxosLearnerService;
        }

        public void handleCompareAndSwap(CompareAndSwapReq request) {
            if (_casService == null) {
                Console.WriteLine("Unexpected behaviour in handleCompareAndSwap function: _casService == null (QueuedCommandHandler.cs : Line 27)");
                throw new Exception();
            }
            _casService.doCompareAndSwap(request);
            // Response will be sent to banks inside this function (When learners learn the value from majority of acceptors)
        }

        public void handlePrepare(PrepareReq request, string sender) {
            if (_paxosAcceptorService == null) {
                Console.WriteLine("Unexpected behaviour in handlePrepare function: _paxosAcceptorService == null (QueuedCommandHandler.cs : Line 36)");
                throw new Exception();
            }
            PromiseResp promise = _paxosAcceptorService.doPrepare(request);
            PaxosAcceptorService.PaxosAcceptorServiceClient _client =
                new PaxosAcceptorService.PaxosAcceptorServiceClient(GrpcChannel.ForAddress(sender));
            _client.SendPromise(promise);
        }

        public void handleAccept(AcceptReq request, string sender) {
            if (_paxosAcceptorService == null) {
                Console.WriteLine("Unexpected behaviour in handleAccept function: _paxosAcceptorService == null (QueuedCommandHandler.cs : Line 43)");
                throw new Exception();
            }
            AcceptedResp acceptedResponse = _paxosAcceptorService.doAccept(request);
            PaxosAcceptorService.PaxosAcceptorServiceClient _client =
                new PaxosAcceptorService.PaxosAcceptorServiceClient(GrpcChannel.ForAddress(sender));
            _client.SendAccepted(acceptedResponse);
        }

        public void handleLearnCommand(LearnCommandReq request, string sender) {
            if (_paxosLearnerService == null)
            {
                Console.WriteLine("Unexpected behaviour in handleLearnCommand function: _paxosLearnerService == null (QueuedCommandHandler.cs : Line 51)");
                throw new Exception();
            }
            LearnCommandResp learnCommandResponse = _paxosLearnerService.doLearnCommand(request);
            PaxosLearnerService.PaxosLearnerServiceClient _client =
                new PaxosLearnerService.PaxosLearnerServiceClient(GrpcChannel.ForAddress(sender));
            _client.SendLearnCommandResponse(learnCommandResponse);
        }
    }
}
