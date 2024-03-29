﻿using BankServer.utils;
using Grpc.Core;
using BankServer.domain.bank;

namespace BankServer.services
{
    public class PaxosResultHandlerServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        BankServerState _state;
        private object _lock;

        public PaxosResultHandlerServiceImpl(BankServerState state, object __lock)
        {
            _state = state;
            _lock = __lock;
        }

        public override Task<HandlePaxosResultResp> HandlePaxosResult(CompareAndSwapResp request, ServerCallContext context)
        {
            if (!_state.IsFrozen()) {
                HandlePaxosResultResp _res = doHandlePaxosResult(request);
                Logger.LogDebug("End of HandlePaxosResult");
                return Task.FromResult(_res);
            }
            throw new Exception("The server is frozen!");
        }

        public HandlePaxosResultResp doHandlePaxosResult(CompareAndSwapResp request)
        {
            Logger.LogDebug($"Bank Server compareAndSwap response:  Elected ( Primary: {request.Primary}, Slot: {request.Slot})");
            uint _prevPrimary = _state.GetSlotManager().GetPrimaryOnSlot(request.Slot);
            uint _primary = request.Primary;
            if (_prevPrimary > 0 && _prevPrimary != _primary) {
                Logger.LogDebug($"Starting cleanup | previous primary: {_prevPrimary} ; current primary: {_primary}");
                _state.Cleanup();
            }
            _state.GetSlotManager().SetPrimaryOnSlot(request.Slot, _primary);

            lock (_lock) { Monitor.Pulse(_lock); }

            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();
            if (currentSlot > 1 && currentSlot < _state.GetSlotManager().GetMaxSlots() && _state.hasUnfrozed()) {
                _state.HandleQueuedMessages();
            }

            return new HandlePaxosResultResp() { };
        }

        public override Task<HandlePaxosResultResp> AckHandlePaxosResult(HandlePaxosResultResp response, ServerCallContext context) {
            return Task.FromResult(response);
        }
    }
}
