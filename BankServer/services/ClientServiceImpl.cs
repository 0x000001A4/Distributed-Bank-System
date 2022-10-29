using Grpc.Core;
using BankServer.domain;

namespace BankServer.services
{
    internal class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public BankManager _bankManager;
        public ClientServiceImpl(BankManager bankManager)
        {
            _bankManager = bankManager;
            
        }
        public override Task<DepositResp> Deposit(DepositReq request, ServerCallContext context)
        {

            return base.Deposit(request, context);
        }

        public override Task<WithdrawResp> Withdraw(WithdrawReq request, ServerCallContext context)
        {
            return base.Withdraw(request, context);
        }

        public override Task<ReadResp> ReadBalance(ReadReq request, ServerCallContext context)
        {
            return base.ReadBalance(request, context);
        }

    }
}
