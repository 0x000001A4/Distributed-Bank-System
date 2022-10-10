using BoneyServer.utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    public interface IMultiPaxos
    {
        void Start(object value);
        void UpdateServers(Dictionary<uint, string> servers);
    }
    /// <summary>
    /// Solves Consensus issue in the context of BoneyServers (optimized to work with slots).
    /// Each process has an instance (object)  of Paxos
    /// </summary>
    internal class Paxos : IMultiPaxos
    {
        private readonly ILogger<Paxos> _logger;

        // TODO - as soon as paxos achieves consensus, it returns a list of all processes in the queue so that they all receive the same answer.
        private List<PaxosInstance> _paxosInstances;      // stores <value, write_time_stamp, read_time_stamp> for each instance (each list index is a diferent instance)
        private Slots<PaxosSlotState> _paxosSlotState;    // stores the state of decision about a slot
        Dictionary<uint, KeyValuePair<string, string>> _paxosServers; // (processID) -> [hotname, suspectedstate]
        private uint _sourceProcessID;                    // The process ID executing Paxos
        private uint _sourceLeaderNumber;
        private uint? _leaderProcessID;                   // The process it suspects to be the leader

        public static uint Instance { get; set; }

        public Paxos(uint sourceProcessID, uint numOfSlots, ILogger<Paxos> logger) {
            Instance = 0;
            _sourceLeaderNumber = sourceProcessID;
            _sourceProcessID = sourceProcessID;
            _leaderProcessID = null;
            _paxosInstances = new List<PaxosInstance>();
            _paxosSlotState = new Slots<PaxosSlotState>(numOfSlots);
            _logger = logger;
        }

        public void Start(object value) {
            if (iAmLeader())
            {
                Thread proposer = new Thread(new ThreadStart(() => proposerWork((CompareAndSwapRequest)value, _sourceLeaderNumber, Instance)));
                proposer.Start();
                Instance++;
            }
        }

        public void UpdateServers(Dictionary<uint, string> servers) {
            if (servers.Count == 0) throw new Exception("Paxos must be composed of at least 1 server!");
            _paxosServers = servers;
            updateLeader();
        }

        private void updateLeader()
        {
            uint minProcID = 0;
            foreach (var server in _paxosServers)
            {
                string serverSuspectedState = server.Value;
                if (serverSuspectedState.Equals(SuspectState.NOTSUSPECTED))
                {
                    minProcID = server.Key;
                }
            }

            if (_leaderProcessID == minProcID)
            {
                _logger.LogInformation("PAXOS: Leader hasn't changed.");
            }
            // if was not already leader and is elected as leader
            else if ( (_leaderProcessID != _sourceProcessID) && (minProcID == _sourceProcessID))
            {
                _sourceLeaderNumber += (uint)_paxosServers.Count();
                _logger.LogInformation($"PAXOS: I was elected leader, updating LeaderNumber to {_sourceLeaderNumber}.");
            }
            // if was not already leader and another process is elected leader
            else if ((_leaderProcessID != _sourceProcessID) && (minProcID != _sourceProcessID))
            {
                _logger.LogInformation($"PAXOS: Process {minProcID} was elected Leader.");
            }

            _leaderProcessID = minProcID;
        }

        private bool iAmLeader()
        {
            return _leaderProcessID == _sourceProcessID;
        }

        private void proposerWork(CompareAndSwapRequest value, uint leaderNumber, uint instance)
        {

            PaxosAcceptorService.PaxosAcceptorServiceClient acceptorClient = new PaxosAcceptorService.PaxosAcceptorServiceClient(channel);
        }
    }


    /// <summary>
    /// Represents an Instance of paxos <value, write_ts , read_ts>
    /// </summary>
    internal class PaxosInstance
    {
        private CompareAndSwapRequest? _value;
        private uint _writeTimeStamp;
        private uint _readTimeStamp;

        public CompareAndSwapRequest? Value 
        {
            get { return _value; }
            set
            {
                lock(this)
                {
                    _value = value;
                }
            }
        }

        public uint WriteTimeStamp
        {
            get { return _writeTimeStamp; }
            set 
            { 
                lock(this)
                {
                    _writeTimeStamp = value;
                }
                
            }
        }

        public uint ReadTimeStamp
        {
            get { return _readTimeStamp;  }
            set
            {
                lock(this)
                {
                    _readTimeStamp = value;
                }
            }
        }

        public PaxosInstance()
        {
            _value = null;
            _writeTimeStamp = 0;
            _readTimeStamp = 0;
        }

    }

    internal class PaxosSlotState
    {
        enum ConsensusState
        {
            NOT_STARTED,
            WAITING,
            FINISHED
        }

        private Queue<uint> _waitingList;
        private ConsensusState _state;


        public PaxosSlotState()
        {
            _state = ConsensusState.NOT_STARTED;
            _waitingList = new Queue<uint>();
        }

        public void StartConsensus()
        {
            _state = ConsensusState.WAITING;
        }

        public void EndConsensus()
        {
            _state = ConsensusState.FINISHED;
        }

        public bool IsFinished()
        {
            return _state == ConsensusState.FINISHED;
        }

        public bool IsWaiting()
        {
            return _state == ConsensusState.WAITING;
        }
    }



}
