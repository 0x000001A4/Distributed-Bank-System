using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {
 
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

		// Proposer: Sends the Prepares and waits for majority of promises
		// After getting majority of promises sends the accepts (AcceptWork function)
		public static void ProposeWork(PrepareReq request)
		{
			uint sourceLeaderNumber = request.LeaderNumber;
			uint instance = request.PaxosInstance;

            List<PromisseValue> promisses = new List<PromisseValue>();

			foreach (var channel in _boneyChannels) {
				Task ret = PrepareAsync(channel, sourceLeaderNumber, instance, promisses);
			}

			while (promisses.Count() < Math.Ceiling((decimal)_boneyChannels.Count() / 2));

			AcceptWork(promisses, instance);
        }

        // Proposer: Sends the Accepts.
        public static void AcceptWork(List<PromisseValue> promisses, uint instance)
		{
			uint max_timestamp = 0;
            PaxosValue? paxosValue = null;
            foreach (PromisseValue promisseValue in promisses) {
				uint write_timestamp = promisseValue.GetWriteTimestamp();
				if (write_timestamp > max_timestamp) {
					max_timestamp = write_timestamp;
					paxosValue = promisseValue.GetPaxosValue();
				}
			}
			if (paxosValue != null) {
				foreach(var channel in _boneyChannels) {
					Accept(
						channel,
						new CompareAndSwapReq { Slot = paxosValue.Slot, Leader = paxosValue.ProcessID },
						max_timestamp,
						instance
						);
				}
			}
			else { 
				/* Think of this ? */
			}

		}

		public static void Accept(GrpcChannel channel, CompareAndSwapReq compareAndSwapReq, uint leaderNumber, uint instance)
		{
            PaxosAcceptorService.PaxosAcceptorServiceClient client = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
            AcceptedResp reply = client.Accept(
                new AcceptReq { Value = compareAndSwapReq, LeaderNumber = leaderNumber, PaxosInstance = instance }
            );
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
			
			public PaxosValue GetPaxosValue()
			{
				return _value;
			}

			public uint GetWriteTimestamp()
			{
				return _writeTimeStamp;
			}
		}

	}
}
