using BankClient.domain;

namespace BankClient.utils
{
    public class ClientCommands
    {
        public const string READ_COMMAND     = "R";
        public const string DEPOSIT_COMMAND  = "D";
        public const string WITHDRAW_COMMAND = "W";
        public const string WAIT_COMMAND     = "S";
    }
    public class ClientConfiguration
    {
        public List<ICommand> Commands;
        public ClientConfiguration(List<ICommand> commands)
        {
            Commands = commands;
        }

        public static ClientConfiguration ReadConfigFromFile(string filePath)
        {
            List<ICommand> commands = new List<ICommand>();
            string[] lines = File.ReadAllLines(filePath);
            string[] words;
            foreach (string line in lines)
            {
                words = line.Split(' ');
                try
                {
                    switch (words[0])
                    {
                        case ClientCommands.DEPOSIT_COMMAND:
                            double depositAmmount = double.Parse(words[1].Replace('.', ','));
                            commands.Add(new DepositCommand(depositAmmount));
                            break;
                        case ClientCommands.WITHDRAW_COMMAND:
                            double withdrawalAmmount = double.Parse(words[1].Replace('.', ','));
                            commands.Add(new WithdrawCommand(withdrawalAmmount));
                            break;
                        case ClientCommands.READ_COMMAND:
                            commands.Add(new ReadCommand());
                            break;
                        case ClientCommands.WAIT_COMMAND:
                            int timeMillis = int.Parse(words[1]);
                            Logger.LogDebug("Time to wait is : " + timeMillis);
                            commands.Add(new WaitCommand(timeMillis));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("Please use the correct format for commands:" +
                        "\n\tD double" +
                        "\n\tW double" +
                        "\n\tR" +
                        "\n\tS int");
                    throw e;
                }

            }
            return new ClientConfiguration(commands);
        }
    }
}
