using BoneyServer.utils;

namespace BoneyServer.domain
{
    /// <summary>
    /// Stores the value of a process to a slot
    /// </summary>
    internal class BoneySlotManager
    {
        private Slots<uint> _processSlots;
        private uint currentSlot;

        public BoneySlotManager(uint maxNumOfSlots)
        {
            _processSlots = new Slots<uint>(maxNumOfSlots);
            currentSlot = 0;
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

        public void IncrementCurrentSlot() { currentSlot++; }

        public uint GetCurrentSlot() { return currentSlot; }
    }
}