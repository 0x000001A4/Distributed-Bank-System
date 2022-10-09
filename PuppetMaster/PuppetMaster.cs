using PuppetMaster.utils;
using System;


using System.Diagnostics;

namespace PuppetMaster
{
    public class PuppetMaster
    {

        public static void Main(string[] args)
        {
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
                Console.WriteLine("Puppet master starting...");

                Console.WriteLine($"Initializing {numberOfBoneyServers} Boney servers");
                for (int processID = 1; processID <= numberOfBoneyServers; processID++)
                {
                    title = $"Boney{processID}";
                    p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {configFilePath} {processID}";

                    p.Start();
                }

                serverType = "BankServer";

                Console.WriteLine($"Initializing {1} Bank servers");
                title = $"Bank1";
                p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {configFilePath}";

                p.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred!!!: " + ex.Message);
                return;
            }

        }
    }
}

