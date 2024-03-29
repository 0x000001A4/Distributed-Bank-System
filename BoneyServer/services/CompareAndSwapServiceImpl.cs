﻿using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;

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
            Logger.LogDebug("CompareAndSwap received.");
            Logger.LogDebug(_state.IsFrozen().ToString());
            if (!_state.IsFrozen()) {
                doCompareAndSwap(request);
                return Task.FromResult(doCompareAndSwap(request));
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public CompareAndSwapResp doCompareAndSwap(CompareAndSwapReq request) {
            PaxosValue value = new PaxosValue(request.Leader, request.Slot);
            Logger.LogDebug("CompareAndSwapServiceImpl: CompareAndSwap received (CompareAndSwapServiceImpl.cs: Line 36)");
            uint primary = _state.GetSlotManager().GetSlotValue((int)request.Slot);
            _multiPaxos.Start(value, request.Sender, primary);
            Logger.LogDebug("End of CompareAndSwap");
            return new CompareAndSwapResp() { Sender = _state.GetHostname() };
        }
    
        public override Task<CompareAndSwapResp> AckCompareAndSwap(CompareAndSwapResp response, ServerCallContext context) {
            return Task.FromResult(response);
        }
    }
}