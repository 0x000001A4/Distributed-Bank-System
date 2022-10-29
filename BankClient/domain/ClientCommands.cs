using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClient.domain
{
    public interface ICommand
    {
        string CommandName { get; set; }
        void Execute();
        string GetName()
        {
            return CommandName;
        }
    }
    public abstract class FrontendCommand
    {
        protected static bool _frontend;
        public static void SetFrontend(bool frontend)
        {
            _frontend = frontend;
        }
    }
    public class ReadCommand : FrontendCommand, ICommand
    {
        public string CommandName { get; set; }
        public ReadCommand()
        {
            CommandName = "Read()";
        }

        public void Execute()
        {
            //_frontend.Read();
        }
    }

    public class DepositCommand : FrontendCommand, ICommand
    {
        public string CommandName { get; set; }
        private double _ammount;
        public DepositCommand(double ammount)
        {
            _ammount = ammount;
            CommandName = $"Deposit({ammount})";
        }
        public void Execute()
        {
            //_frontend.Deposit(_ammount);
        }
    }

    public class WithdrawCommand : FrontendCommand, ICommand
    {
        public string CommandName { get; set; }
        private double _ammount;
        public WithdrawCommand( double ammount)
        {
            _ammount = ammount;
            CommandName = $"Withdraw({ammount})";
        }
        public void Execute()
        {
            //_frontend.Withdraw(_ammount);
        }
    }

    public class WaitCommand : FrontendCommand, ICommand
    {
        public string CommandName { get; set; }
        private int _timeMillis;
        public WaitCommand(int timeMillis)
        {
            _timeMillis = timeMillis;
            CommandName = $"Wait({timeMillis})";
        }
        public void Execute()
        {
            Thread.Sleep(_timeMillis);
        }
    }


}
