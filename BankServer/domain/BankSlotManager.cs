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
        List<GrpcChannel> _channels;

        public BankSlotManager(ServerConfiguration config) {
            _config = config;
            _channels = new List<GrpcChannel>();
            var servers = config.GetBoneyServersPortsAndAddresses();
            foreach(var server in servers)
            {
                _channels.Add(GrpcChannel.ForAddress("https://" + server));
            }
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
            foreach (var channel in _channels) {
                CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                client.CompareAndSwap(new CompareAndSwapRequest { Slot = _slot, Leader = ChooseLeader() });
            }
            Logger.LogDebug("CompareAndSwap sent");
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
