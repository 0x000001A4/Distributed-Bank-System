using BankServer.utils;
using Grpc.Core;

namespace BankServer.services
{
    public partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        public override Task<ReadResp> ReadBalance(ReadReq request, ServerCallContext context)
        {
            Logger.LogDebug("Read received.");
            Logger.LogDebug(_state.IsFrozen().ToString());
            if (!_state.IsFrozen()) {
                ReadResp response = doRead(request);
                Logger.LogDebug("End of Read");
                return Task.FromResult(response);
            }
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public ReadResp doRead(ReadReq request)
        {

            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();
            while (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == 0) ;

            if (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == _state.GetProcessId()) {
                _2PC.Start(_state.GetSlotManager().GetCurrentSlot(), request.Client.ClientID, _state.GetProcessId());
            }

            if (_2PC.WaitForCommit(request.Client.ClientID))
            {
                double balance = _bankManager.Read((int)request.Client.ClientID);
                return new ReadResp() { Balance = balance };
            }

            return new ReadResp() { Balance = -1};
        }
    }
}
