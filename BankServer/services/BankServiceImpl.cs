using BankServer.domain;
using BankServer.domain.bank;
using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    public class BankServiceImpl : BankService.BankServiceBase
    {

        ITwoPhaseCommit _2PC;
        BankServerState _state;

        public BankServiceImpl(ITwoPhaseCommit _2pc, BankServerState state)
        {
            _2PC = _2pc;
            _state = state;
        }  

        public override Task<ProposeResp> ProposeSeqNum(ProposeReq request, ServerCallContext context)
        {
            Logger.LogDebug("ProposeSeqNum received");
            if (!_state.IsFrozen()) {
                return Task.FromResult(doPropose(request));
            }
            Logger.LogDebug("ProposeSeqNum end frozen");
            throw new Exception("The server is frozen");
        }

        public ProposeResp doPropose(ProposeReq request) {
            bool _isPrimary = (request.PrimaryBankID == (uint)_state.GetSlotManager().GetPrimaryOnSlot(request.Slot));
            if (_isPrimary) _2PC.AcceptProposedSeqNum((int)request.SeqNumber);
            Logger.LogDebug("ProposeSeqNum end not frozen");
            return new ProposeResp { Ack = _isPrimary };
        }


        public override Task<CommitResp> CommitSeqNum(CommitReq request, ServerCallContext context)
        {
            Logger.LogDebug("CommitSeqNum received");
            if (!_state.IsFrozen()) {
                return Task.FromResult(doCommit(request));
            }
            Logger.LogDebug("CommitSeqNum end frozen");
            throw new Exception("The server is frozen");
        }

        public CommitResp doCommit(CommitReq request)
        {
            _2PC.HandleCommit((int)request.SeqNumber, request.ClientID);
            Logger.LogDebug("CommitSeqNum end not frozen");
            return new CommitResp() { };
        }

        public override Task<ListPendingRequestsResp> ListPendingRequests(ListPendingRequestsReq request, ServerCallContext context)
        {
            Logger.LogDebug("ListPendingRequests received");
            if (!_state.IsFrozen()) {
                return Task.FromResult(doListPendingRequests(request));
            }
            Logger.LogDebug("ListPendingRequests end frozen");
            throw new Exception("The server is frozen.");
        }

        public ListPendingRequestsResp doListPendingRequests(ListPendingRequestsReq request)
        {
            List<ClientRequest> _clientRequests = _2PC.GetClientRequests();
            List<ClientRequestMsg> _pendingRequests = new List<ClientRequestMsg>();
            for (int seqNum = 0; seqNum < _clientRequests.Count; seqNum++)
            {
                ClientRequest clientRequest = _clientRequests[seqNum];
                _pendingRequests.Add(new ClientRequestMsg {
                    ClientId = clientRequest.GetClientId(),
                    SeqNum = clientRequest.GetSeqNum(),
                    Commited = clientRequest.isCommited()
                });
            }

            var response = new ListPendingRequestsResp { };
            response.PendingRequests.Add(_pendingRequests);
            Logger.LogDebug("ListPendingRequests end not frozen");
            return response;
        }
    }
}
