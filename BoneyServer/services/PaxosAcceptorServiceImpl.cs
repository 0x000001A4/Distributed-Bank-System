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
            Logger.LogDebug("PaxosAcceptorServiceImpl: Received PROMISE request");
            uint leaderNumber = request.LeaderNumber;
            uint instance = request.PaxosInstance;
            (PaxosValue value, bool ack) = _multiPaxos.Promisse(leaderNumber,instance);

            if (value == null) // if no value was chosen yet
            {
                return Task.FromResult(new PromiseResp() { WriteTimeStamp = leaderNumber, PaxosInstance = instance, PromisseFlag = ack });
            }
            else
            {
                uint processID = value.ProcessID;
                uint Slot = value.Slot;
                CompareAndSwapReq valueToSend = new CompareAndSwapReq() { Leader = processID, Slot = Slot };
                return Task.FromResult(new PromiseResp() { Value = valueToSend, WriteTimeStamp = leaderNumber, PaxosInstance = instance, PromisseFlag = ack });
            }

        }

		public override Task<AcceptedResp> Accept(AcceptReq request, ServerCallContext context)
		{
            Logger.LogDebug("PaxosAcceptorServiceImpl: Received ACCEPT! request");
			Acceptor.LearnWork(request);
			return Task.FromResult(new AcceptedResp(/* Send accepted information */));
		}
	}
}
