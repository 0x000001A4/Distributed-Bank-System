using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {

	public class Message {
        public static readonly uint COMPARE_AND_SWAP = 1;
        public static readonly uint PREPARE = 2;
        public static readonly uint ACCEPT= 3;
        public static readonly uint LEARN_COMMAND = 4;

        private uint requestId;
        private string sender;
		private CompareAndSwapReq? compareAndSwapRequest = null;
		private PrepareReq? prepareRequest = null;
		private AcceptReq? acceptRequest = null;
        private LearnCommandReq? learnCommandRequest = null;

        public Message(CompareAndSwapReq _request, string _sender) {
			compareAndSwapRequest = _request;
            sender = _sender;
            requestId = COMPARE_AND_SWAP;
		}
        public Message(PrepareReq _request, string _sender)
        {
            prepareRequest = _request;
            sender = _sender;
            requestId = PREPARE;
        }
        public Message(AcceptReq _request, string _sender)
        {
            acceptRequest = _request;
            sender = _sender;
            requestId = ACCEPT;
        }
        public Message(LearnCommandReq _request, string _sender)
        {
            learnCommandRequest = _request;
            sender = _sender;
            requestId = LEARN_COMMAND;
        }

        public uint GetRequestId() {
            return requestId;
        }

        public CompareAndSwapReq? GetCompareAndSwapRequest() {
			return compareAndSwapRequest;
		}

        public PrepareReq? GetPrepareRequest() {
            return prepareRequest;
        }

        public AcceptReq? GetAcceptRequest() {
            return acceptRequest;
        }
    
        public LearnCommandReq? GetLearnCommandRequest() {
            return learnCommandRequest;
        }

        public string GetSender() {
            return sender;
        }
    }
}
