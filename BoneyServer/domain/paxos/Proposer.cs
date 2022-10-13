using BoneyServer.utils;
using Grpc.Net.Client;

namespace BoneyServer.domain.paxos
{

    public delegate void TaskCompletedCallBack(string taskResult);
    public class Proposer
    {

        static List<GrpcChannel> _boneyChannels = new List<GrpcChannel>();

        public static void SetServers(List<string> boneyAdress)
        {
            _boneyChannels = new List<GrpcChannel>();
            foreach (string address in boneyAdress)
            {
                _boneyChannels.Add(GrpcChannel.ForAddress("http://" + address));
            }
            Logger.LogDebugProposer("Proposer servers set.");
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
            Logger.LogDebugProposer("New proposer for instance " + instance);
            List<ProposerVector> promisses = new List<ProposerVector>(); // used to store all promisses received
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
            ProposerVector promisse = new ProposerVector();

            if (!reply.PromisseFlag) // promisse has greater readTS than the sent in prepare
            {
                Logger.LogDebugProposer("Proposer canceled proposing becuase received a NACK from " + channel.Target);
                Thread.CurrentThread.Interrupt();
            }
            else if (reply.Value == null) // promisse has null value
            {
                Logger.LogDebugProposer("Proposer received a null value from " + channel.Target);
                promisse = new ProposerVector(null, reply.WriteTimeStamp, reply.PaxosInstance);
            }
            else // promisse has value
            {
                Logger.LogDebugProposer("Proposer received a value from " + channel.Target);
                uint slot = reply.Value.Slot;
                uint processElected = reply.Value.Leader;
                promisse = new ProposerVector(new PaxosValue(processElected, slot), reply.WriteTimeStamp, reply.PaxosInstance);
            }
            
            promisses.Add(promisse);
            Logger.LogDebugProposer($"Received {promisses.Count()}/{_boneyChannels.Count()} promisses");
        }

        // TODO: it is currently actively waiting
        private static void waitForMajority(List<ProposerVector> promisses)
        {
            Logger.LogDebugProposer("Waiting for a majority of promisses...");
            while (promisses.Count() < Math.Ceiling((decimal)_boneyChannels.Count() / 2));
            Logger.LogDebugProposer("Received a majority of promisses.");
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
            foreach (var channel in _boneyChannels)
            {
                accept(channel, value);
                Logger.LogDebugProposer("Accept sent to " + channel.Target);
            }

        }

        private static void accept(GrpcChannel channel, ProposerVector valueToSend)
        {
            uint leaderProcessID = valueToSend.Value.ProcessID;
            uint slot = valueToSend.Value.Slot;
            uint leaderNumber = valueToSend.WriteTimeStamp;
            uint instance = valueToSend.Instance;
            CompareAndSwapReq value = new CompareAndSwapReq() { Leader = leaderProcessID, Slot = slot };
            PaxosAcceptorService.PaxosAcceptorServiceClient client = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
            AcceptReq request = new AcceptReq { Value = value, LeaderNumber = leaderNumber, PaxosInstance = instance };
            AcceptedResp reply = client.Accept(request);
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

            public ProposerVector() { }

            public static bool operator >(ProposerVector a, ProposerVector b)
            {
                return a.WriteTimeStamp > b.WriteTimeStamp;
            }
            public static bool operator <(ProposerVector a, ProposerVector b)
            {
                return a.WriteTimeStamp < b.WriteTimeStamp;
            }

        }

    }
}
