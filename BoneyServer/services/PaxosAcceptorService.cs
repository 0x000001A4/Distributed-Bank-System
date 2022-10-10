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
        public override Task<Reply> Prepare(Request request, ServerCallContext context)
        {
            PrepareReq prepare = request.PrepareRequest;
            return base.Prepare(request, context);
        }
    }
}
