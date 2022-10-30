using Grpc.Core;
using BankServer.domain;

namespace BankServer.services
{
    public partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public BankManager _bankManager;
        public BankServerState _state;

        public ClientServiceImpl(BankManager bankManager, BankServerState state)
        {
            _bankManager = bankManager;
            _state = state;   
        }
    }
}
