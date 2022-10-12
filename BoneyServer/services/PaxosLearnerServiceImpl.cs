using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.services
{
    internal class PaxosLearnerServiceImpl : PaxosLearnerService.PaxosLearnerServiceBase {

        public override Task<LearnCommandResp> LearnCommand(LearnCommandReq request, ServerCallContext context)
        {
            LearnCommandReq learnCmd = request;
            return Task.FromResult(new LearnCommandResp());
        }

    }
}
