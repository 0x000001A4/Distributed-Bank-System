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
                Logger.LogDebug("End of Read when not frozen");
                return Task.FromResult(response);
            }
            Logger.LogDebug("End of Read when frozen");
            // Request got queued and will be handled later
            throw new Exception("The server is frozen.");
        }

        public ReadResp doRead(ReadReq request)
        {

            uint currentSlot = _state.GetSlotManager().GetCurrentSlot();
            while (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == 0) ;

            if (_state.GetSlotManager().GetPrimaryOnSlot(currentSlot) == _state.GetProcessId()) {
                Logger.LogDebug("Starting 2PC");
                Thread thread = new Thread(() => _2PC.Start(currentSlot, request.Client.ClientID, _state.GetProcessId()));
                thread.Start();
            }
            Logger.LogDebug("Waiting for commit started...");
            if (_2PC.WaitForCommit(request.Client.ClientID))
            {
                double balance = _bankManager.Read((int)request.Client.ClientID);
                Logger.LogDebug("Waited succesfully, sending the response");
                return new ReadResp() { Balance = balance };
            }
            Logger.LogDebug("Timedout, sending the response FAIL");
            return new ReadResp() { Balance = -1};
        }
    }
}
