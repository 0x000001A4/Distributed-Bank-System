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
        
        static List<GrpcChannel> _boneyChannels;


        public static void SetServers(List<String> boneyAdress)
        {
            _boneyChannels = new List<GrpcChannel>();
            foreach (string address in boneyAdress)
            {
                _boneyChannels.Add(GrpcChannel.ForAddress(address));
            }

        }
        public static void ProposeWork(uint sourceLeaderNumber, uint instance)
        {
            List<PromisseValue> promisses = new List<PromisseValue>();

            foreach (var channel in _boneyChannels)
            {
                Task ret = PrepareAsync(channel, sourceLeaderNumber, instance, promisses);
            }

            while (promisses.Count() < Math.Ceiling((decimal) _boneyChannels.Count() / 2) )
            {

            }

        }




        public static async Task PrepareAsync(GrpcChannel channel, uint sourceLeaderNumber, uint instance, List<PromisseValue> promisses)
        {
            PaxosAcceptorService.PaxosAcceptorServiceClient client = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
            PromiseResp reply = await client.PrepareAsync(new PrepareReq { LeaderNumber = sourceLeaderNumber, PaxosInstance = instance });
            uint processElected = reply.Value.Leader;
            uint slot = reply.Value.Slot;
            PromisseValue promisse = new PromisseValue(new PaxosValue(processElected, slot), reply.WriteTimeStamp, reply.PaxosInstance);
            promisses.Add(promisse);
        }

        public class PromisseValue
        {
            uint _writeTimeStamp;
            PaxosValue _value;
            uint _instance;
            public PromisseValue(PaxosValue value, uint writeStamp, uint instance)
            {
                _value = value;
                _writeTimeStamp = writeStamp;
                _instance = instance;
            }
        }

    }
}
