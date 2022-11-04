using Grpc.Core;
using BankClient.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClient.services
{
    public class ClientServiceImpl : ClientService.ClientServiceBase
    {
        BankClientFrontend _bankClientFrontend;

        public ClientServiceImpl(BankClientFrontend bankClientFrontend)
        {
            _bankClientFrontend = bankClientFrontend;
        }

        public override Task<AckResp> AckDeposit(DepositResp response, ServerCallContext context)
        {
            Console.WriteLine($"Deposit ack received: {response.Response}");
            Monitor.Pulse(_bankClientFrontend);
            Thread.CurrentThread.Interrupt();
            return Task.FromResult(new AckResp() { });
        }

        public override Task<AckResp> AckWithdraw(WithdrawResp response, ServerCallContext context)
        {
            Console.WriteLine($"Withdraw ack received: {response.Response}");
            Monitor.Pulse(_bankClientFrontend);
            Thread.CurrentThread.Interrupt();
            return Task.FromResult(new AckResp() { });
        }

        public override Task<AckResp> AckReadBalance(ReadResp response, ServerCallContext context)
        {
            Console.WriteLine($"Read ack received | Balance read: {response.Balance}");
            Monitor.Pulse(_bankClientFrontend);
            Thread.CurrentThread.Interrupt();
            return Task.FromResult(new AckResp() { });
        }
    }
}
