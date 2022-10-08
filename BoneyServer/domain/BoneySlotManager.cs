using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class BoneySlotManager
    {
        private Slots<uint> _slots;

        public BoneySlotManager(uint maxNumOfSlots)
        {
            _slots = new Slots<uint>(maxNumOfSlots);
        }

        public uint FillSlot(uint slotNum, uint slotVal)
        {
            _slots[slotNum] = slotVal;
            return slotVal;
        }

        public uint GetSlotValue(uint slotNum, uint slotVal)
        {
            return _slots[slotNum];
        }
    }
}
