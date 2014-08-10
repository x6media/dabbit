using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dabbit.Base;

using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace dabbit.Win
{
    public class WinContext : IContext, INotifyPropertyChanged
    {
        
        private User[] activeUserList;
        public User[] ActiveUserList
        {
            get
            {
                return this.activeUserList;
            }
            set
            {
                this.activeUserList = value;
                this.users.InvokeIfRequired(() =>
                {
                    this.users.ItemsSource = this.activeUserList;
                    NotifyPropertyChanged();
                });

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetUserList(IWindow sender, User[] users)
        {
            if (this.openItems[0] == sender)
                this.ActiveUserList = users;
        }

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public WinContext(Panel sp, TreeView chanlist, ListBox userList)
        {
            this.Servers = new List<Server>();
            this.Settings = new Dictionary<string, string>();
            this.sp = sp;
            this.tree = chanlist;
            this.users = userList;
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

                lock (this.openItems)
                {
                    foreach (IWindow win in this.openItems)
                    {
                        win.ScrollToEnd();

                        if (win != svr)
                            win.SwitchAway(true);
                    }
                }

                //this.openItems.Clear();

                ((GuiServer)svr).SwitchTo();
                ((GuiServer)svr).AddLine(LineTypes.Info, new User() { Nick = "debug", Host = "localhost" }, "Debug for: " + svr.Connection.Host);
                this.openItems.Add(((GuiServer)svr));
                return true;
            }

            channel = channel.TrimStart();

            Channel chan;
            var chans = svr.Channels.Keys;

            //bool found = false;

            //foreach(string itm in chans)
            //{
            //    if (itm == channel)
            //        found = true;
            //}

            svr.Channels.TryGetValue(channel, out chan);


            lock (this.openItems)
            {
                foreach (IWindow win in this.openItems)
                {
                    if (win != chan)
                        win.SwitchAway(true);
                }
            }

            //this.openItems.Clear();

            ((GuiChannel)chan).SwitchTo();

            this.openItems.Add((GuiChannel)chan);
            ((GuiChannel)chan).AddLine(LineTypes.Info, new User() { Nick = "debug", Host = "localhost" }, "Debug for: " + channel);

            return true;
            

        }
        public bool AppendView(Server svr, string channel)
        {
            if (svr == null)
            {
                throw new ArgumentNullException("svr");
            }
            
            if (String.IsNullOrEmpty(channel))
            {
                ((GuiServer)svr).SwitchTo();
                return true;
            }

            channel = channel.TrimStart();

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

            
            TreeViewItem tvi = TreeviewHelper.CreateTreeViewItem(con.Host + ":" + con.Port.ToString(), TreeviewHelper.IconTypes.Offline);
            this.tree.Items.Add(tvi);
            ((GuiServer)svr).TreeItem = tvi;
            tvi.ExpandSubtree();

            string tmp = ((object)this.openItems).GetHashCode().ToString();
            
            this.SwitchView(svr, null);
            ((GuiServer)svr).jsb.OnOverlayHide += WinContext_OnOverlayHide;
            ((GuiServer)svr).jsb.OnOverlayVisible += WinContext_OnOverlayVisible;
            this.Servers.Add(svr);
 

            Thread thrd = new Thread(svr.Connection.ReadAsync);
            thrd.SetApartmentState(ApartmentState.STA);
            thrd.IsBackground = true;
            thrd.Start();
            this.socketThreads.Add(thrd);

        }

        void WinContext_OnOverlayVisible(object sender)
        {
            WebBrowser sndr = sender as WebBrowser;
            if (!sndr.IsLoaded)
            {
                return;
            }

            sndr.InvokeScript("scrollToEnd", new object[] { });
        }

        void WinContext_OnOverlayHide(object sender)
        {            
            int senderIndex = 0;

            for(int i = 0; i < this.openItems.Count; i++)
            {
                if (this.openItems[i].Window != sender)
                {
                    this.openItems[i].SwitchAway();
                }
                else
                {
                    senderIndex = i;

                }
            }

            GuiChannel client = this.openItems[senderIndex] as GuiChannel;
            if (client == null)
            {
                ((StackPanel)this.users.Parent).Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (((StackPanel)this.users.Parent).Visibility == System.Windows.Visibility.Collapsed)
                    ((StackPanel)this.users.Parent).Visibility = System.Windows.Visibility.Visible;
                //this.users.Items.Clear();

                /*foreach (User user in client.Users)
                {
                    this.users.Items.Add(user.Display);
                }*/
                this.ActiveUserList = client.Users.ToArray();
                ((TextBlock)((StackPanel)this.users.Parent).Children[0]).Text = "Users " + client.Users.Count();
            }

            if (this.openItems.Count > 0)
            {
                var temp = this.openItems[0];
                this.openItems[0] = this.openItems[senderIndex];
                this.openItems[senderIndex] = temp;
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
            
            this.sp.InvokeIfRequired(() =>
            {
                guichan = new GuiChannel(svr);

                guichan.jsb.OnOverlayVisible += WinContext_OnOverlayVisible;
                guichan.jsb.OnOverlayHide += WinContext_OnOverlayHide;

                this.sp.Children.Add(guichan.Window);

                TreeViewItem tvi = TreeviewHelper.CreateTreeViewItem("#", TreeviewHelper.IconTypes.None);
                ((GuiServer)svr).TreeItem.Items.Add(tvi);
                guichan.TreeItem = tvi;

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

        /// <summary>
        /// Called by the user when they are sending a message or command.
        /// </summary>
        /// <param name="message">Input taken from the user</param>
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
        private TreeView tree;
        private ListBox users;
        private List<System.Threading.Thread> socketThreads = new List<System.Threading.Thread>();
        internal List<IWindow> openItems = new List<IWindow>();
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

            if (this.secure)
            {
                this.sslStream = new System.Net.Security.SslStream(this.sock.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate));
                this.sslStream.AuthenticateAsClient(this.Host);

                this.streamRead = new StreamReader(this.sslStream);
                this.streamWriter = new StreamWriter(this.sslStream);
            }
            else
            {
                this.streamRead = new StreamReader(this.sock.GetStream());
                this.streamWriter = new StreamWriter(this.sock.GetStream());
            }
            this.streamWriter.AutoFlush = true;

            return new Task(new Action(nope));
        }
        private bool ValidateServerCertificate(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
        {
            return true;
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

        //private NetworkStream ns;
        private System.Net.Security.SslStream sslStream;
        private StreamReader streamRead;
        private StreamWriter streamWriter;
    }
}
