using BoneyServer.utils;
using BoneyServer.services;
using BoneyServer.domain.paxos;
using Grpc.Core;

namespace BoneyServer.domain
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BoneyServerState : IUpdatable
    {
        private uint _slot;
        private BoneySlotManager _slotManager;
        private uint _processId;
        private uint _numberOfProcesses;
        private IMultiPaxos _paxos;
        private ServerConfiguration _config;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();
        private QueuedCommandHandler _cmdHandler;

        public BoneyServerState(uint processId, IMultiPaxos paxos, ServerConfiguration config, QueuedCommandHandler cmdHandler)
        {
            uint numberOfSlots = (uint)config.GetNumberOfSlots();
            _numberOfProcesses = (uint)config.GetNumberOfBoneyServers();

            _slotManager = new BoneySlotManager(numberOfSlots+1);
            _slot = 0;
            _paxos = paxos;
            _config = config;
            _processId = processId;
            _frozen = config.GetFrozenStateOfProcessInSlot(_processId, _slot);
            _cmdHandler = cmdHandler;
        }

        public void Enqueue(Message _msg)
        {
            _queue.Enqueue(_msg);
        }


        public void IncrementSlot()
        {
            _slot += 1;
        }


        public void HandleQueuedMessage(Message _msg)
        {
            string _sender = _msg.GetSender();
            uint msgId = _msg.GetRequestId();

            if (msgId == 1) {
                _cmdHandler.handleCompareAndSwap(_msg.GetCompareAndSwapRequest());
            }

            else if (msgId == 2) {
                _cmdHandler.handlePrepare(_msg.GetPrepareRequest(), _sender);
            }
        
            else if (msgId == 3) {
                _cmdHandler.handleAccept(_msg.GetAcceptRequest(), _sender);
            }
            
            else if (msgId == 4) {
                _cmdHandler.handleLearnCommand(_msg.GetLearnCommandRequest(), _sender);
            }
        }

        public void Update() {
            // Save previous slot boney server state Status (frozen/not frozen) and incrementSlot
            var _prevSlotStatus = _frozen;
            IncrementSlot();

            // Update servers' suspicions for new slot.
            Dictionary<uint, string> servers = new Dictionary<uint, string>();
            List<int> boneysID = _config.GetBoneyServerIDs();
            foreach (int id in boneysID) {
                servers.Add((uint)id, _config.GetServerSuspectedInSlot((uint)id, _slot));
            }
            _paxos.UpdateServers(servers);

            // Update servers' own frozen state for new slot.
            _frozen = _config.GetFrozenStateOfProcessInSlot(_processId, _slot);

            // Set Configuration as complete (Needed to avoid crashing boney servers while they are configurating)
            _config.setAsConfigured();

            // Check if boney server just unfroze!
            if (_slot > 1 && _prevSlotStatus == FrozenState.FROZEN && _frozen == FrozenState.UNFROZEN) {
                HandleQueuedMessages();
            }
        }

        public void HandleQueuedMessages() {
            // If yes handle Queued messages!
            while (_queue.Count > 0)
            {
                Message msg = _queue.Dequeue();
                Logger.LogDebug($"Dequeued: {msg} with MessageId: {msg.GetRequestId()}");
                HandleQueuedMessage(msg);
                Logger.LogDebug($"Exited HandleQueuedMessage with Message Id: {msg.GetRequestId()}");
            }
        }

        public bool IsFrozen()
        {
            return _frozen == FrozenState.FROZEN;
        }


        public uint GetNumberOfBoneyProcesses()
        {
            return _numberOfProcesses;
        }

        public List<string> GetBankServersHostnameAndPort()
        {
            return _config.GetBankServersPortsAndAddresses();
        }


        public BoneySlotManager GetSlotManager()
        {
            return _slotManager;
        }

        public bool isConfigured() {
            return _config.hasFinished();
        }
    }
}