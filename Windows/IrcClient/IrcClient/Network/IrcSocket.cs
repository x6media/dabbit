using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DabbitClient.Network
{
    public interface IIRCSocket
    {
        /// <summary>
        /// Send a message to the IRC socket
        /// </summary>
        /// <param name="message"></param>
        void Send(string message);

        /// <summary>
        /// This delegate is called when a message is received. Set the delegate here.
        /// </summary>
        IrcSocketMessage Receive { set; }

        /// <summary>
        /// Connect to the Network
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from the Network
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Is the socket connected to the network
        /// </summary>
        bool Connected { get; }
    }

    /// <summary>
    /// A delegate called on an ASYNC socket message received call
    /// </summary>
    /// <param name="socketMessage">The string received from the socket call</param>
    public delegate void IrcSocketMessage(string socketMessage);

    class IrcSocket
    {
    }
}
