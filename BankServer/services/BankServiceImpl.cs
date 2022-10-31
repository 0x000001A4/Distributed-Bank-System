using BankServer.domain;
using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    internal class BankServiceImpl : BankService.BankServiceBase
    {

        ITwoPhaseCommit _2PC;


        public BankServiceImpl(ITwoPhaseCommit _2pc)
        {
            _2PC = _2pc;
        }


        public override Task<ProposeResp> ProposeSeqNum(ProposeReq request, ServerCallContext context)
        {
            //Logger.LogDebug("Porpose received.");
            //Logger.LogDebug(_state.IsFrozen().ToString());
            //if (!_state.IsFrozen())
            //{
            //Logger.LogDebug("End of Read");
            return Task.FromResult(new ProposeResp());                     //Rick Ve Isto
            //}
            // Request got queued and will be handled later
            //throw new Exception("The server is frozen.");
        }

    }
}
