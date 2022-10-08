using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    internal class Paxos {

        public static int Id { get; set; }

        public Paxos() {
            Id++;
        }

        public void Start() {
            // Start proposer thread
        }
    }
}
