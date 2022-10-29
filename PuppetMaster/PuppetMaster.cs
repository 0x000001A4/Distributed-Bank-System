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
            public readonly string BaseConfigPath;
            private readonly bool  _isClient;

            public string Title { get; set; }

            public ProcessInfo(string serverType, string configFile, bool isClient = false) {
                ServerType  = serverType;
                AppName     = $"{serverType}.exe";
                Path             = $"..\\..\\..\\..\\{serverType}\\bin\\Debug\\net6.0";
                ConfigFilePath   = $"..\\..\\..\\..\\{serverType}\\{configFile}.txt";
                BaseConfigPath = $"..\\..\\..\\..\\{serverType}\\";
                _isClient = isClient;
                CheckIfFileExists($"{Path}\\{AppName}");
                CheckIfFileExists(ConfigFilePath);
            }

            public bool IsClient() { return _isClient;  }

            public string GetClientScriptPath(string config) {
                return BaseConfigPath + config + ".txt";
            }

            public void CheckIfFileExists(string path)
            {
                if (!File.Exists(path)) throw new Exception($"File {path} does not exist! Make sure the solution is compiled.");
            }

        }


        public static void Main(string[] args)
        {
            Logger.LogInfo("Puppet master starting...");
            Logger.NewLine();
            Logger.DebugOn();

            ProcessInfo BoneyInitInfo = new ProcessInfo("BoneyServer", "configuration_sample");
            ServerConfiguration Boneyconfig = ServerConfiguration.ReadConfigFromFile(BoneyInitInfo.ConfigFilePath);

            ProcessInfo BankInitInfo = new ProcessInfo("BankServer", "configuration_sample");
            ServerConfiguration Bankconfig = ServerConfiguration.ReadConfigFromFile(BankInitInfo.ConfigFilePath);

            ProcessInfo ClientInitInfo = new ProcessInfo("BankClient", "configuration_sample", true);
            ServerConfiguration Clientconfig = ServerConfiguration.ReadConfigFromFile(ClientInitInfo.ConfigFilePath);

            checkIfClientScriptFileExists(Clientconfig, ClientInitInfo);

            Logger.LogInfo($"Initializing {Boneyconfig.GetNumberOfBoneyServers()} Boney servers:");
            initializeServers(Boneyconfig.GetBoneyServerIDs(), BoneyInitInfo);

            Logger.LogInfo($"Initializing {Bankconfig.GetNumberOfBankServers()} Bank servers");
            initializeServers(Bankconfig.GetBankServerIDs(), BankInitInfo);

            Logger.LogInfo($"Initializing {Clientconfig.GetNumberOfClients()} Client servers");
            initializeServers(Clientconfig.GetClientIDs(), ClientInitInfo, Clientconfig);

        }

        private static void checkIfClientScriptFileExists(ServerConfiguration config, ProcessInfo info)
        {
            foreach(var client in config.GetClientIDs())
            {
                info.CheckIfFileExists(info.GetClientScriptPath(config.GetClientScriptNameById(client)));
            }
        }



        private static void initializeServers(List<int> processIDs, ProcessInfo initInfo, ServerConfiguration config = null)
        {
            foreach (int processID in processIDs)
            {
                initInfo.Title = $"{initInfo.ServerType} {processID}";
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";

                if (initInfo.IsClient())
                {
                    string clientConfigName = config.GetClientScriptNameById(processID);
                    string clientConfigPath = initInfo.GetClientScriptPath(clientConfigName);
                    p.StartInfo.Arguments = $"/k start \"{initInfo.Title}\" {initInfo.Path}\\{initInfo.AppName}" + 
                        $" {initInfo.ConfigFilePath} {clientConfigPath} {processID}";   
                }
                else
                {
                    p.StartInfo.Arguments = $"/k start \"{initInfo.Title}\" {initInfo.Path}\\{initInfo.AppName}" + 
                        $" {initInfo.ConfigFilePath} {processID}";
                }
                p.Start();

                Logger.LogInfo($"\t{initInfo.Title}  initialized.");
            }
            Logger.NewLine();

        }

    }
}

