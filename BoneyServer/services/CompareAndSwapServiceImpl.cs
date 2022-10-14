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
        public CompareAndSwapServiceImpl(BoneyServerState state, IMultiPaxos multiPaxos) {
            _multiPaxos = multiPaxos;
            _state = state;
         
        }

        public override Task<CompareAndSwapResp> CompareAndSwap(CompareAndSwapReq request, ServerCallContext context) {

            if (!_state.IsFrozen()) {
                doCompareAndSwap(request);
                Logger.LogDebug("End of CompareAndSwap");
                return Task.FromResult(new CompareAndSwapResp());
            //}
            // Request got queued and will be handled later
            //throw new Exception("The server is frozen.");
        }

        public void doCompareAndSwap(CompareAndSwapReq request) {
                PaxosValue value = new PaxosValue(request.Leader, request.Slot);
                Logger.LogDebug("CompareAndSwapServiceImpl: CompareAndSwap received (CompareAndSwapServiceImpl.cs: Line 36)");
                uint primary = _state.GetSlotManager().GetSlotValue((int)request.Slot);
                _multiPaxos.Start(value, request.Address, primary);
            }

    }
}