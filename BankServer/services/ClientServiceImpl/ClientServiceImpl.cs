using Grpc.Core;
using BankServer.domain;

namespace BankServer.services
{
    partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public BankManager _bankManager;
        public ClientServiceImpl(BankManager bankManager)
        {
            _bankManager = bankManager;
            
        }
    }
}
