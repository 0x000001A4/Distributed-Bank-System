using BankClient.domain;
using BankClient.utils;
using Grpc.Core;

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
				throw e;
			}
			Logger.LogInfo($"Starting Bank Client {clientID}");

			ClientLogic clientLogic = new ClientLogic(clientConfig.Commands, globalConfig);
			clientLogic.Start();

			Logger.LogInfo("All commands executed");
			while (true);
		}

	}

	public class ClientLogic
    {
		private List<ICommand> _commands;
		public ClientLogic(List<ICommand> commands, ServerConfiguration globalConfig)
        {
			_commands = commands;
			// TODO: delete from comment
			// FrontendCommand.SetFrontend(new BankClientFrontEnd(globalConfig));
        }

		public void Start()
        {
			int count = 1;
			foreach(var command in _commands)
            {
				Logger.LogInfo(command.GetName() + " executed");
				command.Execute();
				count++;
            }
        }
    }

}

