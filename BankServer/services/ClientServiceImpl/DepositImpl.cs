using BankServer.domain;
using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public override Task<DepositResp> Deposit(DepositReq request, ServerCallContext context)
        {
            Logger.LogDebug("Deposit received.");
            //Logger.LogDebug(_state.IsFrozen().ToString());
            //if (!_state.IsFrozen())
            //{
            DepositResp response = doDeposit(request);
                Logger.LogDebug("End of Deposit");
                return Task.FromResult(response);                     //Rick Ve Isto
            //}
            // Request got queued and will be handled later
            //throw new Exception("The server is frozen.");
        }

        public DepositResp doDeposit(DepositReq request){
            /* Consensus */
            int logPos;
            if (verifyImLeader())
            {
                logPos = before2pc();
                IncrementLog();
                Propose2PC.executePropose((uint)_processId, _bankSlotManager.GetCurrentSlot(), _config, logPosition, request.Address);
                after2pc(logPosition);


            }
            else
            {
                while(getLogPosition(logPos))
            }
            string response = _bankManager.Deposit((int) request.Client.ClientID,request.Amount);
           
            
            
            return new DepositResp() { Response = response};
        }
    }
}
