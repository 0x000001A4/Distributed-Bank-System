
using BoneyServer.domain;
using BoneyServer.services;
using BoneyServer.utils;
using System;
using Grpc.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Grpc.Core.Interceptors;

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
			try {
				if (_state.isFrozen()) {
					Type requestType = typeof(TRequest);
					Message? _msg = null;

                    if (requestType == typeof(CompareAndSwapRequest)) {
						_msg = new Message((CompareAndSwapRequest)(object) request, 1);
					}
                    if (requestType == typeof(PrepareRequest)) {
                        _msg = new Message((PrepareRequest)(object)request, 2);
                    }
                    if (requestType == typeof(AcceptRequest)) {
                        _msg = new Message((AcceptRequest)(object)request, 3);
                    }

					if (_msg != null) _state.enqueue(_msg);
					else Console.WriteLine("Error: Can't queue message because it does not belong to any of specified types.");
				}

				return await continuation(request, context);

			} catch (Exception ex) {
				Console.WriteLine(ex);
				throw;
			}
		}
	}
	public class BoneyServer
	{

		public static void Main(string[] args) // TODO - edit to receive all server state through the config file
		{
			ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
			uint processID = uint.Parse(args[1]);
			uint maxSlots = (uint)config.GetNumberOfSlots();
			(string hostname, int port) = config.GetBoneyHostnameAndPortByProcess((int)processID);
			BoneySlotManager slotManager = new BoneySlotManager(maxSlots);
            IMultiPaxos multiPaxos = new Paxos(processID, maxSlots, config.GetBoneyPortsAndAdress());


            ServerPort serverPort;
            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);

            CompareAndSwapServiceImpl compareAndSwapServiceImpl = new CompareAndSwapServiceImpl(slotManager, multiPaxos);

			Server server = new Server {
				Services = { CompareAndSwapService.BindService(compareAndSwapServiceImpl).Intercept(new BoneyServerMessageInterceptor(boneyServerState)) },
				Ports = { serverPort }
			};

			server.Start();

			string startupMessage = $"Started Boney server {processId} at hostname {hostname}:{port}";
			Console.WriteLine(startupMessage);

			//Configuring HTTP for client connections in Register method
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
			while (true) ;

			//server.ShutdownAsync().Wait();
		}

	}

}
