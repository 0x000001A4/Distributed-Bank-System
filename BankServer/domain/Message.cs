using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.domain {

	public class Message {

		private uint requestId;
        private string sender;
		private CompareAndSwapResp? compareAndSwapResp = null;

        public Message(CompareAndSwapResp _request, string _sender) {
			compareAndSwapResp = _request;
            sender = _sender;
            requestId = 1;
		}

        public uint GetRequestId() {
            return requestId;
        }

        public CompareAndSwapResp? GetCompareAndSwapResponse() {
			return compareAndSwapResp;
		}

        public string GetSender() {
            return sender;
        }
    }
}
