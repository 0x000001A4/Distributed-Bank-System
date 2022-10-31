using BankServer.domain;
using BankServer.domain.bank;
using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    public class BankServiceImpl : BankService.BankServiceBase
    {

        ITwoPhaseCommit _2PC;
        ServerConfiguration _config;
        BankServerState _state;

        public BankServiceImpl(ITwoPhaseCommit _2pc, ServerConfiguration config, BankServerState state)
        {
            _2PC = _2pc;
            _config = config;
            _state = state;
        }

        public bool verifyIsLeader(uint liderID,uint slot)
        {
            List<int> bankIds = _config.GetBankServerIDs();
            uint leaderId = (uint)bankIds[0];
            uint actualSlot = _state.GetSlotManager().GetCurrentSlot();
            for (uint slotI = slot; slotI <= actualSlot; slotI ++ )
            {
                foreach (int id in bankIds){

                    if (_config.GetServerSuspectedInSlot((uint)id,slotI ) == SuspectState.NOTSUSPECTED)
                    {

                    leaderId = (uint)id;
                    if (liderID != leaderId) return false;
                    }
                }

            }
         
            return true;
        }


  

        public override Task<ProposeResp> ProposeSeqNum(ProposeReq request, ServerCallContext context)
        {

           if(verifyIsLeader(request.PrimaryBankID,request.Slot))
            {
                _2PC.AcceptProposedSeqNum((int)request.SeqNumber);
                ProposeResp response = new ProposeResp() { Ack = true };
            }
            else
            {
                ProposeResp response = new ProposeResp() { Ack = false };
            }


            return Task.FromResult(new ProposeResp());                     //Rick Ve Isto
           
        }


        public override Task<CommitResp> CommitSeqNum(CommitReq request, ServerCallContext context)
        {


            _2PC.HandleCommit((int)request.SeqNumber, request.ClientID);
            return Task.FromResult(new CommitResp());                     //Rick Ve Isto

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
            List<ClientRequest> _clientRequests = _2PC.GetClientRequests();
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
