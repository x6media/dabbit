using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dabbit.Base
{
    public interface IContext
    {
        Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket);

        List<Server> Servers { get; set; }

        Dictionary<string, string> Settings { get; set; }
        
        /// <summary>
        /// Creates a blank TCP Socket for use in a connection
        /// </summary>
        /// <param name="host">The host you want to connect to.</param>
        /// <param name="port">port</param>
        /// <param name="secure">bool is secure</param>
        /// <returns>A socket wrapper for a connection. This is platform dependant. This will not be in the same namespace</returns>
        ISocketWrapper CreateSocket(string host, int port, bool secure);
    }

    // TODO: DELETE THIS!!
    public class DabbitContext : IContext
    {
        public Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket)
        {
            return new Connection(this, socket);
        }

        public List<Server> Servers { get; set; }
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Create an empty blank TCP socket.
        /// </summary>
        /// <returns>Blank tcp socket capable of returning read/write streams and Async Connect.</returns>
        public ISocketWrapper CreateSocket(string host, int port, bool secure)
        {
            return (ISocketWrapper)new object();
        }
    }

}
