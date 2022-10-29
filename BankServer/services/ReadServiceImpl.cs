using BankServer.utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.domain;

namespace BankServer.services
{
    internal class ReadServiceImpl : ClientService.ClientServiceBase
    {
        public BankManager _bankManager;
        public ReadServiceImpl(BankManager bankManager)
        {
            _bankManager = bankManager;

        }
        public override Task<ReadResp> Read(ReadReq request, ServerCallContext context)
        {
            Logger.LogDebug("Read received.");
            //Logger.LogDebug(_state.IsFrozen().ToString());
            //if (!_state.IsFrozen())
            //{
            ReadResp response = doRead(request);
            Logger.LogDebug("End of CompareAndSwap");
            return Task.FromResult(response);                     //Rick Ve Isto
            //}
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public ReadResp doRead(ReadReq request)
        {
            double balance = _bankManager.Read((int)request.Client.ClientID);
            return new ReadResp() { Balance = balance };
        }
    }
}
