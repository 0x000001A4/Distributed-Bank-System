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
           /* if (_serverState.isFrozen()) return Task.FromResult(new CompareAndSwapResp { Ok = false }); */
            doCompareAndSwap(request);
            return Task.FromResult(new CompareAndSwapResp());
        }


        public void doCompareAndSwap(CompareAndSwapReq request) {
            Logger.LogDebug("CompareAndSwapServiceImpl: CompareAndSwap received");
            PaxosValue value = new PaxosValue(request.Leader, request.Slot);
            _multiPaxos.Start(value,request.Address,_state.GetSlotManager().GetSlotValue((int)request.Slot));
        }
    }
}