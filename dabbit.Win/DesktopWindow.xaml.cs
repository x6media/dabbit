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
using System.Windows.Shapes;
using dabbit.Base;
using System.Collections.ObjectModel;
using System.IO;

namespace dabbit.Win
{
    /// <summary>
    /// Interaction logic for DesktopWindow.xaml
    /// </summary>
    /// 

    public partial class DesktopWindow : Window
    {
        public DesktopWindow()
        {
            InitializeComponent();

            DataContext = DesktopViewModel.Context;

            DesktopViewModel.Context.Display = "Hello World";
            
            //WinContext ctx = new WinContext(null, treeViewServerList, null);
            //this.srvrs = new List<Server>();

            DesktopViewModel.Context.Servers.Add(new GuiSvr(DesktopViewModel.Context, new User() { Nick = "dab" }, new Connection(DesktopViewModel.Context, new StringTestSocket(new MemoryStream(), new MemoryStream()))));
            DesktopViewModel.Context.Servers.Add(new GuiSvr(DesktopViewModel.Context, new User() { Nick = "dab" }, new Connection(DesktopViewModel.Context, new StringTestSocket(new MemoryStream(), new MemoryStream()))));
            DesktopViewModel.Context.Servers.Add(new GuiSvr(DesktopViewModel.Context, new User() { Nick = "dab" }, new Connection(DesktopViewModel.Context, new StringTestSocket(new MemoryStream(), new MemoryStream()))));

            DesktopViewModel.Context.Servers[0].Channels.Add("#dab", new Channel(DesktopViewModel.Context.Servers[0]) { Name = "#dabName" });
            DesktopViewModel.Context.Windows.Add(new GuiChnl() { Name = "#dab", Topic = new Topic() { Display = "\x03" + "4Th" + "\x03" + "6is is \x16http://dab.biz/ \x02\x09\x13\x15\x1f my webs\x0fite" } });

            ((GuiChnl)DesktopViewModel.Context.Windows[0]).Users.Add(new GuiUser() { Nick = "dab", Modes = new List<string>() { "q", "a", "o" } });
            ((GuiChnl)DesktopViewModel.Context.Windows[0]).Users.Add(new GuiUser() { Nick = "dab2", Modes = new List<string>() { "a", "o" } });
            ((GuiChnl)DesktopViewModel.Context.Windows[0]).Users.Add(new GuiUser() { Nick = "dab3", Modes = new List<string>() { "o" } });

            using (var reader = new StreamReader(@"C:\Users\me_000\Desktop\ggxy.txt"))
            {
                string line = String.Empty;

                while((line = reader.ReadLine()) != null)
                {
                    DesktopViewModel.Context.Windows[0].WindowBuffer.Inlines.Add((new Run(line + Environment.NewLine)));
                }
            }
            DesktopViewModel.Context.Windows[0].WindowBuffer.Inlines.Add((new Run("Hello World There!!!")));
            

            DesktopViewModel.Context.Servers[0].Channels.Add("#dab.beta", new Channel(DesktopViewModel.Context.Servers[0]) { Name = "#dab.betaName" });
            DesktopViewModel.Context.Servers[0].Channels.Add("#ggxy", new Channel(DesktopViewModel.Context.Servers[0]) { Name = "#ggxyName" });

            DesktopViewModel.Context.Servers[1].Channels.Add("#dab.beta", new Channel(DesktopViewModel.Context.Servers[1]) { Name = "#dab.beta" });
            DesktopViewModel.Context.Servers[1].Channels.Add("#dab", new Channel(DesktopViewModel.Context.Servers[1]) { Name = "#dab" });

            //treeViewServerList.ItemsSource = this.srvrs;
            //MultiSelect.AllowMultiSelection(treeViewServerList, this.OnSelectedItemsChanged);

        }

