﻿using BankServer.utils;
using Grpc.Core;
using Grpc.Net.Client;

namespace BankServer.domain
{
    /// <summary>
    /// Stores Boney Server's state. This includes all slots information.
    /// </summary>
    public class BankServerState : IUpdatable {

        private BankSlotManager _slotManager;
        private uint _processId;
        private uint _numberOfProcesses;
        private ServerConfiguration _config;

        private string _frozen;
        private Queue<Message> _queue { get; set; } = new Queue<Message>();
        private QueuedCommandHandler _cmdHandler;

        public BankServerState(int processId, ServerConfiguration config, QueuedCommandHandler cmdHandler, BankSlotManager slotManager)
        {
            _slotManager = slotManager;
            _numberOfProcesses = (uint)config.GetNumberOfBankServers();
            _config = config;
            _processId = (uint)processId;
            _frozen = config.GetFrozenStateOfProcessInSlot(_processId, _slotManager.GetCurrentSlot());
            _cmdHandler = cmdHandler;
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
        }



        public void Update()
        {
            var _prevSlotStatus = _frozen;
            _slotManager.IncrementSlot();

            // Update servers' own frozen state for new slot.
            _frozen = _config.GetFrozenStateOfProcessInSlot(_processId, _slotManager.GetCurrentSlot());

            // Set Configuration as complete (Needed to avoid crashing boney servers while they are configurating)
            _config.setAsConfigured();

            // Check if bankServer server just unfroze!
            if (_slotManager.GetCurrentSlot() > 1 && _prevSlotStatus == FrozenState.FROZEN && _frozen == FrozenState.UNFROZEN) {
                HandleQueuedMessages();
            }

            Logger.LogDebug("BankSlotManager update");
            if (_slotManager.GetCurrentSlot() > _slotManager.GetMaxSlots())
            {
                Logger.LogInfo("Max number of slots reached. Freezing process.");
                while (true) ;

            }

            BroadcastCompareAndSwap();
            Logger.LogDebug("BankSlotManager end of update");
        }


        public void HandleQueuedMessages() {
            // If yes handle Queued messages!
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

        public bool isConfigured() {
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
                //Console.Write("Item1 " +tuplo.Item1 + " Item2 " + tuplo.Item2+"\n");
                GrpcChannel channel = GrpcChannel.ForAddress("http://" + boneyHost + ":" + boneyPort);
                CompareAndSwapService.CompareAndSwapServiceClient client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);

                Logger.LogDebug("CompareAndSwap sent");
                client.CompareAndSwapAsync(new CompareAndSwapReq { Slot = _slotManager.GetCurrentSlot(), Leader = leader, Address = address });
            }

        }
    }
}