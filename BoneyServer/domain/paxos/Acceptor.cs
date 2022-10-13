using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BoneyServer.domain.paxos.Acceptor;

namespace BoneyServer.domain.paxos
{
    public class Acceptor
    {

        static List<GrpcChannel> _boneyChannels = new List<GrpcChannel>();

        public static void SetServers(List<string> boneyAdress)
        {
            _boneyChannels = new List<GrpcChannel>();
            foreach (string address in boneyAdress)
            {
                _boneyChannels.Add(GrpcChannel.ForAddress("http://" + address));
            }
        }


        public static bool PromisseWork(uint leaderNumber, uint readTimeStamp)
        {
            if (leaderNumber >= readTimeStamp) return true;
            else return false;
        }




        public static void LearnWork(AcceptReq request)
        {
            Task ret = AcceptCommand(
                   new CompareAndSwapReq(request.Value),
                   request.LeaderNumber,
                   request.PaxosInstance);
        }

        public async static Task AcceptCommand(CompareAndSwapReq compareAndSwapReq, uint leaderNumber, uint instance)
        {
            foreach (var channel in _boneyChannels)
            {
                PaxosLearnerService.PaxosLearnerServiceClient client = new PaxosLearnerService.PaxosLearnerServiceClient(channel);
                LearnCommandResp reply = await client.LearnCommandAsync(
                    new LearnCommandReq { Value = compareAndSwapReq, LeaderNumber = leaderNumber, PaxosInstance = instance }
                );
            }
        }
    }

    public class AcceptValue
    {
        PaxosValue _value;
        uint _leaderNumber;
        uint _instance;

        public AcceptValue(PaxosValue value, uint leaderNumber, uint instance)
        {
            _value = value;
            _leaderNumber = leaderNumber;
            _instance = instance;
        }
    }
}
