using BoneyServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneyServer.utils;
using BoneyServer.services;

namespace BoneyServer
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BoneyServerState
    {

        private BoneySlotManager _slotManager;
        private Slots<string> _frozenSlots;
        private Slots<string>[] _suspectedProcessesSlots;

        private uint _processId;
        private uint _numberOfProcesses;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processId">Process ID</param>
        /// <param name="numberOfProcesses">Number of Boney Servers</param>
        /// <param name="numberOfSlots">Number of slots</param>
        /// <param name="frozenSlots">string[slot, process] -> indicates if process if frozen in slot</param>
        /// <param name="suspectedProcessesSlots">string[slot, suspected] -> indicates if process is suspected in slot</param>
        public BoneyServerState(uint processId, ServerConfiguration config)
        {
            uint numberOfSlots = (uint)config.GetNumberOfSlots();
            _numberOfProcesses = (uint)config.GetNumberOfBoneyServers();

            _slotManager = new BoneySlotManager(numberOfSlots);
            _frozenSlots = new Slots<string>(numberOfSlots);
            _suspectedProcessesSlots = new Slots<string>[_numberOfProcesses];
            _processId = processId;

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

        public bool isFrozen()
        {
            return _frozen == FrozenState.FROZEN;
        }
    }
}