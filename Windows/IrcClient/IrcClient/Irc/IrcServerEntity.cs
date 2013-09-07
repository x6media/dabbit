using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DabbitClient.Base;

namespace DabbitClient.Irc
{
    class IrcServerEntity : IrcFromEntity
    {
        public IrcServerEntity(string serverName) : base(serverName)
        {
        }
    }
}
