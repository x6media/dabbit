using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;
using Dabbit.Irc;

namespace Dabbit.Network
{
    public class IrcConnection
    {
        public IrcConnection(string host, uint port, bool secure)
        {
            if (String.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(host);
            }

            if (connectionId == null)
            {
                throw new ArgumentNullException("connectionId");
            }

            this.host = host;
            this.port = port;
            this.secure = secure;

            this.socket = IrcCentral.Instance().Context.CreateSocket();
            this.socket.Hostname = this.Hostname;
            this.socket.Port = (int)this.Port;
            this.socket.SSL = this.IsSecure;
        }

        public string Hostname
        {
            get 
            { 
                return this.host; 
            }
        }

        public uint Port
        {
            get
            { 
                return this.port; 
            }
        }

        public bool IsSecure
        {
            get
            {
                return this.secure;
            }
        }

        public Guid ConnectionId
        {
            get
            {
                return this.connectionId;
            }
        }

        public IIrcSocket Socket
        {
            get
            {
                return this.socket;
            }
        }

        private string host = String.Empty;
        private uint port = 6667;
        private bool secure = false;
        private Guid connectionId = Guid.NewGuid();
        private IIrcSocket socket = null;
    }
}
