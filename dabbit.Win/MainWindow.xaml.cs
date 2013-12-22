using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using dabbit.Base;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace dabbit.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Connection cn = new Connection(new DabbitContext(), "localhost", 667, false);
        Thread thrd;
        WinDabbitContext ctx = new WinDabbitContext();
        Server svr;
        public MainWindow()
        {
            InitializeComponent();

            svr = new Server(ctx, new User() { Nick = "dabb", Ident = "dabitp", Name = "David" }, new Connection(ctx, new WinSocket("127.0.0.1", 6667, false)));

            svr.OnRawMessage += svr_OnRawMessage;
           //svr.Connection.Connect();
            thrd = new Thread(svr.Connection.ReadAsync);

            thrd.Start();
            
        }

        void svr_OnRawMessage(object sender, Message e)
        {
            this.Dispatcher.Invoke(new Action(delegate()
            {
                output.Text += e.RawLine + Environment.NewLine;
            }));
            
        }
    }
    public class WinDabbitContext : IContext
    {
        public Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket)
        {
            return new DirectConnection(this, socket);
        }

        public List<Server> Servers { get; set; }
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Create an empty blank TCP socket.
        /// </summary>
        /// <returns>Blank tcp socket capable of returning read/write streams and Async Connect.</returns>
        public ISocketWrapper CreateSocket(string host, int port, bool secure)
        {
            return new WinSocket(host, port, secure);
        }
    }

    public class DirectConnection : Connection
    { 
        public DirectConnection(IContext ctx, ISocketWrapper sckt)
            : base(ctx, sckt)
        {
        }

        public new void Write(string message)
        {
            Console.WriteLine(message);
            base.Write(message);
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
