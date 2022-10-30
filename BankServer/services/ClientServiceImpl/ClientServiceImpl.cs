using Grpc.Core;
using BankServer.domain;
using System.Net.Sockets;
using BankServer.utils;

namespace BankServer.services
{
    partial class ClientServiceImpl : ClientService.ClientServiceBase
    {
        ServerConfiguration _config;
        int _processId;
        BankSlotManager _bankSlotManager;
        BankManager _bankManager;
        List<bool> _log2PC;
        int logPosition;


        public ClientServiceImpl(ServerConfiguration config,int processId, BankSlotManager bankSlotManager, BankManager bankManager, List<bool> lista )
        {
            _bankManager = bankManager;
            _config = config;
            _bankSlotManager = bankSlotManager;
            _processId = processId;
            _log2PC = lista;
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
        public void IncrementLog()
        {
            logPosition += 1;
        }

        public int before2pc()
        {
            _log2PC[logPosition] = false;
            return logPosition;
        }
        public void after2pc(int pos)
        {
            _log2PC[pos] = true;
        }

        public bool getLogPosition(int pos)
        {
            return _log2PC[pos];
        }
    }
}
