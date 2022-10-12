using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.services
{
    internal class PaxosResultHandlerServiceImpl : PaxosResultHandlerService.PaxosResultHandlerServiceBase
    {
        public override Task<PaxosResultResponse> HandlePaxosResult(PaxosResultRequest request, ServerCallContext context)
        {
            // Use request.primary to chose a primary for request.slot
            return Task.FromResult(new PaxosResultResponse());
        }
    }
}
