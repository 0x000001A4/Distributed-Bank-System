using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {

	public class Message {

		private uint requestId;
        private string sender;
		private CompareAndSwapReq? compareAndSwapRequest = null;
		private PrepareReq? prepareRequest = null;
		private AcceptReq? acceptRequest = null;
        private LearnCommandReq? learnCommandRequest = null;

        public Message(CompareAndSwapReq _request, string _sender) {
			compareAndSwapRequest = _request;
            sender = _sender;
            requestId = 1;
		}
        public Message(PrepareReq _request, string _sender)
        {
            prepareRequest = _request;
            sender = _sender;
            requestId = 2;
        }
        public Message(AcceptReq _request, string _sender)
        {
            acceptRequest = _request;
            sender = _sender;
            requestId = 3;
        }
        public Message(LearnCommandReq _request, string _sender)
        {
            learnCommandRequest = _request;
            sender = _sender;
            requestId = 4;
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
