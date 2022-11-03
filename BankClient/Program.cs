﻿using BankClient.domain;
using BankClient.utils;
using BankClient.services;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Globalization;

namespace BankClient
{

    public class BankClient
	{
		public static void Main(string[] args)
		{
			Logger.DebugOn();
			ServerConfiguration globalConfig;
			ClientConfiguration clientConfig;
			uint clientID;

            try
            {
                globalConfig = ServerConfiguration.ReadConfigFromFile(args[0]);
				clientConfig = ClientConfiguration.ReadConfigFromFile(args[1]);
				clientID = uint.Parse(args[2]);
			}
			catch(Exception e)
			{
				Logger.LogError("Client process expected 3 input arguments:\n\t" +
					"generalConfigFilePath(string) clientScriptPath(string) clientID(int/uint)");
				throw;
			}

			Logger.LogInfo($"Starting Bank Client {clientID}");
			TimeoutTimer.SetTimeout(globalConfig.GetSlotDuration());
			ClientLogic clientLogic = new ClientLogic(clientConfig.Commands, globalConfig, clientID);

            (string hostname, int portNum) = globalConfig.GetClientHostnameAndPortByProcess((int)clientID);

			ClientServiceImpl _clientServiceImpl = new ClientServiceImpl();

            ServerPort serverPort;
            Logger.LogDebug(hostname + ":" + portNum);
            serverPort = new ServerPort(hostname, portNum, ServerCredentials.Insecure);
            Server server = new Server
            {
				Services = {
                    ClientService.BindService(_clientServiceImpl),
                },

                Ports = { serverPort }
            };

			server.Start();
            clientLogic.Start();

			Logger.LogInfo("All commands executed");
			while (true);
		}

	}

	public class ClientLogic
    {
		private List<ICommand> _commands;
		private int _timeToSleep;
		public ClientLogic(List<ICommand> commands, ServerConfiguration globalConfig, uint clientID)
        {
			_commands = commands;
			FrontendCommandContext.Frontend = new BankClientFrontend(globalConfig);
			FrontendCommandContext.ClientID = clientID;

			DateTime dateTime = DateTime.ParseExact(globalConfig.GetSlotFisrtTime(), "HH:mm:ss",
							CultureInfo.InvariantCulture);
			var span = dateTime - DateTime.Now;
			_timeToSleep = (int)span.TotalMilliseconds + globalConfig.GetSlotDuration();

        }

        public void Start()
        {
			sleepUntilStartTime();
			uint executionOrder = 1;
			foreach(var command in _commands)
            {
				try
				{
					Logger.LogDebug(command.ToString());
                    command.Execute(executionOrder);
				}catch(Exception e){
					Logger.LogError(e.Message);
				}
                Logger.LogInfo(command.GetName() + " executed");
                executionOrder++;
            }
        }

		private void sleepUntilStartTime()
        {
			Thread.Sleep(_timeToSleep);
		}
    }

}

