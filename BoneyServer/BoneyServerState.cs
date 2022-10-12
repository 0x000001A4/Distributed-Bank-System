using BoneyServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneyServer.utils;
using BoneyServer.domain;
using BoneyServer.services;

namespace BoneyServer
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BoneyServerState : IUpdateState{
        private int _slot;
        private Slots<string> _frozenSlots;
        private Slots<string>[] _suspectedProcessesSlots;

        private uint _processId;
        private uint _numberOfBoneyProcesses;
        private int _currentSlot;
        private IMultiPaxos _paxos;
        private ServerConfiguration _config;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();

        public BoneyServerState(IMultiPaxos paxos /*uint processId*/, ServerConfiguration config
                                )
        {
            _slot = 1;
            _paxos = paxos;
            _config = config;
            
            //_processId = processId;
            //_numberOfBoneyProcesses = (uint)config.GetNumberOfBoneyServers();
            //_multiPaxos = new Paxos(processId, )

            for (int slot = 0; slot < numberOfSlots; slot++)
            {
                _frozenSlots[slot] = config.GetServerStateInSlot(_processId, (uint)slot);
                for (uint process = 0; process < _numberOfProcesses; process++)
                {
                    _suspectedProcessesSlots[process][slot] = config.GetServerSuspectedInSlot(_processId,(uint)slot);
                }
            }
            _frozen = _frozenSlots[(int)_slotManager.GetCurrentSlot()];
        }

        public void updateState()
        {
            int slotId = (int)_slotManager.GetCurrentSlot();
            _frozen = _frozenSlots[slotId];
            // Check if prozen unfroze in this slot
            if (_frozenSlots[slotId] == FrozenState.UNFROZEN && _frozenSlots[slotId - 1] == FrozenState.FROZEN)
            {
                // Handle Queued requests
            }
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
        }

       



    }
}