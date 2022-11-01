using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    public partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public override Task<WithdrawResp> Withdraw(WithdrawReq request, ServerCallContext context)
        {
            Logger.LogDebug("Withdraw received.");
            Logger.LogDebug(_state.IsFrozen().ToString());
            if (!_state.IsFrozen())
            {
                WithdrawResp response = doWithdraw(request);
                Logger.LogDebug("End of Withdraw");
                return Task.FromResult(response);
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public WithdrawResp doWithdraw(WithdrawReq request)
        {
            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();
            while (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == 0) ;

            if (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == _state.GetProcessId()) {
                Logger.LogInfo("CURRENT slott " + currentSlot);
                Thread thread = new Thread(() => _2PC.Start(currentSlot, request.Client.ClientID, _state.GetProcessId()));
                thread.Start();
            }
            if (_2PC.WaitForCommit(request.Client.ClientID))
            {
                string response = _bankManager.Withdraw((int)request.Client.ClientID, request.Amount);
                return new WithdrawResp() { Response = response };
            }
            return new WithdrawResp() { Response = "FAIL" };
        }
    }
}
