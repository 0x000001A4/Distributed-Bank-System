using Grpc.Core;
using BankClient.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankClient.utils;

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
            Logger.LogDebug($"Deposit ack received: {response.Response}");
            lock (_bankClientFrontend)
            {
                _bankClientFrontend.AddDepoitResponse(response);
                Monitor.Pulse(_bankClientFrontend);
            }
            //Thread.CurrentThread.Interrupt();
            Logger.LogDebug($"end of deposit");
            return Task.FromResult(new AckResp() { });
        }

        public override Task<AckResp> AckWithdraw(WithdrawResp response, ServerCallContext context)
        {
            Logger.LogDebug($"Withdraw Ack received: {response.Response}");
            lock(_bankClientFrontend)
            {
                _bankClientFrontend.AddWithdrawResponse(response);
                Monitor.Pulse(_bankClientFrontend);
            }
            Thread.CurrentThread.Interrupt();

            
            //Thread.CurrentThread.Interrupt();
            Logger.LogDebug($"end of Withdraw");
            return Task.FromResult(new AckResp() { });
        }

        public override Task<AckResp> AckReadBalance(ReadResp response, ServerCallContext context)
        {
            Logger.LogDebug($"Read ack received | Balance read: {response.Balance}");
            lock (_bankClientFrontend)
            {
                _bankClientFrontend.AddReadBalanceResponse(response);
                Monitor.Pulse(_bankClientFrontend);
            }
            //Thread.CurrentThread.Interrupt();
            Logger.LogDebug($"end of ReadBalance");
            return Task.FromResult(new AckResp() { });
        }
    }
}
