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
    internal class BankSlotManager : IUpdatable
    {

        uint _slot = 0;
        ServerConfiguration _config;
        int _processID;
        int _maxSlots;

        public BankSlotManager(ServerConfiguration config,int processId) {
            _config = config;
            _processID = processId;
            _maxSlots = config.GetNumberOfSlots();
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

        public void BroadcastCompareAndSwap() {

            List<int> boneyAdresses = _config.GetBoneyServerIDs();
            (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(_processID);
            string address = "http://" + bankHost + ":" + bankPort;
            uint leader = ChooseLeader();

            foreach (int id in boneyAdresses) {
                    (string boneyHost, int boneyPort) = _config.GetBoneyHostnameAndPortByProcess(id);
                    Logger.LogDebug($"Sending to {boneyHost}:{boneyPort}");
                    //Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                    GrpcChannel channel = GrpcChannel.ForAddress("http://" + boneyHost + ":" + boneyPort);
                    CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
               
                    client.CompareAndSwap(new CompareAndSwapReq { Slot = _slot, Leader = leader, Address = address});
                }

            Logger.LogDebug("CompareAndSwap sent");
        }

        public void IncrementSlot() {
             _slot += 1;
        }

        public void Update() {
            IncrementSlot();
            Logger.LogDebug("BankSlotManager update");
            if (_slot > _maxSlots)
            {
                Logger.LogInfo("Max number of slots reached. Freezing process.");
                while (true) ;

            }
            BroadcastCompareAndSwap();
            Logger.LogDebug("BankSlotManager end of update");
        }


    }
}
