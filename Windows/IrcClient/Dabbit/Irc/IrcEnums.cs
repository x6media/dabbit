using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dabbit.Irc
{

    /// <summary>
    /// enum for the type of message being passed
    /// </summary>
    enum IrcMessageType
    {
        Private,
        Channel,
        Status
    }

    /// <summary>
    /// If you catch the generic event, this is passed to determine what event you're catching.
    /// </summary>
    enum IrcEventTypes
    {
        Welcome,
        Motd,
        Message,
        Notice
    }
}
