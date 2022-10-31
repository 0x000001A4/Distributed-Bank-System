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
        private DepositReq? depositReq = null;
        private WithdrawReq? withdrawReq = null;
        private ReadReq? readReq = null;

        public Message(CompareAndSwapResp _request, string _sender) {
			compareAndSwapResp = _request;
            sender = _sender;
            requestId = 1;
		}

        public Message(DepositReq _request, string _sender)
        {
            depositReq = _request;
            sender = _sender;
            requestId = 2;
        }

        public Message(WithdrawReq _request, string _sender)
        {
            withdrawReq = _request;
            sender = _sender;
            requestId = 3;
        }

        public Message(ReadReq _request, string _sender)
        {
            readReq = _request;
            sender = _sender;
            requestId = 4;
        }

        public uint GetRequestId() {
            return requestId;
        }

        public CompareAndSwapResp? GetCompareAndSwapResponse() {
			return compareAndSwapResp;
		}

        public DepositReq? GetDepositReq()
        {
            return depositReq;
        }

        public WithdrawReq? GetWithdrawReq()
        {
            return withdrawReq;
        }
        public ReadReq? GetReadReq()
        {
            return readReq;
        }


        public string GetSender() {
            return sender;
        }
    }
}
