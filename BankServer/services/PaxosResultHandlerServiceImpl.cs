using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.utils;

namespace BankServer.services
{
    internal class PaxosResultHandlerServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        public override Task<CompareAndSwapResp> HandlePaxosResult(CompareAndSwapResp request, ServerCallContext context)
        {
            // Use request.primary to chose a primary for request.slot
            Logger.LogDebugBankServer($"Bank Server HandlePaxosResult(request) called:  Elected Primary: {request.Primary} | Slot: {request.Slot}");
            return Task.FromResult(request);
        }
    }
}
