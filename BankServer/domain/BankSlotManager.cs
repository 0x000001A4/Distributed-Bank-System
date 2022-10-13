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

        uint _slot=1;
        ServerConfiguration _config;

        public BankSlotManager(ServerConfiguration config) {
            _config = config;
        }

        public uint ChooseLeader() {
            List<int> boneyIds = _config.GetBoneyServerIDs();
            uint leaderId = (uint) boneyIds[0];

            foreach(int id in boneyIds)
            {
                if (_config.GetServerSuspectedInSlot((uint)id, _slot) == SuspectState.NOTSUSPECTED)
                {
                    leaderId = (uint)id;
                    break;
                }
            }

            return leaderId;
        }

        public void BroadcastCompareAndSwap() {
                for (int i = 0; i < _config.GetNumberOfBoneyServers(); i++) {
                    (string, int) tuplo = _config.GetBoneyHostnameAndPortByProcess(i + 1);
                    Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                    GrpcChannel channel = GrpcChannel.ForAddress(tuplo.Item1 + ":" + tuplo.Item2);
                    CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                    client.CompareAndSwap(new CompareAndSwapRequest { Slot = _slot, Leader = ChooseLeader() });
                }
        }

        public void IncrementSlot() {
             _slot += 1;
        }

        public void Update() {
            BroadcastCompareAndSwap();
            IncrementSlot();
        }


    }
}
