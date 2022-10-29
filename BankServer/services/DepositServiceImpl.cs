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
        public override Task<DepositResp> HandleDepositReq(DepositReq request, ServerCallContext context)
        {
            
            return Task.FromResult(request);
        }
    }
}
