using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {

	public class Message {

		private uint requestId;
        private ServerCallContext context;
		private CompareAndSwapReq? compareAndSwapRequest = null;
		private PrepareReq? prepareRequest = null;
		private AcceptReq? acceptRequest = null;
        private LearnCommandReq? learnCommandRequest = null;

        public Message(CompareAndSwapReq _request, ServerCallContext _context) {
			compareAndSwapRequest = _request;
            context = _context;
            requestId = 1;
		}
        public Message(PrepareReq _request, ServerCallContext _context)
        {
            prepareRequest = _request;
            context = _context;
            requestId = 2;
        }
        public Message(AcceptReq _request, ServerCallContext _context)
        {
            acceptRequest = _request;
            context = _context;
            requestId = 3;
        }
        public Message(LearnCommandReq _request, ServerCallContext _context)
        {
            learnCommandRequest = _request;
            context = _context;
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

        public ServerCallContext GetServerCallContext() {
            return context;
        }
    }
}
