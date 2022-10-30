using BankServer.utils;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.domain
{
    public class BankManager {
       
        
       private Dictionary<int, double> _clientLogic = new Dictionary<int, double>();
        public BankManager() {}


        public void registerClient(int id)
        {
            double final;
            if(!_clientLogic.TryGetValue(id, out final)) _clientLogic.Add(id, 0);
          
        }
        public string Deposit(int client_id, double value) {
            lock (this)
            {
                double final_value;
                registerClient(client_id);
                if (_clientLogic.TryGetValue(client_id, out final_value))
                {
                     _clientLogic[client_id] = final_value + value;
                    Logger.LogDebug("Deposit operaion from Client: " + client_id + " with value : " + value);
                    _clientLogic.TryGetValue(client_id, out final_value);
                    Logger.LogDebug("New Balance: " + final_value);
                    return "SUCESS";
                }
               
                return "FAIL";
            }
           
        }
       
        public string Withdraw(int client_id, double value) {
            lock (this)
            {
                
                double final_value;
                registerClient(client_id);
                if (_clientLogic.TryGetValue(client_id, out final_value))
                {
                    Logger.LogDebug("Actual Balance: " + final_value);
                    if (final_value >= value)
                    {
                        Logger.LogDebug("In withdraw operation");

                        _clientLogic[client_id] = final_value - value;
                        Logger.LogDebug("Withdraw operaion from Client: " + client_id + " with value : " + value);
                        _clientLogic.TryGetValue(client_id, out final_value);
                        Logger.LogDebug("New Balance: " + final_value);
                        return "SUCESS";
                    }
                }
                return "FAIL";
            }
        }


        public double Read(int client_id)
        {
            lock (this)
            {
                double final_value;
                registerClient(client_id);
                if (_clientLogic.TryGetValue(client_id, out final_value))
                {
                    Logger.LogDebug("Read operaion from Client: " + client_id + " with value : " + final_value);
                    return final_value;
                    
                } 
                return -1; //Client does not exist impossible situation
            }
        }

    }
}
