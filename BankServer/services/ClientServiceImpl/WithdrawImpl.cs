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
                Logger.LogDebug("End of Withdraw not frozen");
                return Task.FromResult(response);
            }
            Logger.LogDebug("End of Withdraw frozen");
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public WithdrawResp doWithdraw(WithdrawReq request)
        {
            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();
            Logger.LogDebug($"Withdraw: slot is {currentSlot}");
            while (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == 0);
            Logger.LogDebug($"Withdraw: primary is {_state.GetSlotManager().GetPrimaryOnSlot(currentSlot)}");

            if (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == _state.GetProcessId()) {
                Logger.LogDebug("Starting 2PC");
                Thread thread = new Thread(() => _2PC.Start(currentSlot, request.Client.ClientID, _state.GetProcessId()));
                thread.Start();
            }
            Logger.LogDebug("Waiting for commit started...");
            if (_2PC.WaitForCommit(request.Client.ClientID))
            {
                string response = _bankManager.Withdraw(request.Amount);
                Logger.LogDebug("Waited succesfully, sending the response");
                return new WithdrawResp() { Response = response };
            }
            Logger.LogDebug("Timedout, sending the response FAIL");
            return new WithdrawResp() { Response = "FAIL" };
        }
    }
}
