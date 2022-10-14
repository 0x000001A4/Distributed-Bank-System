using BoneyServer.utils;
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
            Logger.LogDebugAcceptor("Servers set.");
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




		public static void LearnWork(AcceptReq request) {
			try {
				Task ret = AcceptCommand(
				   new CompareAndSwapReq(request.Value),
				   request.LeaderNumber,
				   request.PaxosInstance);
			} catch(Exception e) {
				Console.WriteLine(e);
			}
		}


		public async static Task AcceptCommand(CompareAndSwapReq compareAndSwapReq, uint leaderNumber, uint instance)
		{
			foreach (var channel in _boneyChannels)
			{
				PaxosLearnerService.PaxosLearnerServiceClient client = new PaxosLearnerService.PaxosLearnerServiceClient(channel);
				try { 
					LearnCommandResp reply = await client.LearnCommandAsync(
						new LearnCommandReq { Value = compareAndSwapReq, LeaderNumber = leaderNumber, PaxosInstance = instance }
					);
				} catch(Exception e) {
					Console.WriteLine(e);
				}
        Logger.LogDebugAcceptor($"Accepted sent to all Learners: < (slot: {compareAndSwapReq.Slot}, leader: {compareAndSwapReq.Leader}), w_ts: {leaderNumber}, instance: {instance} >");
			}
		}
	}
}
