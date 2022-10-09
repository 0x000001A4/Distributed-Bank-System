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
                
                Process p;
                ServerConfiguration config = ServerConfiguration.ReadConfigFromFile(configFilePath);
                int numberOfBoneyServers = config.GetNumberOfBoneyServers();
                

                for (int i=0; i < numberOfBoneyServers; i++)
                {
                    title = $"Boney{i + 1}";
                    p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {configFilePath}";

                    p.Start();
                }

                serverType = "BankServer";
                path = $"..\\..\\..\\..\\{serverType}\\bin\\Debug\\net6.0";
                appName = $"{serverType}.exe";
                title = $"Bank1";
                configFilePath = $"..\\..\\..\\..\\{serverType}\\configuration_sample.txt";
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

