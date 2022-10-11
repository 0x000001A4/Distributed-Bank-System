using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServer.utils;

namespace BankServer.domain
{

    

    internal class BankSlotManager : IUpdateState
    {

    public delegate void MyDelegate();

    int _slot;

    ServerConfiguration _config;


    public BankSlotManager(ServerConfiguration config)
    {
        _config = config;

    }
    public int chooseLeader()
    {
            Console.Write("Meio" + "\n");
            int process = _config.GetNumberOfBoneyServers()+1;
        int leader;
        while (true)
        {
            if (_config.GetServerSuspetInSlot(process, _slot) == SuspectState.NOTSUSPECTED)
            {
                leader = process;
                break;
            }
            process+=1;
        }
        return leader;
    }

    public void executeCompareandSwap()
    {
            for (int i = 0; i < _config.GetNumberOfBoneyServers(); i++)
            {
                (string, int) tuplo = _config.GetBoneyHostnameAndPortByProcess(i + 1);
                Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                GrpcChannel channel = GrpcChannel.ForAddress(tuplo.Item1 + ":" + tuplo.Item2);
                CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                client.CompareAndSwap(new CompareAndSwapRequest { Slot = (uint)_slot, Leader = (uint)chooseLeader() });
  
            }


    }

    public void incrementSlot()
    {

         _slot+=1;
    }


    public void  update()

    {

            executeCompareandSwap();
            incrementSlot();
        }


    }
}
