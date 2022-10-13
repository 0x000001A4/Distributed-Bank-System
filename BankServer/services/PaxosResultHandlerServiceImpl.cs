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
        private bool _debug = false;
        public override Task<PaxosResultResponse> HandlePaxosResult(PaxosResultRequest request, ServerCallContext context)
        {
            
            return Task.FromResult(new PaxosResultResponse());
        }
    }
}
