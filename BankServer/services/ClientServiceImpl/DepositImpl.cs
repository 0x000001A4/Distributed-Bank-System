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
            Logger.LogDebug(_state.IsFrozen().ToString());
            if (!_state.IsFrozen()) {
                DepositResp response = doDeposit(request);
                Logger.LogDebug("End of Deposit");
                return Task.FromResult(response);
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public DepositResp doDeposit(DepositReq request){

            Console.WriteLine("Verifying if Im leader");
            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();

            while (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == 0) ; // while hasnt started yet
            if (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == _state.GetProcessId())
            {
                Console.WriteLine("Starting 2PC");
                Thread thread = new Thread(() =>_2PC.Start(currentSlot, request.Client.ClientID, _state.GetProcessId()));
                thread.Start();
            }

            if (_2PC.WaitForCommit(request.Client.ClientID))
            {
                string response = _bankManager.Deposit((int)request.Client.ClientID, request.Amount);
                return new DepositResp() { Response = response };
            }
            
            return new DepositResp() { Response = "FAIL"};
        }
    }
}
