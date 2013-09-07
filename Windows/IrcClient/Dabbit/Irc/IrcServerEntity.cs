using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;

namespace Dabbit.Irc
{
    class IrcServerEntity : IrcFromEntity
    {
        public IrcServerEntity(string serverName)
            : base(typeof(IrcServerEntity), serverName)
        {
        }
    }
}
