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


                string folder = "BoneyServer";
                string path = $"..\\..\\..\\..\\{folder}\\bin\\Debug\\net6.0";
                string appName = $"{folder}.exe";
                string arg1 = $"..\\..\\..\\..\\{folder}\\configuration_sample.txt";
                Process p;
                string title = "";
                
                for (int i=0; i < 3; i++)
                {
                    title = $"Boney{i + 1}";
                    p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {arg1}";


                    p.Start();

                }

                folder = "BankServer";
                title = $"Bank1";
                p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/k start \"{title}\" {path}\\{appName} {arg1}";


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

