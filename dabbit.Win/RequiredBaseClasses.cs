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
using System.Windows.Controls;

namespace dabbit.Win
{
    public class WinContext : IContext
    {
        public WinContext(Panel sp)
        {
            this.Servers = new List<Server>();
            this.Settings = new Dictionary<string, string>();
            this.sp = sp;
        }
        public Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket)
        {
            if (connectionType == ConnectionType.Direct)
            {
                return new WinDirectConnection(this, socket);
            }
            else
                return null;
        }

        public bool SwitchView(Server svr, string channel)
        {
            if (svr == null)
            {
                throw new ArgumentNullException("svr");
            }



            if (String.IsNullOrEmpty(channel))
            {
                foreach (IWindow win in this.openItems)
                {

                    if (win != svr)
                        win.SwitchAway(true);
                }

                this.openItems.Clear();

                ((GuiServer)svr).SwitchTo();
                this.openItems.Add(((GuiServer)svr));
                return true;
            }


            Channel chan;
            svr.Channels.TryGetValue(channel, out chan);


            foreach (IWindow win in this.openItems)
            {

                if (win != chan)
                    win.SwitchAway(true);
            }

            this.openItems.Clear();

            ((GuiChannel)chan).SwitchTo();

            this.openItems.Add((GuiChannel)chan);

            return true;
            

        }
        public bool AppendView(Server svr, string channel)
        {
            if (svr == null)
            {
                throw new ArgumentNullException("svr");
            }


            foreach (IWindow win in this.openItems)
            {
                if (win != svr)
                    win.SwitchAway(true);
            }

            this.openItems.Clear();

            if (String.IsNullOrEmpty(channel))
            {
                ((GuiServer)svr).SwitchTo();
                return true;
            }


            Channel chan;
            svr.Channels.TryGetValue(channel, out chan);

            ((GuiChannel)chan).SwitchTo();

            this.openItems.Add((GuiChannel)chan);

            return true;
        }

        public void AddServer(IContext ctx, User me, Connection con)
        {

            Server svr = new GuiServer(this, me, con);

            this.sp.Children.Add(((GuiServer)svr).Window); 
            
            string tmp = ((object)this.openItems).GetHashCode().ToString();

            /*
            if (this.openItems.Count == 0)
            {
                this.openItems.Add((GuiServer)svr);
            }
            else
            {
                IWindow temp = this.openItems[0];
                this.openItems[0] = (GuiServer)svr;
                this.openItems.Add(temp);
            }
            */

            this.SwitchView(svr, null);
            ((GuiServer)svr).jsb.OnOverlayHide += WinContext_OnOverlayHide;
            ((GuiServer)svr).jsb.OnOverlayVisible += WinContext_OnOverlayVisible;
            this.Servers.Add(svr);
 

            Thread thrd = new Thread(svr.Connection.ReadAsync);
            thrd.SetApartmentState(ApartmentState.STA);
            thrd.IsBackground = true;
            thrd.Start();
            this.socketThreads.Add(thrd);

            svr.Connection.Write("JOIN #dab");
        }

        void WinContext_OnOverlayVisible(object sender)
        {
        }

        void WinContext_OnOverlayHide(object sender)
        {

            foreach (IWindow webbr in this.openItems)
            {
                if (webbr.Window != sender)
                {
                    webbr.SwitchAway();
                }
            }
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

        public Channel CreateChannel(Server svr)
        {
            GuiChannel guichan = null;

            string tmp = ((object)this.openItems).GetHashCode().ToString();

            this.sp.InvokeIfRequired(() =>
            {
                guichan = new GuiChannel(svr);

                guichan.jsb.OnOverlayVisible += WinContext_OnOverlayVisible;
                guichan.jsb.OnOverlayHide += WinContext_OnOverlayHide;

                this.sp.Children.Add(guichan.Window);

                
                /*
                if (this.openItems.Count == 0)
                {
                    this.openItems.Add(guichan);
                }
                else
                {
                    IWindow temp = this.openItems[0];
                    this.openItems[0] = guichan;
                    this.openItems.Add(temp);
                }*/
            });


            return guichan;

        }

        public void FormClosing()
        {
            foreach(Server svr in this.Servers)
            {
                svr.Connection.Disconnect();
            }

            foreach(Thread tmp in this.socketThreads)
            {
                tmp.Abort();
            }
        }

        public void SendToActivewindow(string message)
        {

            GuiChannel trychan = this.openItems[0] as GuiChannel;

            Connection sending = null;

            if (trychan == null)
            {
                // sending message to server
                if (message[0] == '/')
                {
                    message = message.Substring(1);
                }
                sending = ((GuiServer)this.openItems[0]).Connection;
            }
            else
            {
                // sending message to channel
                if (message[0] == '/')
                {
                    message = message.Substring(1);
                    string[] commands = message.Split(' ');
                    switch(commands[0])
                    {
                        case "msg":
                            if (commands.Count() < 3)
                            {
                                trychan.AddLine(LineTypes.Info, new User() { Nick = "", Host = "" }, "Invalid usage of /msg. Usage: /msg #ChanOrUser Content");
                                return;
                            }

                            message = "PRIVMSG " + commands[1] + " :" + String.Join(" ", commands.Skip(2).ToArray());
                            break;
                        case "me":
                            trychan.AddLine(LineTypes.Action, trychan.Users.Where(u => u.Nick == trychan.ServerOf.Me.Nick).First(), "Invalid usage of /msg. Usage: /msg #ChanOrUser Content");
                            message = "PRIVMSG " + commands[1] + " :\001ACTION" + String.Join(" ", commands.Skip(1).ToArray()) + "\001";
                            break;
                        case "idk":

                            break;
                    }
                }
                else
                {
                    trychan.AddLine(LineTypes.Normal, trychan.Users.Where(u => u.Nick == trychan.ServerOf.Me.Nick).First(), message);
                    message = "PRIVMSG " + trychan.Name + " :" + message;
                }
                
                sending = trychan.ServerOf.Connection;
            }

            sending.Write(message);
        }

        private Panel sp;
        private List<System.Threading.Thread> socketThreads = new List<System.Threading.Thread>();
        private List<IWindow> openItems = new List<IWindow>();
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
        public string Host { get { return this.host; } }
        public int Port { get { return this.port; } }
        public bool Secure { get { return this.secure; } }

        public bool Connected { get { return this.sock.Connected; } }

        public Task ConnectAsync()
        {
            this.sock.Connect(this.Host, this.Port);
            this.streamRead = new StreamReader(this.sock.GetStream());
            this.streamWriter = new StreamWriter(this.sock.GetStream());

            this.streamWriter.AutoFlush = true;

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
            this.host = host;
            this.port = port;
        }

        private TcpClient sock;
        private string host;
        private int port;
        private bool secure = false;

        private NetworkStream ns;
        private StreamReader streamRead;
        private StreamWriter streamWriter;
    }
}
