using BoneyServer.utils;

namespace BoneyServer.domain
{
    /// <summary>
    /// Stores the value of a process to a slot
    /// </summary>
    public class BoneySlotManager
    {
        private Slots<uint> _processSlots;
        private uint _currentSlot;
        private readonly uint _maxSlot;

        public BoneySlotManager(uint maxNumOfSlots)
        {
            _processSlots = new Slots<uint>(maxNumOfSlots);
            _maxSlot = maxNumOfSlots;
            _currentSlot = 0;
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

        public void IncrementCurrentSlot() { _currentSlot++; }

        public uint GetCurrentSlot() { return _currentSlot; }

        public uint GetMaxSlot()
        {
            return _maxSlot;
        }
    }
}