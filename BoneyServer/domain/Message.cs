using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain {

	internal class Message<TRequest> {

		private TRequest request;

		public Message(TRequest _request) {
			this.request = _request;
		}

		public TRequest getRequest() {
			return request;
		}
	}
}
