using BankServer.domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.services.BankServiceImpl
{
    internal class BankServiceImpl : BankService.BankServiceBase
    {
      
        List<bool> _log2PC;
      

        public BankServiceImpl(, List<bool> lista)
        {
            
            _log2PC = lista;
            
        }


        public override Task<AcceptResp> Propose(ProposeReq request, ServerCallContext context)
        {
            //Logger.LogDebug("Porpose received.");
            //Logger.LogDebug(_state.IsFrozen().ToString());
            //if (!_state.IsFrozen())
            //{
            AcceptResp response = doAccept(request);
            //Logger.LogDebug("End of Read");
            return Task.FromResult(response);                     //Rick Ve Isto
            //}
            // Request got queued and will be handled later
            //throw new Exception("The server is frozen.");
        }

        public ReadResp doAccept(ReadReq request)
        {
            double balance = _bankManager.Read((int)request.Client.ClientID);
            return new ReadResp() { Balance = balance };
        }
    }
}
