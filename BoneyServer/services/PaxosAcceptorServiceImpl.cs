using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{

    public class PaxosAcceptorServiceImpl : PaxosAcceptorService.PaxosAcceptorServiceBase
    {
        private IMultiPaxos _multiPaxos;
        private BoneyServerState _state;

        public PaxosAcceptorServiceImpl(BoneyServerState state, IMultiPaxos multiPaxos)
        {
            _state = state;
            _multiPaxos = multiPaxos;
        }

        public override Task<PromiseResp> Prepare(PrepareReq request, ServerCallContext context) {
            if (!_state.IsFrozen()) {
                return Task.FromResult(doPrepare(request));
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public PromiseResp doPrepare(PrepareReq request)
        {
            Logger.LogDebugAcceptor($"Received Prepare({request.LeaderNumber}) request");
            uint leaderNumber = request.LeaderNumber;
            uint instance = request.PaxosInstance;
            (PaxosValue value, uint writeTS,  bool ack) = _multiPaxos.Promisse(leaderNumber, instance);

            if (value == null) // if no value was chosen yet
            {
                Logger.LogDebugAcceptor($"Sending Promise( value: null ,  w_ts: {leaderNumber} , instance: {instance} , NACK )");
                return new PromiseResp() { WriteTimeStamp = leaderNumber, PaxosInstance = instance, PromisseFlag = ack };
            }
            else
            {
                uint processID = value.ProcessID;
                uint Slot = value.Slot;
                CompareAndSwapReq valueToSend = new CompareAndSwapReq() { Leader = processID, Slot = Slot };
                Logger.LogDebugAcceptor($"Sending Promise( value: < slot: {valueToSend.Slot} , primary: {valueToSend.Leader}> , w_ts: {writeTS} , instance: {instance} , ACK )");
                return new PromiseResp() { Value = valueToSend, WriteTimeStamp = writeTS, PaxosInstance = instance, PromisseFlag = ack };
            }
        }

      public override Task<AcceptedResp> Accept(AcceptReq request, ServerCallContext context) {
          Logger.LogDebug("PaxosAcceptorServiceImpl: Received ACCEPT! request");
          if (!_state.IsFrozen()) {
              return Task.FromResult(doAccept(request));
          }
              // Message was queued and will he handled later
          throw new Exception("The server is frozen.");
      }

      public AcceptedResp doAccept(AcceptReq request) {
          _multiPaxos.UpdateAccept(new PaxosValue(request.Value.Leader,request.Value.Slot),
                    request.LeaderNumber,request.PaxosInstance);
          Acceptor.LearnWork(request);
          return new AcceptedResp();
      }
	}
}
