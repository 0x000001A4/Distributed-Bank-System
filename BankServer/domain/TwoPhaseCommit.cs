using Grpc.Net.Client;
using BankServer.utils;
using BankServer.domain.bank;

namespace BankServer.domain
{
    internal class ClientState
    {
        public int SeqNum;
        public readonly object Lock;
        public ClientState()
        {
            SeqNum = -1;
            Lock = new object();
        }
    }


    public interface ITwoPhaseCommit
    {
        void Start(uint slot, uint clientID, uint senderID);
        void AcceptProposedSeqNum(int seqToAccept);
        bool WaitForCommit(uint clientID);
        void HandleCommit(int seqToCommit, uint clientID);
    }

    public class TwoPhaseCommit : ITwoPhaseCommit
    {
        BankFrontend _bankFrontend;
        // false -> proposed, not commited
        // true  -> proposed, commited
        private List<bool> _seqNumbersCommitedState;
        private Dictionary<uint, ClientState> _clientsState;
        private int _seqNumber = 0;

        public TwoPhaseCommit(ServerConfiguration config)
        {
            _bankFrontend = new BankFrontend(config);
            _seqNumbersCommitedState = new List<bool>();
            _clientsState = new Dictionary<uint, ClientState>();
            List<int> clientIDs = config.GetClientIDs();
            foreach(uint clientID in clientIDs)
            {
                _clientsState.Add(clientID, new ClientState());
            }
        }





        public void Start(uint slot, uint clientID, uint senderID)
        {
            int seqToPropose;
            lock (this) { seqToPropose = getSeqNumToPropose(); }
            if (propose(slot, seqToPropose, senderID))
            {
                sendCommit(seqToPropose, clientID);
            }
        }

        public void AcceptProposedSeqNum(int seqToAccept)
        {
            initializeAllPrevSeqNumsUpTo(seqToAccept);
        }

        public bool WaitForCommit(uint clientID)
        {
            lock(getClientLock(clientID))
            {
                TimeoutTimer timeout = new TimeoutTimer();
                do 
                {
                    if (timeout.TimedOut()) return false;
                    Monitor.Wait(getClientLock(clientID)); 
                }
                while (hasntCommitedAllSeqNumsUntil(getClientSeqNum(clientID)));
                resetClientSeqNum(clientID);
                return true;
            }
        }

        public void HandleCommit(int seqToCommit, uint clientID)
        {
            checkSeqNum(seqToCommit);
            lock (getClientLock(clientID))
            {
                setSeqNumAsCommited(seqToCommit);
                setClientAsCommited(clientID, seqToCommit);
                notifyAllClientsWaitingOnPrevCommits();
            }
        }

        private void notifyAllClientsWaitingOnPrevCommits()
        {
            foreach(var clientState in _clientsState)
            {
                object clientLock = clientState.Value.Lock;
                Monitor.Pulse(clientLock);
            }
        }




        private bool hasntCommitedAllSeqNumsUntil(int seqNum)
        {
            for (int seq = 0; seq <= seqNum; seq++)
            {
                bool commited = _seqNumbersCommitedState[seqNum];
                if (!commited) return true;
            }
            return false;
        }


        private bool propose(uint slot, int seqToPropose, uint senderID)
        {
            List<ProposeResp> respReceived = new List<ProposeResp>();
            object signal = new object();
            _bankFrontend.SendProposeSeqNumToAllBanks(slot, seqToPropose, respReceived, senderID, signal);
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

        private void initializeAllPrevSeqNumsUpTo(int seq)
        {
            if (seq > _seqNumber)
            {
                for (int i = _seqNumber+1; i <= seq; i++)
                {
                    _seqNumbersCommitedState.Add(false);
                }
            }
        }

        private int getSeqNumToPropose() // TODO: seqnumber may not be linear!
        {
            _seqNumbersCommitedState.Add(false);
            return _seqNumber++;
        }
        private void resetClientSeqNum(uint clientID)
        {
            _clientsState[clientID].SeqNum = -1;
        }
        private void setSeqNumAsCommited(int pos)
        {
            _seqNumbersCommitedState[pos] = true;
        }
        private void setClientAsCommited(uint clientID, int seq)
        {
            checkifClientExists(clientID);
            _clientsState[clientID].SeqNum = seq;
        }
        private void setClientAsProposed(uint clientID)
        {
            checkifClientExists(clientID);
            _clientsState[clientID] = new ClientState();
        }
        private object getClientLock(uint clientID)
        {
            checkifClientExists(clientID);
            return _clientsState[clientID];
        }
        private int getClientSeqNum(uint clientID)
        {
            checkifClientExists(clientID);
            return _clientsState[clientID].SeqNum;
        }

        private void checkifClientExists(uint clientID)
        {
            try
            {
                var catchError = _clientsState[clientID];
            }
            catch(KeyNotFoundException e){
                Logger.LogError("Client does not exist. TwoPhaseCommit.cs (checkifClientExists())");
                Logger.NewLine();
                Logger.LogError(e.Message);
            }
        }

        private void checkSeqNum(int seq)
        {
            try
            {
                var catchError = _seqNumbersCommitedState[seq];
            }
            catch(IndexOutOfRangeException e)
            {
                Logger.LogError("SequenceNumber does not exist. TwoPhaseCommit.cs (checkSeqNum())");
                Logger.NewLine();
                Logger.LogError(e.Message);
            }
        }
    }
}

