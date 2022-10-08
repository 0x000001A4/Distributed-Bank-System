using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class BoneySlotManager
    {
        private Slots<uint> _processSlots;
        private Paxos _paxos;






        public BoneySlotManager(uint maxNumOfSlots)
        {
            _processSlots = new Slots<uint>(maxNumOfSlots);
            _paxos = new Paxos();
        }

        public uint FillSlot(uint slotNum, uint slotVal)
        {
            _processSlots[slotNum] = slotVal;
            return slotVal;
        }

        public uint GetSlotValue(uint slotNum, uint slotVal)
        {
            return _processSlots[slotNum];
        }
    }
}
