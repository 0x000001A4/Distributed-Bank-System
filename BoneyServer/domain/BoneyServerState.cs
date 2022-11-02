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
        private BoneySlotManager _slotManager;
        private uint _processId;
        private uint _numberOfProcesses;
        private IMultiPaxos _paxos;
        private ServerConfiguration _config;
        private Server _server;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();
        private QueuedCommandHandler _cmdHandler;

        public BoneyServerState(uint processId, IMultiPaxos paxos, ServerConfiguration config, QueuedCommandHandler cmdHandler)
        {
            uint numberOfSlots = (uint)config.GetNumberOfSlots();
            _numberOfProcesses = (uint)config.GetNumberOfBoneyServers();

            _slotManager = new BoneySlotManager(numberOfSlots);
            _paxos = paxos;
            _config = config;
            _processId = processId;
            _frozen = config.GetFrozenStateOfProcessInSlot(_processId, 1);
            _cmdHandler = cmdHandler;
        }

        public void Enqueue(Message _msg)
        {
            _queue.Enqueue(_msg);
        }





        public void HandleQueuedMessage(Message _msg)
        {
            string _sender = _msg.GetSender();
            uint msgId = _msg.GetRequestId();

            if (msgId == Message.COMPARE_AND_SWAP) {
                _cmdHandler.handleCompareAndSwap(_msg.GetCompareAndSwapRequest(), _sender);
            }

            else if (msgId == Message.PREPARE) {
                _cmdHandler.handlePrepare(_msg.GetPrepareRequest(), _sender);
            }
        
            else if (msgId == Message.ACCEPT) {
                _cmdHandler.handleAccept(_msg.GetAcceptRequest(), _sender);
            }
            
            else if (msgId == Message.LEARN_COMMAND) {
                _cmdHandler.handleLearnCommand(_msg.GetLearnCommandRequest(), _sender);
            }
        }

        public void Update(uint tick) {
            // Save previous slot boney server state Status (frozen/not frozen) and incrementSlot
            var _prevSlotStatus = _frozen;
            // Update servers' suspicions for new slot.
            Dictionary<uint, string> servers = new Dictionary<uint, string>();
            List<int> boneysID = _config.GetBoneyServerIDs();

            foreach (int id in boneysID) {
                servers.Add((uint)id, _config.GetServerSuspectedInSlot((uint)id, tick));
            }
            _paxos.UpdateServers(servers);

            // Update servers' own frozen state for new slot.
            _frozen = _config.GetFrozenStateOfProcessInSlot(_processId, tick);

            // Set Configuration as complete (Needed to avoid crashing boney servers while they are configurating)
            _config.setAsConfigured();

            // Check if boney server just unfroze!

            if (tick > 1 && _prevSlotStatus == FrozenState.FROZEN && _frozen == FrozenState.UNFROZEN) {
                HandleQueuedMessages();
            }
            
            Logger.LogInfo("After verification: ");
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

        public void Stop()
        {
            HandleQueuedMessages();
            Logger.LogInfo("Boney Server State: Max number of slots reached. Shutting process down after processing queued requests.");
            _server.ShutdownAsync().Wait();
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

        public void AddServer(Server server)
        {
            _server = server;
        }
    }
}