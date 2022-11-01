namespace BankServer.utils
{
    internal class Slots<T>
    {
        private T[] _slots;
        private uint _maxNumOfSlots;

        public uint size { get => _maxNumOfSlots; }

        public Slots(uint maxNumOfSlots)
        {
            _slots = new T[maxNumOfSlots];
            _maxNumOfSlots = maxNumOfSlots;
        }

        public T this[int i]
        {
            get => _slots[i];
            set
            {
                if (i < _maxNumOfSlots) _slots[i] = value;
                else throw new ArgumentOutOfRangeException();
            }

        }
    }
}
