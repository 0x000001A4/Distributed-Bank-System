using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class Slot {

        //private uint leaderId;
        //private uint primaryId;
        private Paxos _paxos;

        public Slot() { 
            _paxos = new Paxos();
        }
    }
}
