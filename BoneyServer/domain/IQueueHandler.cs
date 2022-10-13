using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    public interface IQueueHandler
    {
        void doCompareAndSwap();
        void doPrepare();
        void doAccept();
        void doLearnCommand();
    }
}
