using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
    internal class PaxosAcceptorServiceImpl : PaxosAcceptorService.PaxosAcceptorServiceBase
    {
        public override Task<PromiseResp> Prepare(PrepareReq request, ServerCallContext context)
        {
            PrepareReq prepare = request;
            return Task.FromResult(new PromiseResp());
        }
    }
}
