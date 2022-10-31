using BankServer.utils;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.domain.bank
{
    public class BankFrontend
    {
        private ServerConfiguration _config;
        private List<GrpcChannel> _bankChannels;
        public BankFrontend(ServerConfiguration config)
        {
            _config = config;
            _bankChannels = new List<GrpcChannel>();
            List<int> bankAdresses = _config.GetBankServerIDs();
            foreach (int id in bankAdresses)
            {
                (string host, int port) = _config.GetBoneyHostnameAndPortByProcess(id);
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + host + ":" + port);
                _bankChannels.Add(channel);
            }
        }

        public void SendProposeSeqNumToAllBanks(uint slot, int seqToPropose, List<ProposeResp> respReceived, 
            uint processID,object signalAcceptSeqNum)
        {
            foreach (GrpcChannel channel in _bankChannels)
            {
                Logger.LogDebug($"Sending Propose({seqToPropose}) to {channel.Target}");
                ProposeReq request = new ProposeReq { Slot = slot, SeqNumber = (uint)seqToPropose , PrimaryBankID = (uint)processID };
                Task res = ProposeSeqNumAsync(request, channel, respReceived, signalAcceptSeqNum);
            }
        }

        private async Task ProposeSeqNumAsync(ProposeReq request, GrpcChannel channel, List<ProposeResp> responsesReceived,
            object signalAcceptSeqNum)
        {
            BankService.BankServiceClient client = new BankService.BankServiceClient(channel);
            ProposeResp response = await client.ProposeSeqNumAsync(request);
            lock (signalAcceptSeqNum)
            {
                responsesReceived.Add(response);
                Monitor.Pulse(signalAcceptSeqNum);
            }
        }



        public void SendCommitSeqNumToAllBanks(int seqToCommit, uint clientID)
        {
            foreach (GrpcChannel channel in _bankChannels)
            {
                Logger.LogDebug($"Sending Commit(seq: {seqToCommit}, clientID: {clientID}) to {channel.Target}");
                CommitReq request = new CommitReq { SeqNumber = (uint)seqToCommit, ClientID = clientID };
                Task res = CommitSeqNumAsync(request, channel);
            }
        }

        private async Task CommitSeqNumAsync(CommitReq request, GrpcChannel channel)
        {
            BankService.BankServiceClient client = new BankService.BankServiceClient(channel);
            CommitResp response = await client.CommitSeqNumAsync(request);
        }


        public int GetNumberOfBanks()
        {
            return _bankChannels.Count();
        }
    }
}
