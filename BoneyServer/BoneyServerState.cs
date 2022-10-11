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
    internal class BoneyServerState // ITimerUpdateable
    {
        private Slots<string> _frozenSlots;
        private Dictionary<uint, string>[] _suspectedProcessesSlots;

        private uint _processId;
        private uint _numberOfBoneyProcesses;
        private int _currentSlot;

        public BoneyServerState(uint processId, ServerConfiguration config)
        {
            
            //_processId = processId;
            //_numberOfBoneyProcesses = (uint)config.GetNumberOfBoneyServers();
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

        public bool IsFrozen()
        {
            return _frozenSlots[_currentSlot].Equals(FrozenState.FROZEN);
        }

		public bool isFrozen() {
			return _frozen == FrozenState.FROZEN;
		}
	}
}
