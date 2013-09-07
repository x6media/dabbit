using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;

namespace Dabbit.Irc
{
    class IrcModeChannelEntity : IrcModeEntity
    {
        public IrcModeChannelEntity(string clientDisplay, string protocolDisplay, int value)
            : base(clientDisplay, protocolDisplay, value)
        {
        }

        public IrcModeChannelEntity(string clientDisplay, string protocolDisplay, int value, IrcModeArgumentEntity modeArgument)
            : base(clientDisplay, protocolDisplay, value, modeArgument)
        {
        }
    }
}
