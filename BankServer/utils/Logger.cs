using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.utils
{
    public class Logger
    {
        private static bool _debug = false;
        private static string INFO = "INFO:";
        private static string ERROR = "ERROR:";
        private static string DEBUG = "DEBUG:";

        public static void DebugOn()
        {
            _debug = true;
        }

        public static void LogInfo(string message)
        {
            setColors(ConsoleColor.Cyan, ConsoleColor.Black);
            Console.Write(INFO);
            setColors(ConsoleColor.Black, ConsoleColor.Cyan);
            Console.WriteLine(" " + message);
            setDefaultColors();
        }

        public static void LogError(string message)
        {
            setColors(ConsoleColor.Red, ConsoleColor.White);
            Console.Write(ERROR);
            setColors(ConsoleColor.Black, ConsoleColor.Red);
            Console.WriteLine(" " + message);
            setDefaultColors();
        }

        public static void LogDebug(string message)
        {
            if (_debug)
            {
                setColors(ConsoleColor.DarkGray, ConsoleColor.Black);
                Console.Write(DEBUG);
                setColors(ConsoleColor.Black, ConsoleColor.DarkGray);
                Console.WriteLine(" " + message);
                setDefaultColors();
            }

        }

        private static void setDefaultColors()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        private static void setColors(ConsoleColor background, ConsoleColor text)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = text;
        }
    }
}
