using BankServer.domain;
using BankServer.domain.bank;
using BankServer.utils;

namespace BankServer.services
{
    public partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        BankServerState _state;
        ServerConfiguration _config;
        int _processId;
        BankManager _bankManager;
        ITwoPhaseCommit _2PC;
        int logPosition;


        public ClientServiceImpl(ServerConfiguration config,int processId, BankManager bankManager,
            ITwoPhaseCommit _2pc, BankServerState state)
        {
            _state = state;
            _bankManager = bankManager;
            _config = config;
            _processId = processId;
            _2PC = _2pc;
            logPosition = 0;
        }

        public bool verifyImLeader()
        {
            List<int> bankIds = _config.GetBankServerIDs();
            uint leaderId = (uint)bankIds[0];

            foreach (int id in bankIds)
            {
                if (_config.GetServerSuspectedInSlot((uint)id, _state.GetSlotManager().GetCurrentSlot()) == SuspectState.NOTSUSPECTED)
                {
                   
                    leaderId = (uint)id;
                    if (_processId == leaderId) return true;
                    else return false;
                }
            }
            return false;
        }

    }
}
