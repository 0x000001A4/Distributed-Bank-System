using BoneyServer.domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
    public class CompareAndSwapServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        private BoneyServerState _serverState;

        public CompareAndSwapServiceImpl(BoneyServerState serverState)
        {
            _serverState = serverState;
        }
        public override Task<CompareAndSwapResponse> CompareAndSwap(CompareAndSwapRequest request, ServerCallContext context)
        {
            if (_serverState.isFrozen()) return Task.FromResult(new CompareAndSwapResponse { Ok = false });
            doCompareAndSwap(request);
            return Task.FromResult(new CompareAndSwapResponse { Ok = true });
        }

        public void doCompareAndSwap(CompareAndSwapRequest request) {
            Console.WriteLine("received msg");
        }
    }
}
