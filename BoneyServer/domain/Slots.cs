using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class Slots<T>
    {
        private T[] _slots;
        private uint _maxNumOfSlots;

        public uint size { get => _maxNumOfSlots; }

        public Slots(uint maxNumOfSlots){
            _slots = new T[maxNumOfSlots];
            _maxNumOfSlots = maxNumOfSlots;
        }

        public T this[uint i]
        {
            get => _slots[i];
            set {
               if (i < _maxNumOfSlots) _slots[i] = value; 
               else throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
