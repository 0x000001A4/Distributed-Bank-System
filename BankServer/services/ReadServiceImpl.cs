using BankServer.utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.domain;

namespace BankServer.services
{
    internal class ReadServiceImpl : ClientService.ClientServiceBase
    {
        public BankManager _bankManager;
        public ReadServiceImpl(BankManager bankManager)
        {
            _bankManager = bankManager;

        }
        /*public override Task<ReadResp> HandleReadReq(ReadReq request, ServerCallContext context)
        {
            
            return Task.FromResult(request);
        }*/
    }
}
