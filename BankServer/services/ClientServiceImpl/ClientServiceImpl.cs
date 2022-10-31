using Grpc.Core;
using BankServer.domain;
using System.Net.Sockets;
using BankServer.utils;
using BankServer.domain.bank;

namespace BankServer.services
{
    partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        ServerConfiguration _config;
        int _processId;
        BankSlotManager _bankSlotManager;
        BankManager _bankManager;
        ITwoPhaseCommit _2PC;
        int logPosition;


        public ClientServiceImpl(ServerConfiguration config,int processId, BankSlotManager bankSlotManager,
            BankManager bankManager, ITwoPhaseCommit _2pc )
        {
            _bankManager = bankManager;
            _config = config;
            _bankSlotManager = bankSlotManager;
            _processId = processId;
            _2PC = _2pc;
            logPosition = 0;
        }

        public bool verifyImLeader()
        {
            List<int> bankIds = _config.GetBankServerIDs();
            uint leaderId = (uint)bankIds[0];

            foreach (int id in bankIds)
            {
                if (_config.GetServerSuspectedInSlot((uint)id, _bankSlotManager.GetCurrentSlot()) == SuspectState.NOTSUSPECTED)
                {
                   
                    leaderId = (uint)id;
                    if (_processId == leaderId) return true;
                    else return false;
                }
            }
            return false;
        }

    }
}
