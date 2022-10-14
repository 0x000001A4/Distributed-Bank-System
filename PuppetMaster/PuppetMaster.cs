using PuppetMaster.utils;
using System;


using System.Diagnostics;

namespace PuppetMaster
{
    public class PuppetMaster
    {

        public static void Main(string[] args)
        {
            Logger.LogInfo("Puppet master starting...");
            try
            {
                string title      = "";
                string serverType = "BoneyServer";
                string appName    = $"{serverType}.exe";
                string path           = $"..\\..\\..\\..\\{serverType}\\bin\\Debug\\net6.0";
                string configFilePath = $"..\\..\\..\\..\\{serverType}\\configuration_sample.txt";
                ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(configFilePath);

                Process p;
                int numberOfBoneyServers = config.GetNumberOfBoneyServers();

                checkIfFileExists($"{path}\\{appName}");
                checkIfFileExists(configFilePath);

                Logger.LogInfo($"Initializing {numberOfBoneyServers} Boney servers:");
                foreach(int processID in config.GetBoneyServerIDs())
                {
                    title = $"Boney{processID}";
                    Logger.LogInfo($"\tBoney server {processID} initialized.");
                    p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {configFilePath} {processID}";

                    p.Start();
                }

                serverType = "BankServer";
                Logger.LogInfo($"Initializing {1} Bank servers");
                title = $"Bank1";
                appName = $"{serverType}.exe";
                path = $"..\\..\\..\\..\\{serverType}\\bin\\Debug\\net6.0";
                configFilePath = $"..\\..\\..\\..\\{serverType}\\configuration_sample.txt";

                checkIfFileExists($"{path}\\{appName}");
                checkIfFileExists(configFilePath);

                p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {configFilePath} {4}";

                p.Start();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return;
            }

        }

        private static void checkIfFileExists(string path)
        {
            if (!File.Exists(path)) throw new Exception($"File {path} does not exist! Make sure the solution is compiled.");
        }


    }
}

