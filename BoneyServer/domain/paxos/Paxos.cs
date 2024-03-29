﻿using BoneyServer.utils;
using static BoneyServer.domain.paxos.Paxos;

namespace BoneyServer.domain.paxos
{
    public interface IMultiPaxos
    {
        public void Start(PaxosValue value, string address, uint primary);
        void UpdateServers(Dictionary<uint, string> servers);
        PaxosInstance GetPaxosInstance(uint instanceId);
        public uint GetPrimaryForSlot(uint slot);

        public bool UpdateAccept(PaxosValue value, uint leaderNumber, uint instance);

        (PaxosValue, uint, bool) Promisse(uint leaderNumber, uint instance);


        public PaxosSlotState GetSlotState(int slot);
    }
    /// <summary>
    /// Solves Consensus issue in the context of BoneyServers (optimized to work with slots).
    /// Each process has an instance (object)  of Paxos
    /// </summary>
    internal class Paxos : IMultiPaxos
    {
        // TODO - as soon as paxos achieves consensus, it returns a list of all processes in the queue so that they all receive the same answer.

        private List<PaxosInstance> _paxosInstances;      // stores <value, write_time_stamp, read_time_stamp> for each instance (each list index is a diferent instance)
        private Slots<PaxosSlotState> _paxosSlotState;    // stores the state of decision about a slot
        Dictionary<uint, string> _paxosServers; // (processID) -> [hotname, suspectedstate]
        private int _numberOfBoneyServers;
        private uint _sourceProcessID;                    // The process ID executing Paxos
        private uint _sourceLeaderNumber;
        private uint? _leaderProcessID;                   // The process it suspects to be the leader
        private List<string> _boneyAdress;
        int counter = 0;

        private uint Instance { get; set; } = 0;

        public Paxos(uint sourceProcessID, uint numOfSlots, List<string> boneysAdress)
        {
            _numberOfBoneyServers = boneysAdress.Count();
            _sourceLeaderNumber = sourceProcessID;
            _sourceProcessID = sourceProcessID;
            _leaderProcessID = null;

            _paxosInstances = new List<PaxosInstance>();
            // create idex 0 not to be used so that instance 1 is at index 1
            _paxosInstances.Add(new PaxosInstance());    

            // Initialize paxos slots state
            _paxosSlotState = new Slots<PaxosSlotState>(numOfSlots+1);
            
            for (int i = 0; i < numOfSlots+1; i++)
            {
                _paxosSlotState[i] = new PaxosSlotState();
            }

            _boneyAdress = boneysAdress;
            Proposer.SetServers(boneysAdress);
            Acceptor.SetServers(boneysAdress);
        }

        public void Start(PaxosValue value, string address, uint primary)
        {
            lock (this)
            {
                uint slot = value.Slot;
                PaxosSlotState slotState = _paxosSlotState[(int)slot];
                if (slotState.NotStarted() && iAmLeader())
                {
                    slotState.StartConsensus();
                    Thread proposer = new Thread(new ThreadStart(() => Proposer.ProposerWork(value, _sourceLeaderNumber, Instance, _paxosInstances)));
                    _paxosInstances.Add(new PaxosInstance() { Value = value });
                    Instance++;
                    proposer.Start();
                    Logger.LogDebug($"Starting proposer");

                    // If an instance for slot has already begun, enqueue the request not to start a new instance

                }
                else if (slotState.IsWaiting())
                {
                    Logger.LogDebug($"Slot {slot} is Waiting");
                    _paxosSlotState[(int)slot].Enqueue(value.ProcessID);
                }

                else if (slotState.IsFinished())
                {
                    Logger.LogDebug($"Slot {slot} is Finished");
                    ConsensusFinalValue.SendPaxosResult(address, slot, primary);
                    Logger.LogDebug($"Response (primary: {primary}, slot: {slot}) sent to {address}. (No Paxos instance started)");
                }
            }


        }
        public (PaxosValue?, uint, bool) Promisse(uint leaderNumber, uint instance)

        {
            try {
                Logger.LogDebug("Instance " + instance);
                createInstancesUpTo(instance);
                PaxosInstance instancia = _paxosInstances[(int)instance];
                PaxosValue? value = instancia.Value;
                uint writeTimeStamp = instancia.WriteTimeStamp;
                uint readTimeStamp = instancia.ReadTimeStamp;

                bool needReadUpdate = Acceptor.PromisseWork(leaderNumber, readTimeStamp);
                if (needReadUpdate) _paxosInstances[(int)instance].ReadTimeStamp = leaderNumber;

                return (value, writeTimeStamp, needReadUpdate);

            } catch(Exception e) {
                Logger.LogError(e.Message);
                throw;
            }
        }

