using Grpc.Net.Client;
using BankServer.utils;
using BankServer.domain.bank;

namespace BankServer.domain
{
    public interface ITwoPhaseCommit
    {
        void Start(uint slot, uint clientID, int processID);
        void AcceptProposedSeqNum(int seqToAccept);
        void WaitForCommit(uint clientID);
        void HandleCommit(int seqToCommit, uint clientID);
    }
    internal class ClientState
    {
        public bool Commited;
        public readonly object Lock;
        public ClientState()
        {
            Commited = false;
            Lock = new object();
        }
    }

    public class TwoPhaseCommit : ITwoPhaseCommit
    {
        BankFrontend _bankFrontend;
        private List<bool> _seqNumbersCommitedState;
        private Dictionary<uint, ClientState> _clientsState;
        private int _seqNumber = 0;

        public TwoPhaseCommit(ServerConfiguration config)
        {
            _bankFrontend = new BankFrontend(config);
            _seqNumbersCommitedState = new List<bool>();
            _clientsState = new Dictionary<uint, ClientState>();
        }





        public void Start(uint slot, uint clientID,int processID)
        {
            int seqToPropose;
            lock (this) { seqToPropose = getSeqNumToPropose(); }
            sendPropose(slot, seqToPropose,processID);
            sendCommit(seqToPropose, clientID);
        }

        public void AcceptProposedSeqNum(int seqToAccept)
        {
            setSeqNumAsProposed(seqToAccept);
        }

        public void WaitForCommit(uint clientID)
        {
            lock(getClientLock(clientID))
            {
                Monitor.Wait(getClientLock(clientID));
            }
        }

        public void HandleCommit(int seqToCommit, uint clientID)
        {
            lock (getClientLock(clientID))
            {
                setSeqNumAsCommited(seqToCommit);
                setClientAsCommited(clientID);
                Monitor.Pulse(getClientLock(clientID));
            }
        }



        private bool sendPropose(uint slot, int seqToPropose,int processID)
        {
            List<ProposeResp> respReceived = new List<ProposeResp>();
            object signal = new object();
            _bankFrontend.SendProposeSeqNumToAllBanks(slot, seqToPropose, respReceived, processID,signal);
            return waitForMajority(_bankFrontend.GetNumberOfBanks(), respReceived, signal);
        }


        private bool waitForMajority(int numberOfBanks, List<ProposeResp> responsePropose, 
            object signalAcceptSeqNum)
        {
            lock (signalAcceptSeqNum)
            {
                while (responsePropose.Count() < Math.Ceiling((decimal)numberOfBanks / 2))
                {
                    Monitor.Wait(signalAcceptSeqNum);
                }

                foreach (ProposeResp resp in responsePropose)
                {
                    if (!resp.Ack) return false;
                }

                return true;
            }
        }

        private void sendCommit(int seqToCommit, uint clientID)
        {
            _bankFrontend.SendCommitSeqNumToAllBanks(seqToCommit, clientID);
        }

        private int getSeqNumToPropose() // TODO: seqnumber may not be linear!
        {
            _seqNumbersCommitedState[_seqNumber] = false;
            return _seqNumber++;
        }
        private void setSeqNumAsProposed(int seq)
        {
            _seqNumbersCommitedState[seq] = false;
        }
        private void setSeqNumAsCommited(int pos)
        {
            _seqNumbersCommitedState[pos] = true;
        }
        private void setClientAsProposed(uint clientID)
        {
            _clientsState[clientID] = new ClientState();
        }
        private void setClientAsCommited(uint clientID)
        {
            checkifClientExists(clientID);
            _clientsState[clientID].Commited = true;
        }
        private object getClientLock(uint clientID)
        {
            checkifClientExists(clientID);
            return _clientsState[clientID].Lock;
        }

        private void checkifClientExists(uint clientID)
        {
            if (_clientsState[clientID] == null)
            {
                throw new Exception("Client does not exist. TwoPhaseCommit.cs (checkifClientExists())");
            }
        }
    }
}

