using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.domain
{
    public class BankManager {
        private Dictionary<int, int> _clientLogic = new Dictionary<int, int>();

        public BankManager() {}

        public bool deposit(int client_id, int value) {
            int final_value;
            if(_clientLogic.TryGetValue(client_id, out final_value))
            {
                _clientLogic.Add(client_id, final_value+value);
                return true;
            }
            return false;
                
        }

        public bool withdrawal(int client_id, int value) {
            int final_value;
            if (_clientLogic.TryGetValue(client_id, out final_value))
            {
                if (final_value >= value) {
                    _clientLogic.Add(client_id, final_value - value);
                    return true;
                }
            }
            return false;
        }

    }
}