        void OnSelectedItemsChanged(object sender, System.Collections.Generic.List<TreeViewItem> selectedItems)
        { 

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //((GuiChnl)DesktopViewModel.Context.Windows[0]).Users[0].Nick = ((TextBox)sender).Text;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((GuiChnl)DesktopViewModel.Context.Windows[0]).WindowBuffer.Inlines.Add((new Run(((TextBox)sender).Text + Environment.NewLine)));
            }
        }
    }

    public static class DesktopViewModel
    {
        static DesktopViewModel()
        {
            Context = new GuiContext();
        }

        public static GuiContext Context { get; set; }
    }

    public class GuiContext : IContext, System.ComponentModel.INotifyPropertyChanged
    {
        private string display;
        public GuiContext()
        {
            Windows = new List<IGuiWindow>();
        }
        public string Display
        {
            get { return this.display; }
            set { this.display = value; NotifyPropertyChanged(); }
        }

        public List<Server> Servers
        {
            get { return this.servers; }
            set { this.servers = value; NotifyPropertyChanged(); }
        }

        private List<Server> servers = new List<Server>();

        public List<IGuiWindow> Windows
        {
            get;
            set;
        }


        public Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket)
        {
            return new Connection(this, new StringTestSocket(new MemoryStream(), new MemoryStream()));
        }

        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// REMOVE THIS AND DO PROPER THEMES
        /// </summary>
        public SolidColorBrush TopicFontColor = Brushes.Black;

        public ISocketWrapper CreateSocket(string host, int port, bool secure)
        {
            return new StringTestSocket(new MemoryStream(), new MemoryStream());
        }

        public Channel CreateChannel(Server svr)
        {
            return new GuiChannel(svr);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public User CreateUser()
        {
            return new GuiUser();
        }

        public User CreateUser(SourceEntity source)
        {
            return new GuiUser(source);
        }
    }
    public interface IGuiWindow 
    { 
        bool IsActive { get; set; }

        Paragraph WindowBuffer { get; set; }

    }

    public class GuiChnl : Channel, System.ComponentModel.INotifyPropertyChanged, IGuiWindow
    {
        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                this.isActive = value;
                NotifyPropertyChanged();
            }
        }

        public override Topic Topic
        {
            get
            {
                return base.Topic;
            }
            set
            {
                base.Topic = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("FormattedTopic");
            }
        }

        public override List<User> Users
        {
            get
            {
                return base.Users;
            }
            set
            {
                base.Users = value;
                NotifyPropertyChanged();
            }
        }
        
        public Paragraph FormattedTopic
        {
            get
            {
                return base.Topic.Display.IrcStringToRtf(DesktopViewModel.Context.TopicFontColor);
            }
            set
            {
                garbage = value;
            }
        }
        private Paragraph garbage;
        private bool isActive;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private Paragraph windowBuffer = new Paragraph();

        public Paragraph WindowBuffer
        {
            get { return this.windowBuffer;  }
            set { this.windowBuffer = value; NotifyPropertyChanged(); }
            
        }
    }



    public class GuiUser : User, System.ComponentModel.INotifyPropertyChanged
    {
        public GuiUser()
            : base()
        {

        }

        public GuiUser(SourceEntity source)
            : base(source)
        { }

        public override string Nick
        {
            get
            {
                return base.Nick;
            }
            set
            {
                base.Nick = value;
                NotifyPropertyChanged("Display");
                NotifyPropertyChanged();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class GuiSvr : Server, System.ComponentModel.INotifyPropertyChanged, IGuiWindow
    {
        public bool IsActive 
        { 
            get { return this.isActive; } 
            set
            {
                this.isActive = value;
                NotifyPropertyChanged();
            }
        }
        private bool isActive;

        public GuiSvr(IContext ctx, User me, Connection cnt)
            : base(ctx, me, cnt)
        { }

        public override Dictionary<string, Channel> Channels 
        { 
            get 
            {
                return base.Channels; 
            } 
            
            protected set 
            {
                base.Channels = value; 
                NotifyPropertyChanged();
            } 
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        private Paragraph windowBuffer = new Paragraph();

        public Paragraph WindowBuffer
        {
            get { return this.windowBuffer; }
            set { this.windowBuffer = value; NotifyPropertyChanged(); }

        }
    }

    public class StringTestSocket : ISocketWrapper
    {
        // Request these attributes in the Constructor
        public string Host { get { return "127.0.0.1"; } }
        public int Port { get { return 6667; } }
        public bool Secure { get { return true; } }

        public bool Connected { get { return connected; } }
        private bool connected = false;

        public StringTestSocket(MemoryStream read, MemoryStream write)
        {
            this.streamRead = new StreamReader(read);
            this.streamWriter = new StreamWriter(write);
        }
        public Task ConnectAsync()
        {
            this.connected = true;
            return new Task(new Action(nope));
        }

        public void nope()
        { }

        public void Disconnect()
        {
        }

        public StreamReader Reader { get { return this.streamRead; } }

        public StreamWriter Writer { get { return this.streamWriter; } }

        private StreamReader streamRead;
        private StreamWriter streamWriter;
    }
}
