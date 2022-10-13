using BoneyServer.utils;
using BoneyServer.services;
using BoneyServer.domain.paxos;

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

        public void HandleQueuedMessage(CompareAndSwapServiceImpl service, Message _msg)
        {

            if (_msg.getRequestId() == 1)
            {
                service.doCompareAndSwap(_msg.getCompareAndSwapRequest());
            }


        }

        public void Enqueue(Message _msg)
        {
            _queue.Enqueue(_msg);
        }


        public void IncrementSlot()
        {
            _slot += 1;
        }



        public void Update()
        {
            Dictionary<uint, string> servers = new Dictionary<uint, string>();
            List<int> boneysID = _config.GetBoneyServerIDs();
            foreach (int id in boneysID)
            {
                servers.Add((uint)id, _config.GetServerSuspectedInSlot((uint)id, _slot));
            }
            _paxos.UpdateServers(servers);
            IncrementSlot();
            _frozen = _config.GetFrozenStateOfProcessInSlot(_processId, _slot);
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
    }
}