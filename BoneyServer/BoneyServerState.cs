using BoneyServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneyServer.utils;

namespace BoneyServer
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    internal class BoneyServerState
    {
        private BoneySlotManager _slotManager;
        private IMultiPaxos _multiPaxos;
        private Slots<string> _frozenSlots;
        private Slots<string>[] _suspectedProcessesSlots;

        private uint _processId;
        private uint _numberOfBoneyProcesses;
        private uint _currentSlot;

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
            _slotManager = new BoneySlotManager(numberOfSlots);
            
            _processId = processId;
            _numberOfBoneyProcesses = (uint)config.GetNumberOfBoneyServers();
            //_multiPaxos = new Paxos(processId, )

            //for (uint slot = 0 ; slot < numberOfSlots; slot++ )
            //{
            //    _frozenSlots[slot] = frozenSlots[slot, processId];

            //    for (uint process = 0; process < numberOfProcesses; process++)
            //    {
            //        _suspectedProcessesSlots[process][slot] = suspectedProcessesSlots[slot, process];
            //    }

            //}
        }

    }
}
