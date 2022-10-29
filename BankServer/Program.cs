using Grpc.Net.Client;
using BankServer.domain;
using BankServer.utils;
using BankServer.services;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BankServer
{
    public class BankServerStateInterceptor : Interceptor
    {
        public BankServerState _state;

        public BankServerStateInterceptor(BankServerState state)
        {
            _state = state;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                if (!_state.isConfigured()) throw new Exception("Server is not configured yet, don't make requests.");

                if (_state.IsFrozen())
                {
                    Type requestType = typeof(TRequest);
                    Message? _msg = null;
                    string sender = context.Peer;

                    if (requestType == typeof(CompareAndSwapResp))
                    {
                        _msg = new Message((CompareAndSwapResp)(object)request, sender);
                    }

                    if (_msg != null) _state.Enqueue(_msg);

                    else Logger.LogError("Interceptor: Can't queue message because it does not belong to any of specified types. (l. 39)");
                }

                return await continuation(request, context);

            }
            catch (Exception ex)
            {
                Logger.LogError("Interceptor:" + ex.Message + " (l. 45)");
                throw ex;
            }
        }
    }


    public class Program
    {
        public static void Main(string[] args) 
        {
            Logger.DebugOn();
            Logger.LogInfo("Bank Server started");
            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            BankManager bankManager = new BankManager();


            QueuedCommandHandler cmdHandler = new QueuedCommandHandler();
            BankServerState bankServerState = new BankServerState(int.Parse(args[1]), config, cmdHandler);

            SlotTimer slotTimer = new SlotTimer(bankServerState, (uint)config.GetSlotDuration(),config.GetSlotFisrtTime());
            slotTimer.Execute();

            PaxosResultHandlerServiceImpl _paxosResultHandlerService = new PaxosResultHandlerServiceImpl(bankServerState);
            cmdHandler.AddPaxosResultHandlerService(_paxosResultHandlerService);

            int processID = int.Parse(args[1]);
            (string hostname, int portNum) = config.GetBankHostnameAndPortByProcess(processID);
            Logger.LogDebug(hostname + ":" + portNum);

            BankServerStateInterceptor _interceptor = new BankServerStateInterceptor(bankServerState);

            ServerPort serverPort;
            serverPort = new ServerPort(hostname, portNum, ServerCredentials.Insecure);
            Server server = new Server
            {
                Services = {
                  CompareAndSwapService.BindService(_paxosResultHandlerService).Intercept(_interceptor)
				         },
                Ports = { serverPort }
            };
            server.Start();

            while (true);
        }
    }
}
