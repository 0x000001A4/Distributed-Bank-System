using PuppetMaster.utils;
using System;


using System.Diagnostics;

namespace PuppetMaster
{
    public class PuppetMaster
    {
        private class ProcessInfo
        {
            public readonly string AppName;
            public readonly string ServerType;
            public readonly string Path;
            public readonly string ConfigFilePath;
            public string Title { get; set; }

            public ProcessInfo(string serverType, string configFile) {
                ServerType  = serverType;
                AppName     = $"{serverType}.exe";
                Path           = $"..\\..\\..\\..\\{serverType}\\bin\\Debug\\net6.0";
                ConfigFilePath = $"..\\..\\..\\..\\{serverType}\\{configFile}.txt";
                checkIfFileExists($"{Path}\\{AppName}");
                checkIfFileExists(ConfigFilePath);
            }

            private void checkIfFileExists(string path)
            {
                if (!File.Exists(path)) throw new Exception($"File {path} does not exist! Make sure the solution is compiled.");
            }

        }


        public static void Main(string[] args)
        {
            ProcessInfo initInfo;
            ServerConfiguration config;
            Logger.LogInfo("Puppet master starting...");

            initInfo = new ProcessInfo("BoneyServer", "configuration_sample");
            config = ServerConfiguration.ReadConfigFromFile(initInfo.ConfigFilePath);
            Logger.LogInfo($"Initializing {config.GetNumberOfBoneyServers()} Boney servers:");
            initializeServers(config.GetBoneyServerIDs(), initInfo);

            initInfo = new ProcessInfo("BankServer", "configuration_sample");
            config = ServerConfiguration.ReadConfigFromFile(initInfo.ConfigFilePath);
            Logger.LogInfo($"Initializing {config.GetNumberOfBankServers()} Bank servers");
            initializeServers(config.GetBankServerIDs(), initInfo);

            initInfo = new ProcessInfo("BankClient", "configuration_sample");
            config = ServerConfiguration.ReadConfigFromFile(initInfo.ConfigFilePath);
            Logger.LogInfo($"Initializing {config.GetNumberOfClients()} Client servers");
            //initializeServers(config.GetClientIDs(), initInfo);

        }



        private static void initializeServers(List<int> processIDs, ProcessInfo initInfo)
        {
            foreach (int processID in processIDs)
            {
                initInfo.Title = $"{initInfo.ServerType} {processID}";
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/k start \"{initInfo.Title}\" {initInfo.Path}\\{initInfo.AppName} {initInfo.ConfigFilePath} {processID}";
                p.Start();

                Logger.LogInfo($"{initInfo.Title}  initialized.");
            }

        }

    }
}

