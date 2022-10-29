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
        /*public override Task<WithdrawResp> HandleWithdrawReq(WithdrawReq request, ServerCallContext context)
        {
            
            return Task.FromResult(request);
        }*/
    }
}
