using BankServer.utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.utils;
using BankServer.domain;

namespace BankServer.services
{
    internal class DepositServiceImpl : ClientService.ClientServiceBase
    {
        public BankManager _bankManager;
        public DepositServiceImpl(BankManager bankManager)
        {
            _bankManager = bankManager;
            
        }
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
            throw new Exception("The server is frozen.");
        }

        public DepositResp doDeposit(DepositReq request)
        {
            string response = _bankManager.Deposit((int) request.Client.ClientID,request.Amount);
            return new DepositResp() { Response = response};
        }
    }
}
