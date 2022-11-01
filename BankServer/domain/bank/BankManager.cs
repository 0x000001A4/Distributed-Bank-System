using BankServer.utils;

namespace BankServer.domain.bank
{
    public class BankManager
    {

        private readonly int ONLY_CLIENT = 1;
        Dictionary<int, double> _clientLogic = new Dictionary<int, double>();
        public BankManager() { }


        public void registerClient(int id)
        {
            double final;
            if (!_clientLogic.TryGetValue(id, out final)) _clientLogic.Add(id, 0);

        }
        public string Deposit(double value)
        {
            lock (this)
            {
                double final_value;
                registerClient(ONLY_CLIENT);
                if (_clientLogic.TryGetValue(ONLY_CLIENT, out final_value))
                {
                    Logger.LogDebug("Deposit: Current Balance: " + final_value);
                    _clientLogic[ONLY_CLIENT] = final_value + value;
                    _clientLogic.TryGetValue(ONLY_CLIENT, out final_value);
                    Logger.LogDebug("Deposit: new balance " + _clientLogic[ONLY_CLIENT]);
                    return "SUCESS";
                }

                return "FAIL";
            }

        }

        public string Withdraw( double value)
        {
            lock (this)
            {

                double final_value;
                registerClient(ONLY_CLIENT);
                if (_clientLogic.TryGetValue(ONLY_CLIENT, out final_value))
                {
                    if (final_value >= value)
                    {
                        Logger.LogDebug("Withdraw: Actual Balance: " + final_value);
                        _clientLogic[ONLY_CLIENT] = final_value - value;
                        _clientLogic.TryGetValue(ONLY_CLIENT, out final_value);
                        Logger.LogDebug("Withdraw: new balance " + _clientLogic[ONLY_CLIENT]);
                        return "SUCESS";
                    }
                    Logger.LogDebug($"Withdraw: Insufficient balance: current: {final_value}, withdraw: {value}");
                }
                return "FAIL";
            }
        }


        public double Read()
        {
            lock (this)
            {
                double final_value;
                registerClient(ONLY_CLIENT);
                if (_clientLogic.TryGetValue(ONLY_CLIENT, out final_value))
                {
                    Logger.LogDebug("Read operaion from Client: " + ONLY_CLIENT + " with value : " + final_value);
                    return final_value;

                }
                return -1; //Client does not exist impossible situation
            }
        }

    }
}
