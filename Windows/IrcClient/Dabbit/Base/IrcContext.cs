using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Irc;
using Dabbit.Network;

namespace Dabbit.Base
{
    public interface IIrcContext
    {
        IIrcSocket CreateSocket();
    }

    public class IrcContext : IIrcContext
    {
        public IIrcSocket CreateSocket()
        {
            return new IrcSocket();
        }
    }
}
