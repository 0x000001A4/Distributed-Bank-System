using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    /// <summary>
    /// Stores the value of a process to a slot
    /// </summary>
    internal class BoneySlotManager
    {
        private Slots<uint?> _processSlots;

        public BoneySlotManager(uint maxNumOfSlots)
        {
            _processSlots = new Slots<uint?>(maxNumOfSlots);
        }

        public uint FillOrGetSlot(uint slotNum, uint slotVal)
        {
            uint value;
            lock(this)
            {
                if (_processSlots[slotNum] == null)
                {
                    _processSlots[slotNum] = value = slotVal;
                }
                else
                {
                    value = (uint)_processSlots[slotNum];
                }
            }
            return value;
        }
    }

}
