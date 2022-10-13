using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.utils;

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
			uint process = (uint)_config.GetNumberOfBoneyServers()+1;
			uint leaderId;
			while (true)
			{
				if (_config.GetServerSuspectedInSlot(process, _slot) == SuspectState.NOTSUSPECTED) {
					leaderId =  process;
					break;
				}
				process+=1;
			}
			return leaderId;
		}

		public void BroadcastCompareAndSwap() {
			for (int i = 0; i < _config.GetNumberOfBoneyServers(); i++) {
				(string, int) tuplo = _config.GetBoneyHostnameAndPortByProcess(i + 1);
				Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
				GrpcChannel channel = GrpcChannel.ForAddress(tuplo.Item1 + ":" + tuplo.Item2);
				CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
				client.CompareAndSwap(new CompareAndSwapReq { Slot = _slot, Leader = ChooseLeader() });
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
