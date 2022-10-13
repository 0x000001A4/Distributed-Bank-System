using BoneyServer.domain;
using BoneyServer.domain.paxos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{

    public class PaxosAcceptorServiceImpl : PaxosAcceptorService.PaxosAcceptorServiceBase
    {
        private IMultiPaxos _multiPaxos;
        private BoneyServerState _state;

        public PaxosAcceptorServiceImpl(IMultiPaxos multiPaxos, BoneyServerState state)
        {
            _state = state;
            _multiPaxos = multiPaxos;
        }

        public override Task<PromiseResp> Prepare(PrepareReq request, ServerCallContext context) {
            if (!_state.IsFrozen()) {
                return Task.FromResult(doPrepare(request));
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public PromiseResp doPrepare(PrepareReq request) {
            Console.WriteLine("BONEY CompareAndSwapServiceImpl: Received CompareAndSwap message request");
            uint leaderNumber = request.LeaderNumber;
            uint instance = request.PaxosInstance;
            (PaxosValue, uint, uint, bool) tuplo = _multiPaxos.Promisse(leaderNumber, instance);
            if (tuplo.Item4)
            {
                uint ProcessID = tuplo.Item1.ProcessID;
                uint Slot = tuplo.Item1.Slot;
                CompareAndSwapReq value = new CompareAndSwapReq() { Leader = ProcessID, Slot = Slot };
                PromiseResp promiseResp = new PromiseResp() { Value = value, WriteTimeStamp = leaderNumber, PaxosInstance = instance, PromisseFlag = true };
                return promiseResp;
            }
            else
            {
                PromiseResp promiseResp = new PromiseResp() { PromisseFlag = false };
                return promiseResp;
            }
        }

		public override Task<AcceptedResp> Accept(AcceptReq request, ServerCallContext context) {
			if (!_state.IsFrozen()) {
                return Task.FromResult(doAccept(request));
            }
            // Message was queued and will he handled later
            throw new Exception("The server is frozen.");
        }

        public AcceptedResp doAccept(AcceptReq request) {
            Acceptor.LearnWork(request);
            return new AcceptedResp();
        }
	}
}
