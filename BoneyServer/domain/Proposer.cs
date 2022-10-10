using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    public class Proposer
    {
        static uint _sourceLeaderNumber;
        static int _instance;
        static List<String> _boneyAdress;
        public static void proposeWork(uint sourceLeaderNumber,
            int instance,List<String> boneyAdress)
        {
            _sourceLeaderNumber = sourceLeaderNumber;
            _instance = instance;
            _boneyAdress = boneyAdress;









        }


    
     
















    }
}
