using BoneyServer.domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
    internal class CompareAndSwapServiceImpl : CompareAndSwapService.CompareAndSwapServiceBase
    {
        private BoneySlotManager _slotManager;

        public CompareAndSwapServiceImpl(BoneySlotManager slotManager)
        {
            _slotManager = slotManager;
        }
        public override Task<CompareAndSwapResponse> CompareAndSwap(CompareAndSwapRequest request, ServerCallContext context)
        {
            Console.WriteLine("received msg");
            return Task.FromResult(new CompareAndSwapResponse { Ok = true });
        }
    }
}
