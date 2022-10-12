using BoneyServer.domain;
using BoneyServer.utils;
using BoneyServer.services;

namespace BoneyServer
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BoneyServerState : IUpdateState{
        private uint _slot;
        private BoneySlotManager _slotManager;
        private uint _processId;
        private uint _numberOfProcesses;
        private IMultiPaxos _paxos;
        private ServerConfiguration _config;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();

        public BoneyServerState(uint processId, IMultiPaxos paxos, ServerConfiguration config)
        {
            uint numberOfSlots = (uint)config.GetNumberOfSlots();
            _numberOfProcesses = (uint)config.GetNumberOfBoneyServers();

            _slotManager = new BoneySlotManager(numberOfSlots);
            _slot = 1;
            _paxos = paxos;
            _config = config;
            _processId = processId;
            _frozen = config.GetFrozenStateOfProcessInSlot(_processId, _slot);
        }

        public void handleQueuedMessage(CompareAndSwapServiceImpl service, Message _msg) {

            if (_msg.getRequestId() == 1) {
                service.doCompareAndSwap(_msg.getCompareAndSwapRequest());
            }


        }

        public void enqueue(Message _msg)
        {
            _queue.Enqueue(_msg);
        }


        public void incrementSlot()
        {
            _slot += 1;
        }



        public void update()
        {
            Dictionary<uint, string> servers = new Dictionary<uint, string>();
            List<int> boneysID = _config.GetBoneyServerIDs();
            foreach (int id in boneysID)
            {
                servers.Add((uint)id, _config.GetServerSuspectedInSlot((uint)id, (uint)_slot));
            }
            _paxos.UpdateServers(servers);
            incrementSlot();
            _frozen = _config.GetFrozenStateOfProcessInSlot(_processId, _slot);
        }

        public bool isFrozen()
        {
            return _frozen == FrozenState.FROZEN;
        }





    }
}