using Grpc.Net.Client;

namespace BoneyServer.domain
{

    public delegate void TaskCompletedCallBack(string taskResult);
	public class Proposer {
		
		static List<GrpcChannel> _boneyChannels = new List<GrpcChannel>();

		public static void SetServers(List<String> boneyAdress) 
		{
			_boneyChannels = new List<GrpcChannel>();
			foreach (string address in boneyAdress) {
				_boneyChannels.Add(GrpcChannel.ForAddress(address));
			}
		}

		/// <summary>
		/// Sends the Prepares and waits for majority of promises.
		/// After getting majority of promises sends the accepts (sendAccept function).
		/// </summary>
		/// <param name="value">Paxos value to propose</param>
		/// <param name="sourceLeaderNumber">Write timestamp</param>
		/// <param name="instance">Paxos instance</param>
		public static void ProposerWork(PaxosValue value, uint sourceLeaderNumber, uint instance)
		{
			List<ProposerVector> promisses = new List<ProposerVector>();
			sendPrepareAsync(sourceLeaderNumber, instance, promisses);
			waitForMajority(promisses);
			ProposerVector valueToSend = selectValueToSend(value, sourceLeaderNumber, instance, promisses);
			sendAccept(valueToSend);
        }

		private static void sendPrepareAsync(uint sourceLeaderNumber, uint instance, List<ProposerVector> promisses)
        {
			foreach (var channel in _boneyChannels)
			{
				Task ret = PrepareAsync(channel, sourceLeaderNumber, instance, promisses);
			}
		}

		public static async Task PrepareAsync(GrpcChannel channel, uint sourceLeaderNumber, uint instance, List<ProposerVector> promisses)
		{
			PaxosAcceptorService.PaxosAcceptorServiceClient client = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
			PromiseResp reply = await client.PrepareAsync(new PrepareReq { LeaderNumber = sourceLeaderNumber, PaxosInstance = instance });
			uint processElected = reply.Value.Leader;
			uint slot			= reply.Value.Slot;
			if (!reply.PromisseFlag) Thread.CurrentThread.Interrupt();
			ProposerVector promisse = new ProposerVector(new PaxosValue(processElected, slot), reply.WriteTimeStamp, reply.PaxosInstance);
			promisses.Add(promisse);
		}


		// TODO: it is currently actively waiting
		private static void waitForMajority(List<ProposerVector> promisses)
        {
			while (promisses.Count() < Math.Ceiling((decimal)_boneyChannels.Count() / 2));
		}


		private static ProposerVector selectValueToSend(PaxosValue value, uint sourceLeaderNumber, uint instance, List<ProposerVector> promisses)
        {
			ProposerVector valueToPropose = new ProposerVector(null, 0, 0);
			foreach (ProposerVector promisse in promisses)
			{
				if (promisse > valueToPropose)
				{
					valueToPropose = promisse;
				}
			}

			if (valueToPropose.Value == null || sourceLeaderNumber > valueToPropose.WriteTimeStamp)
				valueToPropose = new ProposerVector(value, sourceLeaderNumber, instance); // Choose my own value

			return valueToPropose;
		}

		private static void sendAccept(ProposerVector value)
        {
			foreach (var channel in _boneyChannels) {
				accept(channel, value);
            }

		}

		private static void accept(GrpcChannel channel, ProposerVector valueToSend)
		{
			uint leaderProcessID = valueToSend.Value.ProcessID;
			uint slot			 = valueToSend.Value.Slot;
			uint leaderNumber	 = valueToSend.WriteTimeStamp;
			uint instance		 = valueToSend.Instance;
			CompareAndSwapReq value = new CompareAndSwapReq() { Leader = leaderProcessID, Slot = slot };
            PaxosAcceptorService.PaxosAcceptorServiceClient client = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
			AcceptReq request = new AcceptReq { Value = value, LeaderNumber = leaderNumber, PaxosInstance = instance };
			AcceptedResp reply = client.Accept( request );
        }




		public class ProposerVector
		{
			public uint WriteTimeStamp;
			public PaxosValue? Value;
			public uint Instance;
			public ProposerVector(PaxosValue? value, uint writeStamp, uint instance)
			{
				Value = value;
				WriteTimeStamp = writeStamp;
				Instance = instance;
			}
			
			public static bool operator>(ProposerVector a, ProposerVector b)
            {
				return a.WriteTimeStamp > b.WriteTimeStamp;
            }
			public static bool operator<(ProposerVector a, ProposerVector b)
			{
				return a.WriteTimeStamp < b.WriteTimeStamp;
			}

		}

	}
}
