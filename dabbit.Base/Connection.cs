using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dabbit.Base
{


    internal delegate void RawMessageReceived(Message msg);

    /// <summary>
    /// An exception thrown when a port number < 1 or > 65000 is passed
    /// </summary>
    public class InvalidPortException : Exception
    {
        public int Port;

        public InvalidPortException(int port)
            : base("Invalid port: " + port)
        {
            this.Port = port;
        }
    }

    public class AlreadyConnectedException : Exception
    {
        public AlreadyConnectedException()
            : base("Cannot connect to the server when there is already a connection")
        { }
    }

    public /*abstract*/ class Connection
    {
        public string Host { get { return this.socketWrapper.Host; } }
        public int Port { get { return this.socketWrapper.Port; } }
        public bool Secure { get { return this.socketWrapper.Secure; } }
        public Guid Id { get; internal set; }
        public bool Async { get; protected set; }
        public bool Connected { get { return this.socketWrapper.Connected; } }
        internal RawMessageReceived RawMessageReceived { get;  set; }

        public Connection(IContext ctx, ISocketWrapper socket)
        {
            /*
            if (String.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host");
            }

            if (port <= 0 || port > 65000)
            {
                throw new InvalidPortException(port);
            }*/
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }
            this.ctx = ctx;
            this.socketWrapper = socket;
        }


        public void Connect()
        {
            if (this.Connected)
            {
                throw new AlreadyConnectedException();
            }

            this.socketWrapper.ConnectAsync();
        }

        public void Disconnect()
        {
            this.socketWrapper.Disconnect();

            this.socketWrapper = this.ctx.CreateSocket(this.socketWrapper.Host, this.socketWrapper.Port, this.socketWrapper.Secure);
        }

        // I don't remember why we needed the reader and writer now...
        // TODO Not needed anymore?
        /*
        public StreamReader Reader 
        {
            get
            {
                return this.socketWrapper.Reader;
            }
        }
        
        public StreamWriter Writer 
        {
            get
            {
                return this.socketWrapper.Writer;
            }
        }
        */

        /// <summary>
        /// This is called to return a single message from the stack. By default this doesn't run in
        /// async, but it can if you call ReadAsync first. 
        /// </summary>
        /// <returns>Message object if there is a message available. Null if not (and running async)</returns>
        private Message Read()
        {
            if (this.Async)
            {
                if (this.messages.Count > 0)
                    return this.messages.Pop();
                else
                    return null;
            } 
            else
            {
                return this.ReadDirect();
            }
        }

        /// <summary>
        /// Calling this method in the same thread as your application WILL lock it up. Use this method for 
        /// doing a background reader and checker. If you set the RawMessageReceived delegate,
        /// you can get a callback for every time a raw message is received.
        /// </summary>
        private void ReadAsync()
        {
            this.Async = true;

            // Callback based 
            if (this.RawMessageReceived == null)
            {
                while (this.Connected && this.RawMessageReceived == null)
                {
                    this.messages.Push(this.ReadDirect());
                }
            }

            // If we're transitioning to using a callback, make sure to empty the stack so we don't miss
            // any messages
            if (this.RawMessageReceived != null)
            {
                // empty stack
                while (this.messages.Count > 0)
                {
                    this.RawMessageReceived(this.messages.Pop());
                }

                // resume async reading
                while (this.Connected)
                {
                    this.RawMessageReceived(this.ReadDirect());
                }
            }

            this.Async = false;
        }

        private Message ReadDirect()
        {
            Message msg = new Message();

            msg.RawLine = this.socketWrapper.Reader.ReadLine();
            string[] messages = msg.RawLine.Split(' ');


            if (messages[0] == "PING" || messages[0] == "ERROR")
            {
                msg.Command = messages[0];
            }
            else
            {
                msg.Command = messages[1];
            }
            
            
            msg.MessageLine = String.Join(" ", messages.Where(p => p[0] == ':' && p != messages[0]).ToArray()).Substring(1);
            
            string[] fromParts = messages[0].Split('!');
            if (fromParts.Count() > 1)
            {
                string[] identHost = fromParts[1].Split('@');
                msg.From = new From(new string[] { fromParts[0].Substring(1), identHost[0], identHost[1] }, FromType.Client);
            }
            else
            {
                msg.From = new From(new string[] { fromParts[0].Substring(1) }, FromType.Server);
            }

            return msg;

        }

        public void Write(string message)
        {
            this.socketWrapper.Writer.WriteLineAsync(message);
        }

        private Stack<Message> messages = new Stack<Message>();
        private IContext ctx;
        private ISocketWrapper socketWrapper;
    }

    public enum ConnectionType
    {
        /// <summary>
        /// Direct TCP/IP connection
        /// </summary>
        Direct,

        /// <summary>
        /// A connection to a BNC server
        /// </summary>
        BNC,

        /// <summary>
        /// Connection to a dabbit Socket Server
        /// </summary>
        Dabbit,

        /// <summary>
        /// Sock 5 server
        /// </summary>
        Socks5,

        /// <summary>
        /// Sock 4 server
        /// </summary>
        Socks4
    }
}
