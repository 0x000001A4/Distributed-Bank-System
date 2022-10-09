
using BoneyServer.domain;
using System;
using BoneyServer.services;
using BoneyServer.utils;
using Grpc.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Grpc.Core.Interceptors;

namespace BoneyServer
{

    public class BoneyServerMessageInterceptor : Interceptor {

		public BoneyServerState _state;

		public BoneyServerMessageInterceptor(BoneyServerState state) {
			_state = state;
		}

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
			TRequest request,
			ServerCallContext context,
			UnaryServerMethod<TRequest, TResponse> continuation)
		{
            try {
                if (state.isFrozen()) {
                    state.enqueue((Request)(object)(request));
                }
                return await continuation(request, context);

			} catch (Exception ex) {
                Console.WriteLine(ex);
				throw ex;
			}
		}
	}

    public class BoneyServer {
        public static async void Main(string[] args) // TODO - edit to receive all server state through the config file
        {
            
            ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(args[0]);
            uint processId = uint.Parse(args[1]);
            uint maxSlots = (uint)config.GetNumberOfSlots();
            (string hostname, int port) = config.GetBoneyHostnameAndPortByProcess((int)processId);

            BoneyServerState boneyServerState = new BoneyServerState(processId, config);
            CompareAndSwapServiceImpl compareAndSwapService = new CompareAndSwapServiceImpl(boneyServerState);

            Server server = new Server {
                Services = { CompareAndSwapService.BindService(compareAndSwapService).Intercept(new BoneyServerMessageInterceptor(boneyServerState)) },
                Ports = { new ServerPort(hostname, port, ServerCredentials.Insecure) }
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
