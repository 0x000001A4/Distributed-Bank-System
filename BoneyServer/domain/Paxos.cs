using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class Paxos {
        // TODO - as soon as paxos achieves consensus, it returns a list of all processes in the queue so that they all receive the same answer.
        private List<PaxosInstance> _paxosInstances; // stores <value, write_time_stamp, read_time_stamp>
        private Slots<PaxosSlotState> _paxosSlotState;    // stores the state of decision about a slot

        public static int Id { get; set; }

        public Paxos() {
            Id++;
        }

        public void Start() {
            // Start proposer thread
        }
    }

    internal class PaxosInstance
    {
        private Request _value;
        private uint _writeTimeStamp;
        private uint _readTimeStamp;

        public Request Value 
        {
            get { return _value; }
            set
            {
                lock(this)
                {
                    _value = value;
                }
            }
        }

        public uint WriteTimeStamp
        {
            get { return _writeTimeStamp; }
            set 
            { 
                lock(this)
                {
                    _writeTimeStamp = value;
                }
                
            }
        }

        public uint ReadTimeStamp
        {
            get { return _readTimeStamp;  }
            set
            {
                lock(this)
                {
                    _readTimeStamp = value;
                }
            }
        }
        

        public PaxosInstance()
        {
            _value = null;
            _writeTimeStamp = 0;
            _readTimeStamp = 0;
        }

    }

    internal class PaxosSlotState
    {
        enum ConsensusState
        {
            NOT_STARTED,
            WAITING,
            FINISHED
        }

        private Queue<uint> _waitingList;
        private ConsensusState _state;


        public PaxosSlotState()
        {
            _state = ConsensusState.WAITING;
            _waitingList = new Queue<uint>();
        }
    }



}
