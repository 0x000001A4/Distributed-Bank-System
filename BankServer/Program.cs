using Grpc.Net.Client;
using BankServer.domain;
using BankServer.utils;
using BankServer.services;
using Grpc.Core;
using Grpc.Core.Interceptors;
using BankServer.domain.bank;
using Microsoft.Extensions.Logging;

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
            // Wait while first slot did not start
            while (!_state.isConfigured()) ;

            if (_state.IsFrozen())
            {
                Logger.LogDebug("Server is frozen");
                Type requestType = typeof(TRequest);
                Message? _msg = null;

                // Handling Compare And Swap Responses sent by learners
                if (requestType == typeof(CompareAndSwapResp))
                {
                    CompareAndSwapResp req = (CompareAndSwapResp)(object)request;
                    _msg = new Message(req, req.Sender);
                }

                else if (requestType == typeof(DepositReq))
                {
                    Logger.LogDebug("Interceptor: caught a Deposit message");
                    DepositReq req = (DepositReq)(object)request;
                    _msg = new Message(req, req.Sender);
                }

                else if (requestType == typeof(WithdrawReq))
                {
                    WithdrawReq req = (WithdrawReq)(object)request;
                    _msg = new Message(req, req.Sender);
                    Logger.LogError($"Interceptor: caught a Withdraw message from {req.Sender}");
                }

                else if (requestType == typeof(ReadReq))
                {
                    Logger.LogDebug("Interceptor: caught a Read message");
                    ReadReq req = (ReadReq)(object)request;
                    _msg = new Message(req, req.Sender);
                }

                else if (requestType == typeof(ListPendingRequestsReq))
                {
                    Logger.LogDebug("Interceptor: caught a ListPendingRequestReq message");
                    ListPendingRequestsReq req = (ListPendingRequestsReq)(object)request;
                    _msg = new Message(req, req.Sender);
                }

                else if (requestType == typeof(ProposeReq))
                {
                    Logger.LogDebug("Interceptor: caught a ProposeReq message");
                    ProposeReq req = (ProposeReq)(object)request;
                    _msg = new Message(req, req.Sender);
                }

                else if (requestType == typeof(CommitReq))
                {
                    Logger.LogDebug("Interceptor: caught a CommitReq message");
                    CommitReq req = (CommitReq)(object)request;
                    _msg = new Message(req, req.Sender);
                }

                if (_msg != null) _state.Enqueue(_msg);

                else Logger.LogError("Interceptor: Can't queue message because it does not belong to any of specified types. (l. 39)");
            }
            return await continuation(request, context);
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
            ITwoPhaseCommit twoPhaseCommit = new TwoPhaseCommit(config);
            BankServerState bankServerState = new BankServerState(int.Parse(args[1]), config, cmdHandler, twoPhaseCommit);

            object _lock = new object();
            BankServiceImpl _bankService = new BankServiceImpl(twoPhaseCommit, bankServerState);
            ClientServiceImpl _clientService = new ClientServiceImpl(config, bankManager, twoPhaseCommit, bankServerState, _lock);
            PaxosResultHandlerServiceImpl _paxosResultHandlerService = new PaxosResultHandlerServiceImpl(bankServerState, _lock);
            cmdHandler.AddPaxosResultHandlerService(_paxosResultHandlerService);
            cmdHandler.AddClientService(_clientService);
            cmdHandler.AddBankService(_bankService);

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
                    BankService.BindService(_bankService).Intercept(_interceptor)
                },
                
                Ports = { serverPort }
            };
            bankServerState.AddServer(server);

            SlotTimer slotTimer = new SlotTimer(bankServerState, (uint)config.GetSlotDuration(),
                config.GetSlotFisrtTime(),(uint)config.GetNumberOfSlots(), config);
            slotTimer.Execute();

            server.Start();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            while (true);
        }
    }
}
