using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;

namespace Dabbit.Network
{
    public class IrcNetwork : IrcEntity
    {
        public IrcNetwork(string networkName, IrcConnection connection) : base(networkName)
        {

        }
    }
}
