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
        private Slots<string> _frozenSlots;
        private Slots<string>[] _suspectedProcessesSlots;

        private uint _processId;
        private uint _numberOfProcesses;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processId">Process ID</param>
        /// <param name="numberOfProcesses">Number of Boney Servers</param>
        /// <param name="numberOfSlots">Number of slots</param>
        /// <param name="frozenSlots">string[slot, process] -> indicates if process if frozen in slot</param>
        /// <param name="suspectedProcessesSlots">string[slot, suspected] -> indicates if process is suspected in slot</param>
        public BoneyServerState(uint processId, uint numberOfProcesses, uint numberOfSlots,
            string[,] frozenSlots, string[,] suspectedProcessesSlots)
        {
            _slotManager = new BoneySlotManager(numberOfSlots);
            _frozenSlots = new Slots<string>(numberOfSlots);
            _suspectedProcessesSlots = new Slots<string>[numberOfProcesses];
            _processId = processId;
            _numberOfProcesses = numberOfProcesses;

            for (uint slot = 0 ; slot < numberOfSlots; slot++ )
            {
                _frozenSlots[slot] = frozenSlots[slot, processId];

                for (uint process = 0; process < numberOfProcesses; process++)
                {
                    _suspectedProcessesSlots[process][slot] = suspectedProcessesSlots[slot, process];
                }

            }
        }

    }
}
