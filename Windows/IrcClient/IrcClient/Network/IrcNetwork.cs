using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DabbitClient.Base;

namespace DabbitClient.Network
{
    public class IrcNetwork : IrcEntity
    {
        IrcNetwork(string networkName, IrcConnection connection) : base(networkName)
        {

        }
    }
}
