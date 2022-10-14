using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
    public class CompareAndSwapServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        private IMultiPaxos _multiPaxos;
        private BoneyServerState _state;
        public CompareAndSwapServiceImpl(BoneyServerState state,IMultiPaxos multiPaxos) {
            _multiPaxos = multiPaxos;
            _state = state;
         
        }

        public override Task<CompareAndSwapResp> CompareAndSwap(CompareAndSwapReq request, ServerCallContext context) {
            if (!_state.IsFrozen()) {
                return Task.FromResult(doCompareAndSwap(request));
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public CompareAndSwapResp doCompareAndSwap(CompareAndSwapReq request) {
            Console.WriteLine("BONEY CompareAndSwapServiceImpl: Received CompareAndSwap message request");
            PaxosValue value = new PaxosValue(request.Leader, request.Slot);
            _multiPaxos.Start(value,request.Address,_state.GetSlotManager().GetSlotValue((int)request.Slot));
            return new CompareAndSwapResp();
        }
    }
}