using BankServer.utils;
using Grpc.Core;
using BankServer.domain.bank;

namespace BankServer.services
{
    public class PaxosResultHandlerServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        BankServerState _state;

        public PaxosResultHandlerServiceImpl(BankServerState state)
        {
            _state = state;
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
            if (_prevPrimary != _primary) {
                _state.Cleanup();
            }
            _state.GetSlotManager().SetPrimaryOnSlot(request.Slot, _primary);
            return new HandlePaxosResultResp() { };
        }
    }
}
