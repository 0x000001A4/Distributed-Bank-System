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
                Logger.LogDebug("End of ReadBalance");
                return Task.FromResult(response);                     //Rick Ve Isto
            }
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
