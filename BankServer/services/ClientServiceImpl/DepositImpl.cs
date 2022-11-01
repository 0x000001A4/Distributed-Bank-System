using BankServer.domain;
using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    public partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public override Task<DepositResp> Deposit(DepositReq request, ServerCallContext context)
        {
            Logger.LogDebug("Deposit received.");
            if (!_state.IsFrozen()) {
                DepositResp response = doDeposit(request);
                Logger.LogDebug("End of Deposit");
                return Task.FromResult(response);
            }
            Logger.LogDebug("End of Deposit");
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public DepositResp doDeposit(DepositReq request){

            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();

            while (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == 0) ; // while hasnt started yet
            if (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == _state.GetProcessId())
            {
                Logger.LogDebug("Starting 2PC");
                Thread thread = new Thread(() =>_2PC.Start(currentSlot, request.Client.ClientID, _state.GetProcessId()));
                thread.Start();
            }
            Logger.LogDebug("Waiting for commit started...");
            if (_2PC.WaitForCommit(request.Client.ClientID))
            {
                string response = _bankManager.Deposit((int)request.Client.ClientID, request.Amount);
                Logger.LogDebug("Waited succesfully, sending the response");
                return new DepositResp() { Response = response };
            }
            Logger.LogDebug("Timedout, sending the response FAIL");
            return new DepositResp() { Response = "FAIL"};
        }
    }
}
