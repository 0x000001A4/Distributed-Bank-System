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
    public class PaxosResultHandlerServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        BankServerState _state;

        public PaxosResultHandlerServiceImpl(BankServerState state)
        {
            _state = state;
        }

        public override Task<CompareAndSwapResp> HandlePaxosResult(CompareAndSwapResp request, ServerCallContext context)
        {
            // Use request.primary to chose a primary for request.slot
            Logger.LogDebug($"Bank Server compareAndSwap response:  Elected ( Primary: {request.Primary}, Slot: {request.Slot})");
            return Task.FromResult(doHandlePaxosResult(request));
        }

        public CompareAndSwapResp doHandlePaxosResult(CompareAndSwapResp request)
        {
            return request;
        }
    }
}
