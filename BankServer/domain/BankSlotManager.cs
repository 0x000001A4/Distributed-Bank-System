using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.utils;
using System.Diagnostics;

namespace BankServer.domain
{
    public class BankSlotManager {

        Slots<uint> _slots;
        uint _slot = 0;
        ServerConfiguration _config;
        int _maxSlots;

        public BankSlotManager(ServerConfiguration config) {
            _config = config;
            _maxSlots = config.GetNumberOfSlots()+1;
        }

        public uint ChooseLeader() {
            List<int> bankIds = _config.GetBankServerIDs();
            uint leaderId = (uint) bankIds[0];

            foreach(int id in bankIds)
            {
                if (_config.GetServerSuspectedInSlot((uint)id, _slot) == SuspectState.NOTSUSPECTED)
                {
                    leaderId = (uint)id;
                    break;
                }
            }
            Logger.LogDebug($"Leader chosen {leaderId}");
            return leaderId;
        }

        public void IncrementSlot() {
             _slot += 1;
        }

        public uint GetCurrentSlot()
        {
            return _slot;
        }

        public int GetMaxSlots() {
            return _maxSlots;
        }

        public void SetPrimaryOnSlot(uint slotId, uint primary)
        {
            _slots[(int)slotId] = primary;
        }


        public uint GetPrimaryOnSlot(uint slotId)
        {
            return _slots[(int)slotId];
        }
    }
}
