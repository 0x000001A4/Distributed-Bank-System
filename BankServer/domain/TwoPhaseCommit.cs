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
        void Start(uint slot, int clientID, uint senderID);
        void StartAsCleanup(uint slot, int clientID, uint senderID, int seqNum);
        void AcceptProposedSeqNum(int seqToAccept);
        bool WaitForCommit(int clientID);
        void HandleCommit(int seqToCommit, int clientID);
        List<ClientRequest> GetClientRequests();
        int GetSequenceNumber();
    }

    public class TwoPhaseCommit : ITwoPhaseCommit
    {
        BankFrontend _bankFrontend;
        // false -> proposed, not commited
        // true  -> proposed, commited
        private List<bool> _seqNumbersCommitedState;
        private Dictionary<int, ClientState> _clientsState;
        private int _seqNumber = 0;
        private List<ClientRequest> _clientRequests = new List<ClientRequest>();

        public TwoPhaseCommit(ServerConfiguration config)
        {
            _bankFrontend = new BankFrontend(config);
            _seqNumbersCommitedState = new List<bool>();
            _clientsState = new Dictionary<int, ClientState>();
            List<int> clientIDs = config.GetClientIDs();
            foreach(int clientID in clientIDs)
            {
                _clientsState.Add(clientID, new ClientState());
            }
        }

        public int GetSequenceNumber()
        {
            return _seqNumber;
        }

        public void Start(uint slot, int clientID, uint senderID)
        {
            int seqToPropose;
            lock (this) { seqToPropose = getSeqNumToPropose(); }
            if (propose(slot, seqToPropose, senderID))
            {
                _clientRequests.Add(new ClientRequest(clientID, (uint)seqToPropose, false));
                sendCommit(seqToPropose, clientID);
            }
        }

        public void StartAsCleanup(uint slot, int clientID, uint senderID, int seqNum) {
            if (propose(slot, seqNum, senderID)) {
                sendCommit(seqNum, clientID);
            }
        }

        public void AcceptProposedSeqNum(int seqToAccept)
        {
            initializeAllPrevSeqNumsUpTo(seqToAccept);
        }

        public bool WaitForCommit(int clientID)
        {
            Console.WriteLine("Waiting for commit ");

            lock (getClientLock(clientID))
            {
                TimeoutTimer timeout = new TimeoutTimer();
                do 
                {
                    if (timeout.TimedOut()) {
                        return false;
                    }
                    Logger.LogInfo("before" + clientID);
                    Monitor.Wait(getClientLock(clientID));
                    Logger.LogInfo("after" + clientID);
                }
                while (hasntCommitedAllSeqNumsUntil(getClientSeqNum(clientID)));

                resetClientSeqNum(clientID);
                return true;
            }
        }

        public void HandleCommit(int seqToCommit, int clientID)
        {
            Console.WriteLine($"handlingCommit: seqNum {seqToCommit}, clientId {clientID}");
            checkSeqNum(seqToCommit);

            lock (getClientLock(clientID)) {
                setSeqNumAsCommited(seqToCommit);
                setClientAsCommited(clientID, seqToCommit);
                notifyAllClientsWaitingOnPrevCommits();
            }
        }

        private void notifyAllClientsWaitingOnPrevCommits()
        {
            foreach(var clientState in _clientsState) {
                Console.WriteLine("PUUUUUUUUUUULSE:" + clientState.Key);
                Monitor.Pulse(getClientLock(clientState.Key));
            }
        }




        private bool hasntCommitedAllSeqNumsUntil(int seqNum)
        {
            for (int seq = 0; seq <= seqNum; seq++)
            {
                bool commited = _seqNumbersCommitedState[seqNum];
                Console.WriteLine($"SeqNum: {seqNum} is commited? ${commited}");
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
                    Console.WriteLine($"received {responsePropose.Count} proposes");
                    Monitor.Wait(signalAcceptSeqNum);
                }

                foreach (ProposeResp resp in responsePropose)
                {
                    if (!resp.Ack)
                    {
                        Console.WriteLine("Got false as response :(");
                        return false;
                    }
                }
                Console.WriteLine("Waited for majority :)");
                return true;
            }
        }

        private void sendCommit(int seqToCommit, int clientID)
        {
            Console.WriteLine("Sending Commit to all banks");
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
        private void resetClientSeqNum(int clientID)
        {
            _clientsState[clientID].SeqNum = -1;
        }
        private void setSeqNumAsCommited(int pos)
        {
            _seqNumbersCommitedState[pos] = true;
        }
        private void setClientAsCommited(int clientID, int seq)
        {
            checkifClientExists(clientID);
            _clientsState[clientID].SeqNum = seq;
        }
        private void setClientAsProposed(int clientID)
        {
            checkifClientExists(clientID);
            _clientsState[clientID] = new ClientState();
        }
        private object getClientLock(int clientID)
        {
            checkifClientExists(clientID);
            return _clientsState[clientID].Lock;
        }
        private int getClientSeqNum(int clientID)
        {
            checkifClientExists(clientID);
            return _clientsState[clientID].SeqNum;
        }

        private void checkifClientExists(int clientID)
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

        public List<ClientRequest> GetClientRequests()
        {
            return _clientRequests;
        }
    }
}

