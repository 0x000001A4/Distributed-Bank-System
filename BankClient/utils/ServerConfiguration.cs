using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BankClient.utils
{

    /// <summary>
    /// Stores any information needed for any type of server
    /// </summary>
    public class ServerConfiguration
    {
        private Dictionary<int, string> _boneyServersHostnames;
        private Dictionary<int, string> _bankServersHostnames;
        private string[,] _serverStatePerSlot;      // TODO - edit to dynamic structure
        private string[,] _serverSuspectedPerSlot;  // 
        private Dictionary<int, string> _clients;
        private string _timeOfFirstSlot;
        private int _numberOfSlots;
        private int _slotDuration;
        private bool _started = false;

        public ServerConfiguration() { }

        public static ServerConfiguration ReadConfigFromFile(string arg)
        {
            Dictionary<int, string> _boneyMap = new Dictionary<int, string>();
            Dictionary<int, string> _bankMap = new Dictionary<int, string>();
            Dictionary<int, string> _clientMap = new Dictionary<int, string>();
            string[,] _serverState;
            string[,] _serverSuspect;
            int _numberSlots = 1;
            string _timeOfFirstSlot = "";
            int _slotDuration = 0;
            string[] lines = File.ReadAllLines(arg);
            string[] words;
            int global = 1;
            string pal1 = "";
            string pal2 = "";
            _serverState = new string[100, 100];
            _serverSuspect = new string[100, 100];
            foreach (string line in lines)
            {

                words = line.Split(' ');
                if (words[0] == "P")
                {
                    var expression = new Regex(@"http://(?<hostname>[^\n]+)");
                    if (words[2] == "boney")
                    {

                        var match = expression.Match(words[3]);
                        _boneyMap.Add(int.Parse(words[1]), match.Groups["hostname"].Value);


                    }

                    if (words[2] == "bank")
                    {
                        var match = expression.Match(words[3]);
                        _bankMap.Add(int.Parse(words[1]), match.Groups["hostname"].Value);
                    }
                    if (words[2] == "client")
                    {
                        try
                        {
                            _clientMap.Add(int.Parse(words[1]), words[3]);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("Please provide clients in the configuration file in the following format: \"P client configFileName\"");
                            throw;
                        }

                    }
                }
                else if (words[0] == "S")
                {
                    _numberSlots = int.Parse(words[1]);

                }
                else if (words[0] == "T")
                {
                    _timeOfFirstSlot = words[1];
                }
                else if (words[0] == "D")
                {
                    _slotDuration = int.Parse(words[1]);
                }
                else if (words[0] == "F")
                {
                    int count = 1;
                    int aux = 1;
                    int end = 1;
                    int bit = 1;
                    string s = line;
                    string[] data = s.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] data_final = new string[data.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != null) data_final[i] = data[i];
                    }
                    foreach (string word in data_final)
                    {
                        string[] final = word.Split(", ");
                        foreach (string palavra in final)
                        {
                            if (count == 1)
                            {
                                count = 0;
                                continue;
                            }
                            if (palavra != null || palavra != "" || palavra != "\n")
                            {
                                if (bit == 1) pal1 = palavra;
                                if (bit == 2) pal2 = palavra;
                                if (bit == 1) bit = 2;
                                else bit = 1;
                            }

                        }
                        if (aux % 2 == 0)
                        {
                            _serverState[global, end] = pal1;
                            _serverSuspect[global, end] = pal2;

                            end++;
                        }
                        count = 1;
                        aux++;

                    }
                    global++;
                }

            }
            ServerConfiguration config = new ServerConfiguration();
            config
                .SetBoneyServersHostnames(_boneyMap)
                .SetBankServersHostnames(_bankMap)
                .SetServerStatePerSlot(_serverState)
                .SetServerSuspectedPerSlot(_serverSuspect)
                .SetClients(_clientMap)
                .SetTimeOfFirstSlot(_timeOfFirstSlot)
                .SetNumberOfSlots(_numberSlots)
                .SetSlotDuration(_slotDuration);


            return config;

        }

        public ServerConfiguration SetBoneyServersHostnames(Dictionary<int, string> boneyServersHostnames)
        {
            _boneyServersHostnames = boneyServersHostnames;
            return this;
        }

        public ServerConfiguration SetBankServersHostnames(Dictionary<int, string> bankServersHostnames)
        {
            _bankServersHostnames = bankServersHostnames;
            return this;
        }

        public ServerConfiguration SetServerStatePerSlot(string[,] serverStatePerSlot)
        {
            _serverStatePerSlot = serverStatePerSlot;
            return this;
        }

        public ServerConfiguration SetServerSuspectedPerSlot(string[,] serverSuspectedPerSlot)
        {
            _serverSuspectedPerSlot = serverSuspectedPerSlot;
            return this;
        }

        public ServerConfiguration SetClients(Dictionary<int, string> clients)
        {
            _clients = clients;
            return this;
        }

        public ServerConfiguration SetTimeOfFirstSlot(string timeOfFirstSlot)
        {
            _timeOfFirstSlot = timeOfFirstSlot;
            return this;
        }

        public ServerConfiguration SetNumberOfSlots(int numberOfSlots)
        {
            _numberOfSlots = numberOfSlots;
            return this;
        }

        public ServerConfiguration SetSlotDuration(int slotDuration)
        {
            _slotDuration = slotDuration;
            return this;
        }


        public int GetNumberOfSlots()
        {
            return _numberOfSlots;
        }

        public int GetSlotDuration()
        {
            return _slotDuration;
        }

        public string GetSlotFisrtTime()
        {
            return _timeOfFirstSlot;
        }


        public Dictionary<int, string> GetClients()
        {
            return _clients;
        }

        public (string, int) GetBoneyHostnameAndPortByProcess(int p)
        {
            var expression = new Regex(@"(?<hostname>[^:]+)\:(?<portnumber>[0-9]+)");
            var match = expression.Match(_boneyServersHostnames.GetValueOrDefault(p));
            string hostname = match.Groups["hostname"].Value;
            int port = int.Parse(match.Groups["portnumber"].Value);
            return (hostname, port);
        }
        public (string, int) GetBankHostnameAndPortByProcess(int p)
        {

            var expression = new Regex(@"(?<hostname>[^:]+):(?<portnumber>[0-9]+)");

            var match = expression.Match(_bankServersHostnames.GetValueOrDefault(p));

            string hostname = match.Groups["hostname"].Value;

            int port = int.Parse(match.Groups["portnumber"].Value);
            return (hostname, port);
        }

        public string GetClientScriptNameById(int id)
        {
            return _clients.GetValueOrDefault(id);
        }

        public string GetServerStateInSlot(uint serverID, uint slotNumber)
        {
            return _serverStatePerSlot[slotNumber, serverID];
        }

        public Dictionary<int, string> GetDic()
        {
            return _bankServersHostnames;
        }



        public string GetServerSuspectedInSlot(uint serverID, uint slotNumber)
        {
            return _serverSuspectedPerSlot[slotNumber, serverID];
        }

        public string GetFrozenStateOfProcessInSlot(uint processId, uint slotNumber)
        {
            return _serverStatePerSlot[slotNumber, processId];
        }
        public int GetNumberOfBoneyServers()
        {
            return _boneyServersHostnames.Count();
        }

        public int GetNumberOfBankServers()
        {
            return _bankServersHostnames.Count();
        }

        public int GetNumberOfClients()
        {
            return _clients.Count();
        }

        public bool CheckClientExists(int id)
        {
            return _clients.ContainsKey(id);
        }

        public List<int> GetBoneyServerIDs()
        {
            return _boneyServersHostnames.Keys.ToList();
        }

        public List<int> GetBankServerIDs()
        {
            return _bankServersHostnames.Keys.ToList();
        }

        public List<int> GetClientIDs()
        {
            return _clients.Keys.ToList();
        }

        public List<string> GetBoneyServersPortsAndAddresses()
        {
            return _boneyServersHostnames.Values.ToList();
        }

        public List<string> GetBankServersPortsAndAddresses()
        {
            return _bankServersHostnames.Values.ToList();
        }

        public bool ExceededMaxSlots(uint slot)
        {
            return slot > _numberOfSlots;
        }

        public bool hasFinished()
        {
            return _started;
        }
        public void setAsConfigured()
        {
            _started = true;
        }
    }
}

