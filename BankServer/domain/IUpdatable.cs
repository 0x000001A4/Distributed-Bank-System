using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.domain
{
    public interface IUpdatable
    {
        void Update(uint tick);
        void Stop();
    }
}
