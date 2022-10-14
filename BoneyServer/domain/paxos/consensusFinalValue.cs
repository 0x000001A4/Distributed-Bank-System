using BoneyServer.utils;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain.paxos
{
    public static class ConsensusFinalValue

    {

        public static void DoWork(string address, uint slot, uint primary)
        {
            GrpcChannel channel = GrpcChannel.ForAddress("http://" + address);
            CompareAndSwapService.CompareAndSwapServiceClient _client =
                new CompareAndSwapService.CompareAndSwapServiceClient(channel);
            var reply = _client.HandlePaxosResult(new CompareAndSwapResp()
            {
                Slot = slot,
                Primary = primary
            });
        }
    }
}
