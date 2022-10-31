using BankServer.domain;
using BankServer.domain.bank;
using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    public class BankServiceImpl : BankService.BankServiceBase
    {
        BankServerState _state;
        ITwoPhaseCommit _2PC;

        public BankServiceImpl(ITwoPhaseCommit _2pc, BankServerState state)
        {
            _2PC = _2pc;
            _state = state;
        }


        public override Task<ProposeResp> ProposeSeqNum(ProposeReq request, ServerCallContext context)
        {
            Logger.LogDebug("Porpose received.");
            Logger.LogDebug(_state.IsFrozen().ToString());
            if (!_state.IsFrozen()) {
                Logger.LogDebug("End of Read");
                return Task.FromResult(new ProposeResp());
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public override Task<ListPendingRequestsResp> ListPendingRequests(ListPendingRequestsReq request, ServerCallContext context)
        {
            if (!_state.IsFrozen()) {
                return Task.FromResult(doListPendingRequests(request));
            }
            throw new Exception("The server is frozen.");
        }

        public ListPendingRequestsResp doListPendingRequests(ListPendingRequestsReq request)
        {
            List<ClientRequest> _clientRequests = _state.GetClientRequests();
            List<ClientRequestMsg> _pendingRequests = new List<ClientRequestMsg>();
            for (int seqNum = 0; seqNum < _clientRequests.Count; seqNum++)
            {
                ClientRequest clientRequest = _clientRequests[seqNum];
                _pendingRequests.Add(new ClientRequestMsg {
                    ClientId = clientRequest.GetClientId(),
                    Commited = clientRequest.isCommited()
                });
            }

            var response = new ListPendingRequestsResp { };
            response.PendingRequests.Add(_pendingRequests);
            return response;
        }
    }
}