        public bool UpdateAccept(PaxosValue value, uint leaderNumber, uint instance)
        {
            PaxosInstance instancia = _paxosInstances[(int)instance];
            if (leaderNumber >= instancia.ReadTimeStamp)
            {
                instancia.WriteTimeStamp = leaderNumber;
                instancia.Value = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateServers(Dictionary<uint, string> servers)
        {
            counter++;
            if (servers.Count == 0) throw new Exception("Paxos must be composed of at least 1 server!");
            
            _paxosServers = servers;
            updateLeader();
            Logger.LogEvent(counter.ToString());
        }

        public PaxosInstance GetPaxosInstance(uint instanceId)
        {
            createInstancesUpTo(instanceId);
            return _paxosInstances[(int)instanceId];
        }

        public uint GetPrimaryForSlot(uint slot)
        {
            if (slot > _paxosInstances.Capacity) 
                throw new Exception("Trying to get primary for a slot that is bigger than the max number of slots.");

            foreach (PaxosInstance paxosInstance in _paxosInstances) { 
                if (paxosInstance.Value != null && paxosInstance.Value.Slot == slot) {
                    return paxosInstance.Value.ProcessID;
                }
            }
            throw new Exception("Unexpected behaviour: There was no paxos instance corresponding to the slot.");
        }

       public PaxosSlotState GetSlotState(int slot)
        {
            return _paxosSlotState[slot];
        }

        private void updateLeader()
        {
            uint minProcID = int.MaxValue;
            Console.WriteLine(_paxosServers.Count);
            foreach (var server in _paxosServers)
            {
                string serverSuspectedState = server.Value;
                if (serverSuspectedState.Equals(SuspectState.NOTSUSPECTED))
                {
                    if (server.Key < minProcID) minProcID = server.Key;
                }
            }

            if (minProcID != int.MaxValue)
            {
                if (_leaderProcessID == minProcID)
                {
                    Logger.LogDebug("PAXOS: Leader hasn't changed.");
                }
                // if was not already leader and is elected as leader
                else if (_leaderProcessID != _sourceProcessID && minProcID == _sourceProcessID)
                {
                    _sourceLeaderNumber += (uint)_paxosServers.Count();
                    Logger.LogDebug($"PAXOS: I was elected leader, updating LeaderNumber to {_sourceLeaderNumber}.");
                }
                // another process is elected leader
                else if (minProcID != _sourceProcessID)
                {
                    Logger.LogDebug($"PAXOS: Process {minProcID} was elected Leader.");
                }
                _leaderProcessID = minProcID;
            }
            else _leaderProcessID = null;
        }

        private bool iAmLeader() {
            return _leaderProcessID == _sourceProcessID;
        }

        private void createInstancesUpTo(uint instance)
        {
            lock(this)
            {
                int currentSize = _paxosInstances.Count();
                int instancesToAdd = (int)instance - currentSize + 1;
                if (instancesToAdd > 0)
                {
                    Instance += (uint)instancesToAdd;
                    for (int i = 0; i < instancesToAdd; i++)
                    {
                        _paxosInstances.Add(new PaxosInstance());
                    }
                }
            }
        }
    }


    /// <summary>
    /// Represents an Instance of paxos <value, write_ts , read_ts> and an auxiliary variable _acceptedCommands
    /// </summary>
    public class PaxosInstance
    {
        private object _lock = new object();
        private PaxosValue? _value;
        private uint _writeTimeStamp;
        private uint _readTimeStamp;
        private uint _acceptedCommands;

        public PaxosValue? Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public object GetLock() { return _lock; }

        public uint WriteTimeStamp
        {
            get { return _writeTimeStamp; }
            set { _writeTimeStamp = value; }
        }

        public uint ReadTimeStamp
        {
            get { return _readTimeStamp; }
            set { _readTimeStamp = value; }
        }

        public uint AcceptedCommands
        {
            get { return _acceptedCommands; }
            set { _acceptedCommands = value; }
        }

        public PaxosInstance()
        {
            _value = null;
            _writeTimeStamp = 0;
            _readTimeStamp = 0;
            _acceptedCommands = 0;
        }
    }



    public class PaxosSlotState
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
        public bool NotStarted()
        {
            return _state == ConsensusState.NOT_STARTED;
        }

        public void Enqueue(uint processID)
        {
            _waitingList.Enqueue(processID);
        }
    }

    public class PaxosValue
    {
        public uint ProcessID;
        public uint Slot;
        public PaxosValue(uint processID, uint slot)
        {
            ProcessID = processID;
            Slot = slot;
        }
    }

}
