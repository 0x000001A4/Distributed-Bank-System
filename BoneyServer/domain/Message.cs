using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {

	public class Message {

		private uint requestId;
		private CompareAndSwapReq? compareAndSwapRequest = null;
		private PrepareReq? prepareRequest = null;
		private AcceptReq? acceptRequest = null;

        public Message(CompareAndSwapReq _request, uint _requestId) {
			compareAndSwapRequest = _request;
            requestId = _requestId;
		}
        public Message(PrepareReq _request, uint _requestId)
        {
            prepareRequest = _request;
            requestId = _requestId;
        }
        public Message(AcceptReq _request, uint _requestId)
        {
            acceptRequest = _request;
            requestId = _requestId;
        }

        public uint getRequestId() {
            return requestId;
        }

        public CompareAndSwapReq? getCompareAndSwapRequest() {
			return compareAndSwapRequest;
		}

        public PrepareReq? getPrepareRequest() {
            return prepareRequest;
        }

        public AcceptReq? getAcceptRequest() {
            return acceptRequest;
        }
    }
}
