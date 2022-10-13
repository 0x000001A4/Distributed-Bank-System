using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public override Task<LearnCommandResp> LearnCommand(LearnCommandReq request, ServerCallContext context)
        {
            uint requestInstance = request.PaxosInstance;
            PaxosInstance? requestInstanceInfo = _multiPaxos.GetPaxosInstance(requestInstance);
            if (MajorityAccepted(requestInstance, requestInstanceInfo)) {
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
            }
            return Task.FromResult(new LearnCommandResp());
        }

        private bool MajorityAccepted(uint currentInstance, PaxosInstance currentInstanceInfo) {
            return currentInstanceInfo.AcceptedCommands > _state.GetNumberOfBoneyProcesses();
        }
    }
}
