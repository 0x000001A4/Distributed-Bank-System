using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BankServer.utils
{

    /// <summary>
    /// Stores any information needed for any type of server
    /// </summary>
    public class ServerConfiguration
    {
        private Dictionary<int, string> _boneyServersHostnames;
        private Dictionary<int, string> _bankServersHostnames;
        private Dictionary<int, string> _clients;
        List<Dictionary<int, string>> _frozenStatePerSlot;
        List<Dictionary<int, string>> _sspectStatePerSlot;
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
            List<Dictionary<int, string>> _serverState = new List<Dictionary<int, string>>();
            List<Dictionary<int, string>> _serverSuspect = new List<Dictionary<int, string>>();
            _serverState.Add(new Dictionary<int, string>());    // just to start at index 1;
            _serverSuspect.Add(new Dictionary<int, string>());  // same as above
            int _numberSlots = 1;
            string _timeOfFirstSlot = "";
            int _slotDuration = 0;
            string[] lines = File.ReadAllLines(arg);
            string[] words;
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
                    _numberSlots = int.Parse(words[1]) + 1;
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
                    Dictionary<int, string> tempFrozenState = new Dictionary<int, string>();
                    Dictionary<int, string> tempSuspectState = new Dictionary<int, string>();
                    var pattern = @"([0-9]+), ([NF]), (N?S)";
                    MatchCollection matches = Regex.Matches(line, pattern);
                    foreach (Match match in matches)
                    {
                        int processID = int.Parse(match.Groups[1].Value);
                        string frozenState = match.Groups[2].Value;
                        string suspectState = match.Groups[3].Value;
                        tempFrozenState.Add(processID, frozenState);
                        tempSuspectState.Add(processID, suspectState);
                    }
                    _serverState.Add(tempFrozenState);
                    _serverSuspect.Add(tempSuspectState);
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

            Logger.LogDebug("aqui2" + config.GetFrozenStateOfProcessInSlot(1, 1));
            Logger.LogDebug("aqui" + config.GetFrozenStateOfProcessInSlot(4, 1));
            Logger.LogDebug("aqui2" + config.GetFrozenStateOfProcessInSlot(5, 1));
            Logger.LogDebug("aqui2" + config.GetFrozenStateOfProcessInSlot(1, 2));
            Logger.LogDebug("aqui" + config.GetFrozenStateOfProcessInSlot(4, 2));
            Logger.LogDebug("aqui2" + config.GetFrozenStateOfProcessInSlot(5, 2));
            Logger.LogDebug("aqui2" + config.GetFrozenStateOfProcessInSlot(1, 3));
            Logger.LogDebug("aqui" + config.GetFrozenStateOfProcessInSlot(4, 3));
            Logger.LogDebug("aqui2" + config.GetFrozenStateOfProcessInSlot(5, 3));
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

        public ServerConfiguration SetServerStatePerSlot(List<Dictionary<int, string>> serverStatePerSlot)
        {
            _frozenStatePerSlot = serverStatePerSlot;
            return this;
        }

        public ServerConfiguration SetServerSuspectedPerSlot(List<Dictionary<int, string>> serverSuspectedPerSlot)
        {
            _sspectStatePerSlot = serverSuspectedPerSlot;
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
            return _frozenStatePerSlot[(int)slotNumber].GetValueOrDefault((int)serverID);
        }

        public Dictionary<int, string> GetDic()
        {
            return _bankServersHostnames;
        }



        public string GetServerSuspectedInSlot(uint processId, uint slotNumber)
        {
            return _sspectStatePerSlot[(int)slotNumber].GetValueOrDefault((int)processId);
        }

        public string GetFrozenStateOfProcessInSlot(uint processId, uint slotNumber)
        {
            return _frozenStatePerSlot[(int)slotNumber].GetValueOrDefault((int)processId);
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
            return slot >= _numberOfSlots;
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

