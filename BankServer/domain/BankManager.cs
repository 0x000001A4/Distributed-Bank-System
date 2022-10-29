using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.domain
{
    public class BankManager {
       
        private Dictionary<int, double> _clientLogic = new Dictionary<int, double>();

        public BankManager() {}

        public string Deposit(int client_id, double value) {
            lock (this)
            {
                double final_value;
                if (_clientLogic.TryGetValue(client_id, out final_value))
                {
                    _clientLogic.Add(client_id, final_value + value);
                    return "SUCESS";
                }
                return "FAIL";
            }
           
        }

        public string Withdraw(int client_id, double value) {
            lock (this)
            {
                double final_value;
                if (_clientLogic.TryGetValue(client_id, out final_value))
                {
                    if (final_value >= value)
                    {
                        _clientLogic.Add(client_id, final_value - value);
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
                if (_clientLogic.TryGetValue(client_id, out final_value))
                {

                    return final_value;
                    
                } 
                return -1; //Client does not exist impossible situation
            }
        }

    }
}
