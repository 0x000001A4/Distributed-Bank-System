using BoneyServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneyServer.utils;
using BoneyServer.domain;
using BoneyServer.services;

namespace BoneyServer
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BoneyServerState : IUpdateState{
        private int _slot;
        private Slots<string> _frozenSlots;
        private Dictionary<uint, string>[] _suspectedProcessesSlots;

        private uint _processId;
        private uint _numberOfBoneyProcesses;
        private int _currentSlot;
        private IMultiPaxos _paxos;
        private ServerConfiguration _config;

        private Queue<Message> _queue { get; set; } = new Queue<Message>();

        public BoneyServerState(IMultiPaxos paxos /*uint processId*/, ServerConfiguration config
                                )
        {
            _slot = 1;
            _paxos = paxos;
            _config = config;
            
            //_processId = processId;
            //_numberOfBoneyProcesses = (uint)config.GetNumberOfBoneyServers();
            //_multiPaxos = new Paxos(processId, )

            //for (uint slot = 0 ; slot < numberOfSlots; slot++ )
            //{
            //    _frozenSlots[slot] = frozenSlots[slot, processId];

            //    for (uint process = 0; process < numberOfProcesses; process++)
            //    {
            //        _suspectedProcessesSlots[process][slot] = suspectedProcessesSlots[slot, process];
            //    }

            //}
        }



        public bool IsFrozen()
        {
            return _frozenSlots[_currentSlot].Equals(FrozenState.FROZEN);
        }

        /* TODO - Rick!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! fantasmaaaa
        public void handleQueuedMessage(CompareAndSwapServiceImpl service, Message _msg)
        {

            if (_msg.getRequestId() == 1)
            {
                service.CompareAndSwap(_msg.getCompareAndSwapRequest());
            }


        }*/

        public void Enqueue(Message _msg)
        {
            _queue.Enqueue(_msg);
        }


        public void incrementSlot()
        {
            _slot += 1;
        }



        public void update()
        {
            Dictionary<uint, string> servers = new Dictionary<uint, string>();
            List<int> boneysID = _config.GetBoneyServerIDs();
            foreach (int id in boneysID)
            {
                servers.Add((uint)id, _config.GetServerSuspectedInSlot((uint)id, (uint)_slot));
            }
            _paxos.UpdateServers(servers);
            incrementSlot();
        }

       



    }
}

