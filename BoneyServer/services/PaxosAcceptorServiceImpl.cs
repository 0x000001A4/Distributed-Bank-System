using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{

    internal class PaxosAcceptorServiceImpl : PaxosAcceptorService.PaxosAcceptorServiceBase
    {
        private IMultiPaxos _multiPaxos;

        public PaxosAcceptorServiceImpl(IMultiPaxos multiPaxos)
        {
            _multiPaxos = multiPaxos;
        }

        public override Task<PromiseResp> Prepare(PrepareReq request, ServerCallContext context)
        {
            Logger.LogDebug("BONEY CompareAndSwapServiceImpl: Received CompareAndSwap message request");
            uint leaderNumber = request.LeaderNumber;
            uint instance = request.PaxosInstance;
            (PaxosValue, uint, uint, bool)  tuplo = _multiPaxos.Promisse(leaderNumber,instance);
            if (tuplo.Item4)
            {
                uint ProcessID = tuplo.Item1.ProcessID;
                uint Slot = tuplo.Item1.Slot;
                CompareAndSwapReq value = new CompareAndSwapReq() { Leader = ProcessID, Slot = Slot };
                return Task.FromResult(new PromiseResp() { Value = value, WriteTimeStamp = leaderNumber, PaxosInstance = instance, PromisseFlag = true });
            }
            else
            {
                return Task.FromResult(new PromiseResp() { PromisseFlag = false });
            }

            
        }

		public override Task<AcceptedResp> Accept(AcceptReq request, ServerCallContext context)
		{
			Acceptor.LearnWork(request);
			return Task.FromResult(new AcceptedResp(/* Send accepted information */));
		}
	}
}
