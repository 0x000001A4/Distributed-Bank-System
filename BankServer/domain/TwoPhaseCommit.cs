using Grpc.Net.Client;
using BankServer.utils;
using BankServer.domain.bank;

namespace BankServer.domain
{
    public interface ITwoPhaseCommit
    {
        void Start(uint slot, int clientID, uint senderID);
        void StartAsCleanup(uint slot, int clientID, uint senderID, int seqNum);
        void AcceptProposedSeqNum(int seqToAccept);
        bool WaitForCommit(int clientID);
        void HandleCommit(int seqToCommit, int clientID);
        List<ClientRequest> GetClientRequests();
        int GetSequenceNumber();

        void UpdateSequenceNumber(int sequenceNumber);
    }

    internal class ClientState
    {
        public static readonly int NOT_INITIALIZED = -1;
        public int SeqNum;
        public readonly object Lock;
        public ClientState()
        {
            SeqNum = NOT_INITIALIZED;
            Lock = new object();
        }
    }

    public class TwoPhaseCommit : ITwoPhaseCommit
    {
        BankFrontend _bankFrontend;
        private Dictionary<int, ClientState> _clientsState;
        private List<ClientRequest> _clientRequests = new List<ClientRequest>();
        private int _seqNumber = 0;

        public TwoPhaseCommit(ServerConfiguration config)
        {
            _bankFrontend = new BankFrontend(config);
            _clientRequests = new List<ClientRequest>();
            _clientsState = new Dictionary<int, ClientState>();
            List<int> clientIDs = config.GetClientIDs();
            foreach (int clientID in clientIDs)
            {
                _clientsState.Add(clientID, new ClientState());
            }
        }

        public void Start(uint slot, int clientID, uint senderID)
        {
            Logger.LogDebug2PC($"Start: start");
            int seqToPropose;
            lock (this) { seqToPropose = getSeqNumToPropose(); }
            if (propose(slot, seqToPropose, senderID))
                _clientRequests.Add(new ClientRequest(clientID, seqToPropose, false));
            {
                sendCommit(seqToPropose, clientID);
                Logger.LogDebug2PC($"Start: succesfully commited (seq: {seqToPropose}, clientID: {clientID})");
            }
            Logger.LogDebug2PC($"Start: received at least one NACK (could not commit)");
        }

        public void AcceptProposedSeqNum(int seqToAccept)
        {
            Logger.LogDebug2PC($"AcceptProposedSeqNum: start");
            initializeAllPrevSeqNumsUpTo(seqToAccept);
            Logger.LogDebug2PC($"AcceptProposedSeqNum: end");
        }

        public bool WaitForCommit(int clientID)
        {
            Logger.LogDebug2PC($"WaitForCommit: start");
            Logger.LogDebug2PC($"WaitForCommit: trying to aquire lock for client {clientID}");
            lock (getClientLock(clientID))
            {
                Logger.LogDebug2PC($"WaitForCommit: lock aquired for client {clientID}");
                TimeoutTimer timeout = new TimeoutTimer();
                do
                {
                    if (timeout.TimedOut())
                    {
                        Logger.LogDebug2PC($"WaitForCommit: TimedOut {clientID}");
                        return false;
                    }
                    Logger.LogDebug2PC($"WaitForCommit: releasing lock for client {clientID}");
                    Monitor.Wait(getClientLock(clientID));
                    Logger.LogDebug2PC($"WaitForCommit: received a pulse");
                }
                while (hasntCommitedAllSeqNumsUntil(getClientSeqNum(clientID)));
                Logger.LogDebug2PC($"WaitForCommit: All seqNums before {getClientSeqNum(clientID)} have been received");
                resetClientSeqNum(clientID);
                return true;
            }
        }

        public void HandleCommit(int seqToCommit, int clientID)
        {
            Logger.LogDebug2PC($"HandleCommit: start of handling commit");
            checkSeqNum(seqToCommit);
            Logger.LogDebug2PC($"HandleCommit: trying to aquire lock for client {clientID}");
            Logger.LogDebug2PC($"HandleCommit: lock aquired for client {clientID}");
            setSeqNumAsCommited(seqToCommit);
            setClientAsCommited(clientID, seqToCommit);
            Logger.LogDebug2PC($"HandleCommit: notifying all clients waiting on previous commits");
            notifyAllClientsWaitingOnPrevCommits();
            Logger.LogDebug2PC($"HandleCommit: Successfully notified all clients waiting on prev commits");

        }


        public List<ClientRequest> GetClientRequests() {
            return _clientRequests;

        }

        public void StartAsCleanup(uint slot, int clientID, uint senderID, int seqNum)
        {
            Logger.LogDebug2PC($"StartAsCleanup: starting");
            if (propose(slot, seqNum, senderID))
            {
                sendCommit(seqNum, clientID);
                Logger.LogDebug2PC($"StartAsCleanup: ended with a majority of ACK (commited)");
            }
            Logger.LogDebug2PC($"StartAsCleanup: ended at least one NACK (could not commit)");
        }

        public int GetSequenceNumber()
        {
            return _seqNumber;
        }


        public void UpdateSequenceNumber(int seqNumber)
        {
            _seqNumber = seqNumber;
            initializeAllPrevSeqNumsUpTo(seqNumber);
        }



        private void notifyAllClientsWaitingOnPrevCommits()
        {
            Logger.LogDebug2PC($"Pulsing: start");
            foreach (var clientState in _clientsState)
            {
                lock (clientState.Value.Lock)
                {
                    Monitor.Pulse(clientState.Value.Lock);
                }
                
            }
            Logger.LogDebug2PC($"Pulsing: pulsed to all clients");
        }




        private bool hasntCommitedAllSeqNumsUntil(int seqNum)
        {
            for (int seq = 0; seq <= seqNum; seq++)
            {
                bool commited = _clientRequests[seqNum].isCommited();
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

        private void sendCommit(int seqToCommit, int clientID)
        {
            _clientRequests[seqToCommit].Commit();
            
            _bankFrontend.SendCommitSeqNumToAllBanks(seqToCommit, clientID);
        }

        private void initializeAllPrevSeqNumsUpTo(int seq)
        {
            Logger.LogDebug2PC($"initializeAllPrevSeqNumsUpTo: current seq: " + seq);
            if (seq >= _seqNumber)
            {
                for (int seqCounter = _seqNumber; seqCounter <= seq; seqCounter++)
                {
                    _clientRequests.Add(new ClientRequest(-1, seqCounter, false));
                }
            }
            Logger.LogDebug2PC($"initializeAllPrevSeqNumsUpTo: initiialzied all to false up to : " + seq);
        }

        private int getSeqNumToPropose() // TODO: seqnumber may not be linear!
        {
            return _seqNumber++;
        }
        private void resetClientSeqNum(int clientID)
        {
            _clientsState[clientID].SeqNum = -1;
        }
        private void setSeqNumAsCommited(int pos)
        {
            _clientRequests[pos].Commit();
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
            catch (Exception e)
            {
                Logger.LogError("Client does not exist. TwoPhaseCommit.cs (checkifClientExists())");
                Logger.NewLine();
                Logger.LogError(e.Message);
            }
        }

        private void checkSeqNum(int seq)
        {
            Logger.LogDebug2PC($"checkSeqNum: start");
            try
            {
                var catchError = _clientRequests[seq];
            }
            catch (Exception e)
            {
                Logger.LogError("SequenceNumber does not exist. TwoPhaseCommit.cs (checkSeqNum())");
                Logger.NewLine();
                Logger.LogError(e.Message);
            }
            Logger.LogDebug2PC($"checkSeqNum: start");
        }
    }
}

