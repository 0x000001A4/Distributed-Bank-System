using BoneyServer.utils;

namespace BoneyServer.domain
{
    /// <summary>
    /// Stores the value of a process to a slot
    /// </summary>
    public class BoneySlotManager
    {
        private Slots<uint> _processSlots;
        private uint _maxSlots;

        public BoneySlotManager(uint maxNumOfSlots)
        {
            _processSlots = new Slots<uint>(maxNumOfSlots);
            _maxSlots = maxNumOfSlots;
        }

        public uint FillSlot(int slotNum, uint slotVal)
        {
            uint value;
            lock (this)
            {
                if (_processSlots[slotNum] == 0)
                {
                    value = _processSlots[slotNum] = slotVal;
                }
                else
                {
                    value = _processSlots[slotNum];
                }
            }
            return value;
        }

        public uint GetSlotValue(int slotNum)
        {
            return _processSlots[slotNum];
        }

        public uint GetMaxSlots()
        {
            return _maxSlots;
        }
    }
}