using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class BoneySlotManager
    {
        private Slots<Slot> _processSlots;





        public BoneySlotManager(uint maxNumOfSlots)
        {
            _processSlots = new Slots<Slot>(maxNumOfSlots);
        }

        public Slot FillSlot(uint slotNum, Slot slotVal)
        {
            _processSlots[slotNum] = slotVal;
            return slotVal;
        }

        public Slot GetSlotValue(uint slotNum)
        {
            return _processSlots[slotNum];
        }
    }
}
