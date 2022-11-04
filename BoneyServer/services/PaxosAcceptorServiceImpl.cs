using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.utils;
using Grpc.Core;

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
				Logger.LogDebugAcceptor($"Sending Promise( value: null ,  w_ts: {writeTS} , instance: {instance} , ACK: {ack} )");
				return new PromiseResp() { WriteTimeStamp = writeTS, PaxosInstance = instance, PromisseFlag = ack };
			}
			else
			{
				uint processID = value.ProcessID;
				uint Slot = value.Slot;
				CompareAndSwapReq valueToSend = new CompareAndSwapReq() { Leader = processID, Slot = Slot , Sender = _state.GetHostname()};
				Logger.LogDebugAcceptor($"Sending Promise( value: < slot: {valueToSend.Slot} , primary: {valueToSend.Leader}> , w_ts: {writeTS} , instance: {instance} , ACK: {ack} )");
				return new PromiseResp() { Value = valueToSend, WriteTimeStamp = writeTS, PaxosInstance = instance, PromisseFlag = ack };
			}
		}

        public override Task<PromiseResp> AckPromise(PromiseResp response, ServerCallContext context) {
            return Task.FromResult(response);
        }

        public override Task<AcceptedResp> Accept(AcceptReq request, ServerCallContext context) {
			Logger.LogDebugAcceptor($"Received ACCEPT!( value: ( primary: {request.Value.Leader}," +
				$" slot: {request.Value.Slot} ) ," +
				$"  w_ts: {request.LeaderNumber} )" +
				$" (PaxosAcceptorServiceImpl.cs l. 57))");
			if (!_state.IsFrozen()) {
				return Task.FromResult(doAccept(request));
			}
			// Message was queued and will he handled later
			throw new Exception("The server is frozen.");
		}

		public AcceptedResp doAccept(AcceptReq request) {
			bool accepted = _multiPaxos.UpdateAccept(new PaxosValue(request.Value.Leader,request.Value.Slot),
					request.LeaderNumber,request.PaxosInstance);
			if (accepted) Acceptor.SendAccepted(request);
			return new AcceptedResp();
		}

        public override Task<AcceptedResp> AckAccepted(AcceptedResp response, ServerCallContext context) {
            return Task.FromResult(response);
        }
    }
}
