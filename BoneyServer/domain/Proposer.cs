using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
 
    public delegate void TaskCompletedCallBack(string taskResult);
    public class Proposer
    {
        
        static uint _sourceLeaderNumber;
        static int _instance;
        static List<String> _boneyAdress;
        public static void proposeWork(uint sourceLeaderNumber,
            int instance,List<String> boneyAdress)
        {
            List<PromisseValue> _promisseInstances;
            _sourceLeaderNumber = sourceLeaderNumber;
            _instance = instance;
            _boneyAdress = boneyAdress;
            _promisseInstances = new List<PromisseValue>();


            foreach (String address in _boneyAdress)
            {
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                PaxosAcceptorService.PaxosAcceptorServiceClient client = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
                client.Prepare(new PrepareReq { LeaderNumber = _sourceLeaderNumber, PaxosInstance = (uint)_instance });
                PromisseValue response = new PromisseValue(new PaxosValue(reply.Value.Leader, reply.Value.Slot),
                    (int) reply.WriteTimeStamp, (int) reply.PaxosInstance);
                _promisseInstances.Add(response);


            }

        }




        public static async Task ProposeAsync()


    
     
















    }
}
