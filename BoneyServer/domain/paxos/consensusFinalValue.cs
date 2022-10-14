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
       
        public static void DoWork(string address,uint slot,uint consensus)
        {
               GrpcChannel channel = GrpcChannel.ForAddress("http://" + address);
                PaxosResultHandlerService.PaxosResultHandlerServiceClient _client =
                    new PaxosResultHandlerService.PaxosResultHandlerServiceClient(channel);

                    var reply = _client.HandlePaxosResult(new PaxosResultRequest()
                    {
                        Slot = slot,
                        Primary = consensus
                    });
                
            
        }

    }
}
