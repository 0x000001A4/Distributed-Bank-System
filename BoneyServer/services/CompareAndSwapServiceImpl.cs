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
        private IMultiPaxos _multiPaxos;


        public CompareAndSwapServiceImpl(IMultiPaxos multiPaxos) {
            _multiPaxos = multiPaxos;
        }

        public override Task<CompareAndSwapResp> CompareAndSwap(CompareAndSwapReq request, ServerCallContext context) {
           /* if (_serverState.isFrozen()) return Task.FromResult(new CompareAndSwapResp { Ok = false }); */
            doCompareAndSwap(request);
            return Task.FromResult(new CompareAndSwapResp());
        }


        public void doCompareAndSwap(CompareAndSwapReq request) {
            Console.WriteLine("BONEY CompareAndSwapServiceImpl: Received CompareAndSwap message request");
            PaxosValue value = new PaxosValue(request.Leader, request.Slot);
            _multiPaxos.Start(value);
        }
    }
}