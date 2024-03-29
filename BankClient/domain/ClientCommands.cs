﻿using BankClient.utils;

namespace BankClient.domain
{
    public interface ICommand
    {
        string CommandName { get; set; }
        void Execute(uint executionOrder);
        string GetName()
        {
            return CommandName;
        }
    }
    public abstract class FrontendCommandContext
    {
        public static BankClientFrontend Frontend { get; set; }
        public static uint ClientID { get; set; }

    }
    public class ReadCommand : FrontendCommandContext, ICommand
    {
        public string CommandName { get; set; }
        public ReadCommand()
        {
            CommandName = "Read()";
        }

        public void Execute(uint executionOrder)
        {
            try 
            { 
                Frontend.ReadBalance(ClientID, executionOrder);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }

    public class DepositCommand : FrontendCommandContext, ICommand
    {
        public string CommandName { get; set; }
        private double _ammount;
        public DepositCommand(double ammount)
        {
            _ammount = ammount;
            CommandName = $"Deposit({ammount})";
        }
        public void Execute(uint executionOrder)
        {
            try
            {
                Frontend.Deposit(ClientID, executionOrder, _ammount);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }

    public class WithdrawCommand : FrontendCommandContext, ICommand
    {
        public string CommandName { get; set; }
        private double _ammount;
        public WithdrawCommand( double ammount)
        {
            _ammount = ammount;
            CommandName = $"Withdraw({ammount})";
        }
        public void Execute(uint executionOrder)
        {
            try 
            { 
                Frontend.Withdraw(ClientID, executionOrder, _ammount);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }

    public class WaitCommand : ICommand
    {
        public string CommandName { get; set; }
        private int _timeMillis;
        public WaitCommand(int timeMillis)
        {
            _timeMillis = timeMillis;
            CommandName = $"Wait({timeMillis})";
        }
        public void Execute(uint seqNumber)
        {
            Thread.Sleep(_timeMillis);
        }
    }


}
