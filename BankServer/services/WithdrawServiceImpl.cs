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
    internal class WithdrawServiceImpl : ClientService.ClientServiceBase
    {

        public BankManager _bankManager;
        public WithdrawServiceImpl(BankManager bankManager)
        {
            _bankManager = bankManager;

        }
        public override Task<WithdrawResp> Withdraw(WithdrawReq request, ServerCallContext context)
        {
            Logger.LogDebug("Withdraw received.");
            //Logger.LogDebug(_state.IsFrozen().ToString());
            //if (!_state.IsFrozen())
            //{
            WithdrawResp response = doWithdraw(request);
            Logger.LogDebug("End of CompareAndSwap");
            return Task.FromResult(response);                     //Rick Ve Isto
            //}
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public WithdrawResp doWithdraw(WithdrawReq request)
        {
            string response = _bankManager.Withdraw((int)request.Client.ClientID, request.Amount);
            return new WithdrawResp() { Response = response };
        }
    }
}
