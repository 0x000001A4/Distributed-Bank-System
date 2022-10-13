using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;
using Grpc.Net.Client;

namespace BoneyServer.services
{
    internal class PaxosLearnerServiceImpl : PaxosLearnerService.PaxosLearnerServiceBase {

        BoneyServerState _state;
        IMultiPaxos _multiPaxos;
        static List<GrpcChannel> _bankServerChannels = new List<GrpcChannel>();

        public PaxosLearnerServiceImpl(BoneyServerState state, IMultiPaxos multiPaxos) 
        {
            _state = state;
            _multiPaxos = multiPaxos;
            SetServers(_state.GetBankServersHostnameAndPort());
        }

        public static void SetServers(List<String> bankServers)
        {
            _bankServerChannels = new List<GrpcChannel>();
            foreach (string address in bankServers)
            {
                _bankServerChannels.Add(GrpcChannel.ForAddress("http://" + address));
            }
            Logger.LogDebugLearner($"Servers set.");
        }

        public override Task<LearnCommandResp> LearnCommand(LearnCommandReq request, ServerCallContext context)
        {
            Logger.LogDebugLearner("Received Accept message");
            uint requestInstance = request.PaxosInstance;
            PaxosInstance? requestInstanceInfo = _multiPaxos.GetPaxosInstance(requestInstance);
            if (MajorityAccepted(requestInstance, requestInstanceInfo)) {
                Logger.LogDebugLearner($"Received majority of accepts for instance {requestInstance}.");
                _state.GetSlotManager().FillSlot((int) request.Value.Slot, request.Value.Leader);
                _multiPaxos.GetSlotState((int)request.Value.Slot).EndConsensus();
                foreach(var channel in _bankServerChannels) {
                    PaxosResultHandlerService.PaxosResultHandlerServiceClient _client = 
                        new PaxosResultHandlerService.PaxosResultHandlerServiceClient(channel);


                    if (requestInstanceInfo.Value != null){
                        var reply = _client.HandlePaxosResult(new PaxosResultRequest()
                        {
                            Slot = requestInstanceInfo.Value.Slot,
                            Primary = requestInstanceInfo.Value.ProcessID
                        });
                    }
                    else {
                        Logger.LogError("Unexpected behaviour in LearnCommand(LearnCommandReq request, ...): requestInstanceInfo.value = null (line 44: PaxosLearnerServiceImpl.cs)");
                        Environment.Exit(-1);
                    }
                }
                Logger.LogDebugLearner($"Response sent to all bank clients: (slot: {requestInstanceInfo.Value.Slot}, leader: {requestInstanceInfo.Value.ProcessID})");
            }
            return Task.FromResult(new LearnCommandResp());
        }

        private bool MajorityAccepted(uint currentInstance, PaxosInstance currentInstanceInfo) {
            return currentInstanceInfo.AcceptedCommands > _state.GetNumberOfBoneyProcesses();
        }
    }
}
