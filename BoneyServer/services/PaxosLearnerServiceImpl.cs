using BoneyServer.domain;
using BoneyServer.domain.paxos;
using Grpc.Net.Client;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
    public class PaxosLearnerServiceImpl : PaxosLearnerService.PaxosLearnerServiceBase {

        IMultiPaxos _multiPaxos;
        static List<GrpcChannel> _bankServerChannels = new List<GrpcChannel>();
        uint _numberOfBoneys;
        BoneyServerState _state;
        

        public PaxosLearnerServiceImpl(List<string> bankServersInfo, IMultiPaxos multiPaxos, uint numberOfBoneyProcesses, BoneyServerState state) 
        {
            _multiPaxos = multiPaxos;
            SetServers(bankServersInfo);
            _numberOfBoneys = numberOfBoneyProcesses;
            _state = state;
        }

        public static void SetServers(List<String> bankServers)
        {
            _bankServerChannels = new List<GrpcChannel>();
            foreach (string address in bankServers) {
                _bankServerChannels.Add(GrpcChannel.ForAddress("http://" + address));
            }
        }

        public override Task<LearnCommandResp> LearnCommand(LearnCommandReq request, ServerCallContext context) {
            if (!_state.IsFrozen()) {
                return Task.FromResult(doLearnCommand(request));
            }
            // Message was queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public LearnCommandResp doLearnCommand(LearnCommandReq request)
        {
            uint requestInstance = request.PaxosInstance;
            PaxosInstance? requestInstanceInfo = _multiPaxos.GetPaxosInstance(requestInstance);
            requestInstanceInfo.AcceptedCommands++;
            if (MajorityAccepted(requestInstanceInfo))
            {
                foreach (var channel in _bankServerChannels)
                {
                    CompareAndSwapService.CompareAndSwapServiceClient _client =
                        new CompareAndSwapService.CompareAndSwapServiceClient(channel);


                    if (requestInstanceInfo.Value != null)
                    {
                        var reply = _client.HandlePaxosResult(new CompareAndSwapResp()
                        {
                            Slot = requestInstanceInfo.Value.Slot,
                            Primary = requestInstanceInfo.Value.ProcessID
                        });
                    }
                    else
                    {
                        Console.WriteLine("Unexpected behaviour in LearnCommand(LearnCommandReq request, ...): requestInstanceInfo.value = null (line 44: PaxosLearnerServiceImpl.cs)");
                        Environment.Exit(-1);
                    }
                }
            }
            return new LearnCommandResp();
        } 

        private bool MajorityAccepted(PaxosInstance currentInstanceInfo) {
            return currentInstanceInfo.AcceptedCommands > (_numberOfBoneys/2)+1;
        }
    }
}
