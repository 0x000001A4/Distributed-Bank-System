using BankServer.utils;
using Grpc.Core;
using Grpc.Net.Client;
using System.Security.Cryptography.X509Certificates;

namespace BankServer.domain.bank
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BankServerState : IUpdatable
    {

        private Server _server;
        private BankSlotManager _slotManager;
        private uint _processId;
        private uint _numberOfProcesses;
        private ServerConfiguration _config;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();
        private QueuedCommandHandler _cmdHandler;
        private ITwoPhaseCommit _2PC;

        public BankServerState(int processId, ServerConfiguration config, QueuedCommandHandler cmdHandler, ITwoPhaseCommit _2pc)
        {
            _slotManager = new BankSlotManager(config);
            _numberOfProcesses = (uint)config.GetNumberOfBankServers();
            _config = config;
            _processId = (uint)processId;
            _frozen = config.GetFrozenStateOfProcessInSlot(_processId, _slotManager.GetCurrentSlot());
            _cmdHandler = cmdHandler;
            _2PC = _2pc;
        }

        public uint GetProcessId()
        {
            return _processId;
        }

        public void Enqueue(Message _msg)
        {
            _queue.Enqueue(_msg);
        }


        public void HandleQueuedMessage(Message _msg)
        {
            string _sender = _msg.GetSender();
            uint msgId = _msg.GetRequestId();

            if (msgId == 1) {
                _cmdHandler.handlePaxosResult(_msg.GetCompareAndSwapResponse(), _sender);
            }

            else if (msgId == 2) {
                _cmdHandler.handleDepositReq(_msg.GetDepositReq(), _sender);
            }

            else if (msgId == 3) {
                _cmdHandler.handleWithdrawReq(_msg.GetWithdrawReq(), _sender);
            }

            else if (msgId == 4) {
                _cmdHandler.handleReadReq(_msg.GetReadReq(), _sender);
            }

            else if (msgId == 5) {
                _cmdHandler.handleListPendingRequestsReq(_msg.GetListPendingRequestsReq(), _sender);
            }

            else if (msgId == 6) {
                _cmdHandler.handleProposeReq(_msg.GetProposeReq(), _sender);
            }

            else if (msgId == 7) {
                _cmdHandler.handleCommitReq(_msg.GetCommitReq(), _sender);
            }
        }

        public void AddServer(Server server)
        {
            _server = server;
        }

        public void Stop()
        {
            HandleQueuedMessages();
            Logger.LogInfo("Bank Server State: Max number of slots reached. Shutting process down after processing queued requests.");
            _server.ShutdownAsync().Wait();
        }


        public void Update(uint tick)
        {
            var _prevSlotStatus = _frozen;

            // Update servers' own frozen state for new slot.
            _frozen = _config.GetFrozenStateOfProcessInSlot(_processId, _slotManager.GetCurrentSlot());

            // Set Configuration as complete (Needed to avoid crashing bank servers while they are configurating)
            _config.setAsConfigured();

            // Check if bankServer server just unfroze!
            if (_slotManager.GetCurrentSlot() > 1 && _prevSlotStatus == FrozenState.FROZEN && _frozen == FrozenState.UNFROZEN)
            {
                HandleQueuedMessages();
            }

            Logger.LogDebug("BankSlotManager update");
            if (_slotManager.GetCurrentSlot() > _slotManager.GetMaxSlots())
            {
                Logger.LogInfo("Max number of slots reached. Freezing process.");
                while (true) ;

            }

            BroadcastCompareAndSwap();
            _slotManager.IncrementSlot();
            Logger.LogDebug("BankSlotManager end of update");
        }


        public void HandleQueuedMessages()
        {
            while (_queue.Count > 0)
            {
                Message msg = _queue.Dequeue();
                Logger.LogDebug($"Dequeued: {msg} with MessageId: {msg.GetRequestId()}");
                HandleQueuedMessage(msg);
                Logger.LogDebug($"Exited HandleQueuedMessage with Message Id: {msg.GetRequestId()}");
            }
        }

        public bool IsFrozen()
        {
            return _frozen == FrozenState.FROZEN;
        }


        public uint GetNumberOfBankProcesses()
        {
            return _numberOfProcesses;
        }

        public BankSlotManager GetSlotManager()
        {
            return _slotManager;
        }

        public bool isConfigured()
        {
            return _config.hasFinished();
        }


        public void BroadcastCompareAndSwap()
        {
            List<int> boneyAdresses = _config.GetBoneyServerIDs();
            (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess((int)_processId);
            string address = "http://" + bankHost + ":" + bankPort;
            uint leader = _slotManager.ChooseLeader();

            foreach (int id in boneyAdresses)
            {
                (string boneyHost, int boneyPort) = _config.GetBoneyHostnameAndPortByProcess(id);
                Logger.LogDebug($"Sending to {boneyHost}:{boneyPort}");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + boneyHost + ":" + boneyPort);
                CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);

                Logger.LogDebug("CompareAndSwap sent");
                client.CompareAndSwapAsync(new CompareAndSwapReq { Slot = _slotManager.GetCurrentSlot(), Leader = leader, Address = address });
            }
        }

        private void BroadcastListPendingRequests(List<List<ClientRequest>> _responses, object signalAcceptSeqNum)
        {
            try {
                List<int> bankAddresses = _config.GetBankServerIDs();
                foreach (int id in bankAddresses)
                {
                        (string bankHost, int bankPort) = _config.GetBankHostnameAndPortByProcess(id);
                        Logger.LogDebug($"Sending to {bankHost}:{bankPort}");
                        GrpcChannel channel = GrpcChannel.ForAddress("http://" + bankHost + ":" + bankPort);
                        BankService.BankServiceClient client = new BankService.BankServiceClient(channel);
                        Task ret = RequestListPendingRequestsAsync(channel, _responses, signalAcceptSeqNum);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<ListPendingRequestsResp> RequestListPendingRequestsAsync(GrpcChannel channel, List<List<ClientRequest>> _responses, object signalAcceptSeqNum) {
            BankService.BankServiceClient _client = new BankService.BankServiceClient(channel);
            Logger.LogDebug("ListPendingRequests sent");
            ListPendingRequestsResp response = await _client.ListPendingRequestsAsync(new ListPendingRequestsReq { LastKnownSeqNumber = (uint)_2PC.GetSequenceNumber() }) ;
            lock(_responses)
            {
                List<ClientRequestMsg> clientPendingRequests = response.PendingRequests.ToList();
                List<ClientRequest> _pendingRequests = new List<ClientRequest>();
                foreach (ClientRequestMsg clientReq in clientPendingRequests) {
                    _pendingRequests.Add(new ClientRequest(clientReq.ClientId, clientReq.SeqNum, clientReq.Commited));
                }
                _responses.Add(_pendingRequests);
                Monitor.Pulse(signalAcceptSeqNum);
            }
            return response;
        }

        private void WaitForMajority(List<List<ClientRequest>> _responses, object signalAcceptSeqNum)
        {
            lock (signalAcceptSeqNum)
            {
                while (_responses.Count() < Math.Ceiling((decimal)_numberOfProcesses / 2)) {
                    Monitor.Wait(signalAcceptSeqNum);
                }
            }
        }

        private void ProposeAndCommitSeqNumbers(List<ClientRequest> _clientPendingRequests) {
            foreach(ClientRequest clientRequest in _clientPendingRequests) {
                uint seqNum = clientRequest.GetSeqNum();
                _2PC.StartAsCleanup(_slotManager.GetCurrentSlot(), clientRequest.GetClientId(), _processId, (int)seqNum);
            }
        }

        public void Cleanup()
        {
            List<List<ClientRequest>> clientPendingRequests = new List<List<ClientRequest>>();
            object signalAcceptSeqNum = new object();
            BroadcastListPendingRequests(clientPendingRequests, signalAcceptSeqNum);
            WaitForMajority(clientPendingRequests, signalAcceptSeqNum);
            List<ClientRequest> _clientPendingRequests = FilterProposedButNotCommitedRequests(clientPendingRequests);
            ProposeAndCommitSeqNumbers(_clientPendingRequests);
        }

        public List<ClientRequest> FilterProposedButNotCommitedRequests(List<List<ClientRequest>> _clientRequests)
        {
            List<ClientRequest> _pendingClientRequests = new List<ClientRequest>();
            foreach (List<ClientRequest> clientRequests in _clientRequests)
            {
                for (int seqNum = 0; seqNum < clientRequests.Count; seqNum++) {
                    if (clientRequests[seqNum].GetClientId() != -1 && !hasBeenCommited(_clientRequests, seqNum)) {
                        _pendingClientRequests.Add(new ClientRequest(clientRequests[seqNum].GetClientId(), (uint)seqNum, false));
                    }
                }
            }
            return _pendingClientRequests;
        }
        
        public bool hasBeenCommited(List<List<ClientRequest>> _clientRequests, int seqNum)
        {
            foreach (List<ClientRequest> clientRequests in _clientRequests) {
                try {
                    if (clientRequests[seqNum].isCommited()) return true;
                } catch (ArgumentOutOfRangeException _) {
                    Console.WriteLine("Sequence Number has not been proposed yet");
                }
            }
            return false;
        }

    }
    public class ClientRequest
    {
        private int _clientId;
        private uint _seqNum;
        private bool _commited;

        public ClientRequest(int clientId, uint seqNum, bool state)
        {
            _clientId = clientId;
            _seqNum = seqNum;
            _commited = state;
        }

        public int GetClientId() {
            return _clientId;
        }

        public bool isCommited() {
            return _commited;
        }

        public uint GetSeqNum()
        {
            return _seqNum;
        }
    }
}