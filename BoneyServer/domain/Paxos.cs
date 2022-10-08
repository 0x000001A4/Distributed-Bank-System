using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class Paxos
    {

        private uint _paxosInstance;



        public Paxos()
        {
            _paxosInstance = 0;
        }

        public uint StartNewInstance()
        {
            return ++_paxosInstance;
        }

        public uint getInstance()
        {
            return _paxosInstance;
        }

        public void Start()
        {
            // Start proposer thread
        }
    }
}
