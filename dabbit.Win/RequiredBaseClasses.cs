using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dabbit.Base;

using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
namespace dabbit.Win
{
    public class WinContext : IContext
    {
        public Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket)
        {
            if (connectionType == ConnectionType.Direct)
            {
                return new WinDirectConnection(this, socket);
            }
            else
                return null;
        }

        public List<Server> Servers { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Creates a blank TCP Socket for use in a connection
        /// </summary>
        /// <param name="host">The host you want to connect to.</param>
        /// <param name="port">port</param>
        /// <param name="secure">bool is secure</param>
        /// <returns>A socket wrapper for a connection. This is platform dependant. This will not be in the same namespace</returns>
        public ISocketWrapper CreateSocket(string host, int port, bool secure)
        {
            return new WinSocket(host, port, secure);
        }
    }


    public class WinDirectConnection : Connection
    {
        public WinDirectConnection(IContext ctx, ISocketWrapper socket)
            : base(ctx, socket)
        {
        }
    }

    public class WinSocket : ISocketWrapper
    {
        // Request these attributes in the Constructor
        public string Host { get { return this.ip.Address.ToString(); } }
        public int Port { get { return this.ip.Port; } }
        public bool Secure { get { return this.secure; } }
        private bool secure = false;

        public bool Connected { get { return this.sock.Connected; } }

        public Task ConnectAsync()
        {
            this.sock.Connect(ip);
            this.streamRead = new StreamReader(this.sock.GetStream());
            this.streamWriter = new StreamWriter(this.sock.GetStream());
            this.streamWriter.WriteLine("HELLO");
            this.streamWriter.Flush();
            Console.WriteLine(this.streamRead.ReadLine());

            //this.streamWriter.AutoFlush = true;

            return new Task(new Action(nope));
        }

        public void nope()
        { }

        public void Disconnect()
        {
            this.sock.Close();
            this.sock = new TcpClient();
            this.sock.Connect(this.Host, this.Port);
        }

        public StreamReader Reader { get { return this.streamRead; } }

        public StreamWriter Writer { get { return this.streamWriter; } }

        public WinSocket(string host, int port, bool secure)
        {
            this.secure = secure;
            this.sock = new TcpClient();
            this.ip = new IPEndPoint(IPAddress.Parse(host), port);
        }

        private TcpClient sock;
        private IPEndPoint ip;
        private NetworkStream ns;
        private StreamReader streamRead;
        private StreamWriter streamWriter;
    }
}
