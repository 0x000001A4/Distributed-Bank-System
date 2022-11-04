using BankServer.domain;
using BankServer.domain.bank;
using BankServer.utils;

namespace BankServer.services
{
    public partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        BankServerState _state;
        ServerConfiguration _config;
        BankManager _bankManager;
        ITwoPhaseCommit _2PC;
        private object _lock;


        public ClientServiceImpl(ServerConfiguration config, BankManager bankManager,
            ITwoPhaseCommit _2pc, BankServerState state, object __lock)
        {
            _state = state;
            _bankManager = bankManager;
            _config = config;
            _2PC = _2pc;
            _lock = __lock;
        }
    }
}
