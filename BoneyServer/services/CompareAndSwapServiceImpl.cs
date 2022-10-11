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
        private IMultiPaxos _multiPaxos;

        public CompareAndSwapServiceImpl(BoneySlotManager slotManager, IMultiPaxos multiPaxos)
        {
            _slotManager = slotManager;
            _multiPaxos = multiPaxos;
        }
        public override Task<CompareAndSwapResp> CompareAndSwap(CompareAndSwapReq request, ServerCallContext context)
        {
            Console.WriteLine("BONEY CompareAndSwapServiceImpl: Received CompareAndSwap message request");
            PaxosValue value = new PaxosValue(request.Leader, request.Slot);
            _multiPaxos.Start(value);
            return Task.FromResult(new CompareAndSwapResp());
        }
    }
}
