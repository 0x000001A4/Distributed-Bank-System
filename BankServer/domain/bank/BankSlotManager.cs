using BankServer.utils;

namespace BankServer.domain.bank
{
    public class BankSlotManager
    {

        uint _slot = 0;
        ServerConfiguration _config;
        int _maxSlots;

        public BankSlotManager(ServerConfiguration config)
        {
            _config = config;
            _maxSlots = config.GetNumberOfSlots() + 1;
        }

        public uint ChooseLeader()
        {
            List<int> bankIds = _config.GetBankServerIDs();
            uint leaderId = (uint)bankIds[0];

            foreach (int id in bankIds)
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

        public void IncrementSlot()
        {
            _slot += 1;
        }

        public uint GetCurrentSlot()
        {
            return _slot;
        }

        public int GetMaxSlots()
        {
            return _maxSlots;
        }
    }
}
