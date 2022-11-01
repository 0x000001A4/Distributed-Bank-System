﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.domain
{
    public interface IUpdatable
    {
        void Update(uint tick);
        void Stop();
    }
}
