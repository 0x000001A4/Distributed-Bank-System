using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {

	public class Message {

		private uint requestId;
		private CompareAndSwapRequest? compareAndSwapRequest = null;
		private PrepareRequest? prepareRequest = null;
		private AcceptRequest? acceptRequest = null;

        public Message(CompareAndSwapRequest _request, uint _requestId) {
			compareAndSwapRequest = _request;
            requestId = _requestId;
		}
        public Message(PrepareRequest _request, uint _requestId)
        {
            prepareRequest = _request;
            requestId = _requestId;
        }
        public Message(AcceptRequest _request, uint _requestId)
        {
            acceptRequest = _request;
            requestId = _requestId;
        }

        public uint getRequestId() {
            return requestId;
        }

        public CompareAndSwapRequest? getCompareAndSwapRequest() {
			return compareAndSwapRequest;
		}

        public PrepareRequest? getPrepareRequest() {
            return prepareRequest;
        }

        public AcceptRequest? getAcceptRequest() {
            return acceptRequest;
        }
    }
}
