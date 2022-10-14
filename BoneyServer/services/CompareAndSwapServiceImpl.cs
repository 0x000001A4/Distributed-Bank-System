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
            //if (!_state.IsFrozen()) {
                Logger.LogDebug("CompareAndSwapServiceImpl: CompareAndSwap received");
                doCompareAndSwap(request);
                Logger.LogDebug("End of CompareAndSwap");
                return Task.FromResult(new CompareAndSwapResp());
            //}
            // Request got queued and will be handled later
            //throw new Exception("The server is frozen.");
        }

        public void doCompareAndSwap(CompareAndSwapReq request) {
            try
            {
                PaxosValue value = new PaxosValue(request.Leader, request.Slot);
                Logger.LogDebug($"aqui");
                uint primary = _state.GetSlotManager().GetSlotValue((int)request.Slot);
                _multiPaxos.Start(value, request.Address, primary);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

        }

        // WHAT IS THIS USED FOR ?
        public override Task<CompareAndSwapResp> HandlePaxosResult(CompareAndSwapResp request, ServerCallContext context)
        {
            // Use request.primary to chose a primary for request.slot
            Logger.LogDebug($"HandlePaxosResult in boney");
            return Task.FromResult(request);
        }

    }
}