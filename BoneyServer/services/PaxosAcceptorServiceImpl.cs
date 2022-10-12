using BoneyServer.domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
	internal class PaxosAcceptorServiceImpl : PaxosAcceptorService.PaxosAcceptorServiceBase
	{
		public override Task<PromiseResp> Prepare(PrepareReq request, ServerCallContext context)
		{
			return Task.FromResult(new PromiseResp(/* Send promise information */));
		}

		public override Task<AcceptedResp> Accept(AcceptReq request, ServerCallContext context)
		{
			Acceptor.LearnWork(request);
			return Task.FromResult(new AcceptedResp(/* Send accepted information */));
		}
	}
}
