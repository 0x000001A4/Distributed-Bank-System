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
			return (leaderNumber >= readTimeStamp);
		}




		public static void SendAccepted(AcceptReq request) {
			try {
				AcceptCommand(
				   new CompareAndSwapReq(request.Value),
				   request.LeaderNumber,
				   request.PaxosInstance);
			} catch(Exception e) {
				Console.WriteLine(e);
				throw e;
			}
		}


		public static void AcceptCommand(CompareAndSwapReq compareAndSwapReq, uint leaderNumber, uint instance)
		{
			Logger.LogDebugAcceptor("sending accepted to learners..");
			foreach (var channel in _boneyChannels)
			{
				PaxosLearnerService.PaxosLearnerServiceClient client = new PaxosLearnerService.PaxosLearnerServiceClient(channel);
				Logger.LogDebugAcceptor("sending Accepted to " + channel.Target);
				try { 
					client.LearnCommandAsync(
						new LearnCommandReq { Value = compareAndSwapReq, LeaderNumber = leaderNumber, PaxosInstance = instance }
					);
				} catch(Exception e) {
					Logger.LogError(e + "(Acceptor.cs   l. 60)");
					throw e;
				}
			Logger.LogDebugAcceptor($"Accepted sent to all Learners: < (slot: {compareAndSwapReq.Slot}, leader: {compareAndSwapReq.Leader}), w_ts: {leaderNumber}, instance: {instance} >");
			}
		}
	}
}
