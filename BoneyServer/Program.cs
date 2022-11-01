using BoneyServer.domain;
using BoneyServer.domain.paxos;
using BoneyServer.services;
using BoneyServer.utils;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Diagnostics;

namespace BoneyServer {

    public class BoneyServerMessageInterceptor : Interceptor {

		public BoneyServerState _state;

		public BoneyServerMessageInterceptor(BoneyServerState state) {
			_state = state;
		}

		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
			TRequest request,
			ServerCallContext context,
			UnaryServerMethod<TRequest, TResponse> continuation) {
			
				while (!_state.isConfigured()) ;

				if (_state.IsFrozen()) {
					Type requestType = typeof(TRequest);
					Message? _msg = null;
					string sender = context.Peer;

                    if (requestType == typeof(CompareAndSwapReq)) {
						_msg = new Message((CompareAndSwapReq)(object) request, sender);
					}
                    else if (requestType == typeof(PrepareReq)) {
                        _msg = new Message((PrepareReq)(object) request, sender);
                    }
                    else if (requestType == typeof(AcceptReq)) {
                        _msg = new Message((AcceptReq)(object) request, sender);
                    }
					else if (requestType == typeof(LearnCommandReq)) {
						_msg = new Message((LearnCommandReq)(object) request, sender);
					}

					if (_msg != null) _state.Enqueue(_msg);

					else Logger.LogError("Interceptor: Can't queue message because it does not belong to any of specified types. (l. 39)");
				}

				return await continuation(request, context);

			
		}
	}
	public class BoneyServer
	{
		public static void Main(string[] args) // TODO - edit to receive all server state through the config file
		{
			Logger.DebugOn();
			ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
			uint processID = uint.Parse(args[1]);
			uint maxSlots = (uint)config.GetNumberOfSlots();
			(string hostname, int port) = config.GetBoneyHostnameAndPortByProcess((int)processID);


            ServerPort serverPort;
            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);

            IMultiPaxos multiPaxos = new Paxos(processID, maxSlots, config.GetBoneyServersPortsAndAddresses());

			QueuedCommandHandler cmdHandler = new QueuedCommandHandler(multiPaxos);
            BoneyServerState boneyServerState = new BoneyServerState(processID, multiPaxos, config, cmdHandler);

			CompareAndSwapServiceImpl _casService = new CompareAndSwapServiceImpl(boneyServerState, multiPaxos);

			PaxosAcceptorServiceImpl _paxosAcceptorService = new PaxosAcceptorServiceImpl(boneyServerState, multiPaxos);
			PaxosLearnerServiceImpl _paxosLearnerService = new PaxosLearnerServiceImpl(boneyServerState,
				multiPaxos, config.GetBankServersPortsAndAddresses(), (uint)config.GetNumberOfBoneyServers());

			cmdHandler.AddCompareAndSwapService(_casService);
			cmdHandler.AddPaxosAcceptorService(_paxosAcceptorService);
			cmdHandler.AddPaxosLearnerService(_paxosLearnerService);


			BoneyServerMessageInterceptor _interceptor = new BoneyServerMessageInterceptor(boneyServerState);

            Server server = new Server {
                Services = {
                  CompareAndSwapService.BindService(_casService).Intercept(_interceptor),
                  PaxosAcceptorService.BindService(_paxosAcceptorService).Intercept(_interceptor),
				  PaxosLearnerService.BindService(_paxosLearnerService).Intercept(_interceptor)
				         },
                Ports = { serverPort }
            };
			boneyServerState.AddServer(server);

            SlotTimer slotTimer = new SlotTimer(boneyServerState, (uint)config.GetSlotDuration(),
				config.GetSlotFisrtTime(),(uint)config.GetNumberOfSlots());
            slotTimer.Execute();

			string startupMessage = $"Started Boney server {processID} at hostname {hostname}:{port}";
            server.Start();
			Logger.LogInfo(startupMessage);
			
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

			while (true);
		}

	}

}
