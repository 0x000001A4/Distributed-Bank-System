using Grpc.Net.Client;
using BankServer.domain;
using BankServer.utils;
using BankServer.services;
using Grpc.Core;
using Grpc.Core.Interceptors;
using BankServer.domain.bank;

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
                while (!_state.isConfigured()) ;

                if (_state.IsFrozen())
                {
                    Type requestType = typeof(TRequest);
                    Message? _msg = null;
                    string sender = context.Peer;

                    // Handling Compare And Swap Responses sent by learners
                    if (requestType == typeof(CompareAndSwapResp)) {
                        _msg = new Message((CompareAndSwapResp)(object)request, sender);
                    }

                    else if (requestType == typeof(DepositReq)) {
                        _msg = new Message((DepositReq)(object)request, sender);
                    }

                    else if (requestType == typeof(WithdrawReq)) {
                        _msg = new Message((WithdrawReq)(object)request, sender);
                    }

                    else if (requestType == typeof(ReadReq)) {
                        _msg = new Message((ReadReq)(object)request, sender);
                    }

                    if (_msg != null) _state.Enqueue(_msg);

                    else Logger.LogError("Interceptor: Can't queue message because it does not belong to any of specified types. (l. 39)");
                }
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                Logger.LogError("Interceptor:" + ex.Message + " (l. 49)");
                throw;
            }
        }
    }


    public class BankServer
    {
        public static void Main(string[] args) 
        {
            Logger.DebugOn();
            Logger.LogInfo("Bank Server started");
            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            BankManager bankManager = new BankManager();
            int processID = int.Parse(args[1]);

            QueuedCommandHandler cmdHandler = new QueuedCommandHandler();
            BankSlotManager bankSlotManager = new BankSlotManager(config);
            ITwoPhaseCommit twoPhaseCommit = new TwoPhaseCommit(config);
            BankServerState bankServerState = new BankServerState(int.Parse(args[1]), config, cmdHandler, bankSlotManager, twoPhaseCommit);

            BankServiceImpl bankService = new BankServiceImpl(twoPhaseCommit, bankServerState);
            ClientServiceImpl _clientService = new ClientServiceImpl(config, bankManager, twoPhaseCommit, bankServerState);
            PaxosResultHandlerServiceImpl _paxosResultHandlerService = new PaxosResultHandlerServiceImpl(bankServerState);
            cmdHandler.AddPaxosResultHandlerService(_paxosResultHandlerService);
            cmdHandler.AddClientService(_clientService);

            (string hostname, int portNum) = config.GetBankHostnameAndPortByProcess(processID);

            BankServerStateInterceptor _interceptor = new BankServerStateInterceptor(bankServerState);

            ServerPort serverPort;
            Logger.LogDebug(hostname + ":" + portNum);
            serverPort = new ServerPort(hostname, portNum, ServerCredentials.Insecure);
            Server server = new Server
            {
                Services = {
                    CompareAndSwapService.BindService(_paxosResultHandlerService).Intercept(_interceptor),
                    ClientService.BindService(_clientService).Intercept(_interceptor),
                    BankService.BindService(bankService)
                },
                
                Ports = { serverPort }
            };
            bankServerState.AddServer(server);

            SlotTimer slotTimer = new SlotTimer(bankServerState, (uint)config.GetSlotDuration(),config.GetSlotFisrtTime());
            slotTimer.Execute();

            server.Start();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


            while (true);
        }
    }
}
