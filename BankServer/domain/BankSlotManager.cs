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
        int _processID;

        public BankSlotManager(ServerConfiguration config,int processId) {
            _config = config;
            _processID = processId;
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

            List<int> boneyAdresses = _config.GetBoneyServerIDs();

                foreach (int id in boneyAdresses) {
                    (string, int) tuplo = _config.GetBoneyHostnameAndPortByProcess(id);
                    Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                    GrpcChannel channel = GrpcChannel.ForAddress(tuplo.Item1 + ":" + tuplo.Item2);
                    CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                
                (string,int) tuplo2 = _config.GetBankHostnameAndPortByProcess(_processID);
                string address = "http://" + tuplo2.Item1 + ":" + tuplo2.Item2;
                client.CompareAndSwap(new CompareAndSwapRequest { Slot = _slot, Leader = ChooseLeader() , Address = address});
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
