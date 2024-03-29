﻿using BoneyServer.domain;
using BoneyServer.domain.paxos;
using Grpc.Net.Client;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneyServer.utils;

namespace BoneyServer.services
{
    public class PaxosLearnerServiceImpl : PaxosLearnerService.PaxosLearnerServiceBase {

        IMultiPaxos _multiPaxos;
        static List<GrpcChannel> _bankServerChannels = new List<GrpcChannel>();
        uint _numberOfBoneys;
        BoneyServerState _state;
        

        public PaxosLearnerServiceImpl(BoneyServerState state, IMultiPaxos multiPaxos, List<string> bankServersInfo, uint numberOfBoneyProcesses) 
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
            Logger.LogDebugLearner($"Servers set.");
        }

        public override Task<LearnCommandResp> LearnCommand(LearnCommandReq request, ServerCallContext context) { // edited ---------------
            if (!_state.IsFrozen()) {
                return Task.FromResult(doLearnCommand(request));
            }
            // Message was queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public LearnCommandResp doLearnCommand(LearnCommandReq request)
        {
            try
            {
                Logger.LogDebugLearner("Received Accept message");
                uint requestInstance = request.PaxosInstance;
                PaxosInstance? requestInstanceInfo = _multiPaxos.GetPaxosInstance(requestInstance);
                requestInstanceInfo.AcceptedCommands++;

                if (MajorityAccepted(requestInstanceInfo))
                {
                    Logger.LogDebugLearner($"Received majority of accepts for instance {requestInstance}.");
                    _state.GetSlotManager().FillSlot((int)request.Value.Slot, request.Value.Leader);
                    _multiPaxos.GetSlotState((int)request.Value.Slot).EndConsensus();
                    foreach (var channel in _bankServerChannels)
                    {
                        CompareAndSwapService.CompareAndSwapServiceClient _client =
                            new CompareAndSwapService.CompareAndSwapServiceClient(channel);

                        Logger.LogDebugLearner($"Value to send: {requestInstanceInfo.Value} ");
                        if (requestInstanceInfo.Value != null)
                        {
                            _client.HandlePaxosResultAsync(new CompareAndSwapResp()
                            {
                                Slot = requestInstanceInfo.Value.Slot,
                                Primary = requestInstanceInfo.Value.ProcessID
                            });
                            Logger.LogDebugLearner($"Response sent to all bank clients: (slot: {requestInstanceInfo.Value.Slot}, leader: {requestInstanceInfo.Value.ProcessID})");
                        }
                        else
                        {
                            Logger.LogError("Unexpected behaviour in LearnCommand(LearnCommandReq request, ...): requestInstanceInfo.value = null (line 44: PaxosLearnerServiceImpl.cs)");
                            throw new Exception();
                        }
                    }
                }
            } catch(Exception e) { 
                Logger.LogError(e.Message + "(Learner l. 84)");
                throw new Exception();
            }
            return new LearnCommandResp();
        }

        public override Task<LearnCommandResp> AckLearnCommand (LearnCommandResp response, ServerCallContext context) {
            return Task.FromResult(response);
        }

        private bool MajorityAccepted(PaxosInstance currentInstanceInfo) {
            return currentInstanceInfo.AcceptedCommands >= (int)(_numberOfBoneys/2)+1;
        }
    }
}
