using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.utils
{
    public class Logger
    {
        private static object mutex = new Object();
        private static bool _debug = false;
        private static string INFO = "INFO:";
        private static string ERROR = "ERROR:";
        private static string DEBUG = "DEBUG:";
        private static string EVENT = "EVENT:";
        private static string PROPOSER = "PROPOSER:";
        private static string ACCEPTOR = "ACCEPTOR:";
        private static string LEARNER = "LEARNER:";

        public static void DebugOn()
        {
            _debug = true;
        }

        private static int getThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }


        public static void LogInfo(string message)
        {
            lock (mutex)
            {
                setColors(ConsoleColor.Cyan, ConsoleColor.Black);
                Console.Write(INFO + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                setColors(ConsoleColor.Black, ConsoleColor.Cyan);
                Console.WriteLine(" " + message);
                setDefaultColors();
            }

        }

        public static void LogError(string message)
        {
            lock (mutex)
            {
                setColors(ConsoleColor.Red, ConsoleColor.White);
                Console.Write(ERROR + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                setColors(ConsoleColor.Black, ConsoleColor.Red);
                Console.WriteLine(" " + message);
                setDefaultColors();
            }
        }

        public static void LogDebug(string message)
        {
            lock (mutex)
            {
                if (_debug)
                {
                    setColors(ConsoleColor.DarkGray, ConsoleColor.Black);
                    Console.Write(DEBUG + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                    setColors(ConsoleColor.Black, ConsoleColor.DarkGray);
                    Console.WriteLine(" " + message);
                    setDefaultColors();
                }
            }

        }

        public static void LogDebugProposer(string message)
        {
            lock (mutex)
            {
                if (_debug)
                {
                    setColors(ConsoleColor.Green, ConsoleColor.White);
                    Console.Write(PROPOSER + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                    setColors(ConsoleColor.Black, ConsoleColor.Green);
                    Console.WriteLine(" " + message);
                    setDefaultColors();
                }
            }
        }

        public static void LogDebugAcceptor(string message)
        {
            lock (mutex)
            {
                if (_debug)
                {
                    setColors(ConsoleColor.Yellow, ConsoleColor.Black);
                    Console.Write(ACCEPTOR + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                    setColors(ConsoleColor.Black, ConsoleColor.Yellow);
                    Console.WriteLine(" " + message);
                    setDefaultColors();
                }
            }
        }

        public static void LogDebugLearner(string message)
        {
            lock (mutex)
            {
                if (_debug)
                {
                    setColors(ConsoleColor.Magenta, ConsoleColor.White);
                    Console.Write(LEARNER + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                    setColors(ConsoleColor.Black, ConsoleColor.Magenta);
                    Console.WriteLine(" " + message);
                    setDefaultColors();
                }
            }
        }

        public static void LogEvent(string message)
        {
            lock (mutex)
            {
                setColors(ConsoleColor.DarkYellow, ConsoleColor.White);
                Console.Write(EVENT + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                setColors(ConsoleColor.Black, ConsoleColor.DarkYellow);
                Console.WriteLine(" " + message);
                setDefaultColors();
            }
        }

        public static void LogDebug2PC(string message)
        {
            lock (mutex)
            {
                if (_debug)
                {
                    setColors(ConsoleColor.Magenta, ConsoleColor.White);
                    Console.Write(DEBUG + getThreadId() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff") + ":");
                    setColors(ConsoleColor.Black, ConsoleColor.Magenta);
                    Console.WriteLine(" " + message);
                    setDefaultColors();
                }
            }
        }

        public static void NewLine()
        {
            Console.WriteLine();
        }

        private static void setDefaultColors()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void setColors(ConsoleColor background, ConsoleColor text)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = text;
        }
    }
}
