using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace dabbit.Base
{
    static class ServerTypeSearch
    {
        public static string[] MatchToServerType = new string[] { "Unreal3.2", "ircd-seven-1" };
    }

    public enum ServerType
    {
        Unknown = 999,

        Unreal3_2 = 0, // Match # to index above

        ircd7_1 = 1
    }


    public class Server
    {
        public string Name { get; set; }
        public ServerType Type { get { return this.serverType; } }
        public User Me { get { return this.me; } }
        public Dictionary<string, string> Attributes { get { return this.attributes; } }
        public List<Channel> Channels { get; protected set; }
        public Connection Connection { get { return this.connection; } }
        public string Display { get { return this.Attributes["Network"]; } }
        public string Password { get; set; }

        public bool MultiModes { get { return this.multiModes; } }
        
        public Server(IContext ctx, Connection connection)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

            this.me = new User();

            this.ctx = ctx;

            this.connection = connection;
            this.connection.RawMessageReceived = this.RawMessageReceived;
            this.Attributes.Add("Network", this.connection.Host);
            this.connection.Connect();

            this.connection.Write("CAP LS"); // Get list of extras (For multi prefix)
            
            if (!String.IsNullOrEmpty(this.Password))
            {
                this.connection.Write("PASS " + this.Password);
            }

            this.connection.Write("NICK " + this.Me.Nick);
            this.connection.Write("USER " + this.Me.Ident + " 8 * :" + this.Me.Name);
        }




#region Events

        public event PingEventHandler OnPing;
        public event PongEventHandler OnPong;
        public event IrcEventHandler OnRawMessage;
        public event ErrorEventHandler OnError;
        public event IrcEventHandler OnErrorMessage;
        public event JoinEventHandler OnJoin;
        public event NamesEventHandler OnNames;
        public event ListEventHandler OnList;
        public event PartEventHandler OnPart;
        public event QuitEventHandler OnQuit;
        public event KickEventHandler OnKick;
        public event IrcEventHandler OnUnAway;
        public event IrcEventHandler OnAway;
        public event InviteEventHandler OnInvite;
        public event BanEventHandler OnBan;
        public event UnbanEventHandler OnUnban;
        /*public event OpEventHandler OnOp;
        public event DeopEventHandler OnDeop;
        public event AdminEventHandler OnAdmin;
        public event DeadminEventHandler OnDeadmin;
        public event IrcopEventHandler OnIrcop;
        public event DeircopEventHandler OnDeircop;
        public event HalfopEventHandler OnHalfop;
        public event DehalfopEventHandler OnDehalfop;
        public event OwnerEventHandler OnOwner;
        public event DeownerEventHandler OnDeowner;
        public event VoiceEventHandler OnVoice;
        public event DevoiceEventHandler OnDevoice;
         */
        public event WhoEventHandler OnWho;
        public event MotdEventHandler OnMotd;
        public event TopicEventHandler OnTopic;
        public event TopicChangeEventHandler OnTopicChange;
        public event NickChangeEventHandler OnNickChange;
        public event IrcEventHandler OnModeChange;
        public event IrcEventHandler OnUserModeChange;
        public event IrcEventHandler OnChannelModeChange;
        public event IrcEventHandler OnChannelMessage;
        public event ActionEventHandler OnChannelAction;
        public event IrcEventHandler OnChannelNotice;
        public event IrcEventHandler OnQueryMessage;
        public event ActionEventHandler OnQueryAction;
        public event IrcEventHandler OnQueryNotice;
        public event CtcpEventHandler OnCtcpRequest;
        public event CtcpEventHandler OnCtcpReply;

#endregion

        private void RawMessageReceived(Message msg)
        {

            if (this.OnRawMessage != null)
                this.OnRawMessage(this, msg);

            switch (msg.Command)
            {
                case "PRIVMSG":
                    PrivmsgMessage pvm = new PrivmsgMessage(msg);

                    if (this.Attributes["STATUSMSG"].Contains(msg.Parts[2][0].ToString()))
                    {
                        // Check for a wallops message (+#channel)
                        pvm.Wall = msg.Parts[2][0].ToString();
                        pvm.To = new SourceEntity(new string[] { msg.Parts[2].Substring(1) }, SourceEntityType.Channel);
                    }
    
                    if (this.Attributes["CHANTYPES"].Contains(pvm.To.Parts[0][0].ToString()))
                    {
                        // We are parsing a message to a channel
                        pvm.To = new SourceEntity(new string[] { msg.Parts[2] }, SourceEntityType.Channel);
                        if (msg.MessageLine[0] == '\u0001')
                        {
                            // CTCP Action
                            if (this.OnChannelAction != null)
                            {
                                this.OnChannelAction(this, pvm);
                            }
                        }
                        else
                        {
                            if (this.OnChannelMessage != null)
                            {
                                this.OnChannelMessage(this, pvm);
                            }
                        }

                        break;
                    }

                    // A message is being sent to a non-channel which means it HAS to be going to us.
                    pvm.To = new SourceEntity(pvm.To.Parts, SourceEntityType.Client);

                    if (msg.MessageLine[0] == '\u0001')
                    {
                        // CTCP Action
                        if (this.OnQueryAction != null)
                        {
                            this.OnQueryAction(this, pvm);
                        }
                    }
                    else
                    {
                        if (this.OnQueryMessage != null)
                        {
                            this.OnQueryMessage(this, pvm);
                        }
                    }

                    break;
                case "NOTICE":
                    
                    break;
                case "PING":
                    this.connection.Write("PONG " + msg.Parts[1]);
                    break;

                case "004": // Get Server Type
                    Array values = Enum.GetValues(typeof(ServerType));
                    this.me.Nick = msg.Parts[2];

                    for( int i = 0; i < ServerTypeSearch.MatchToServerType.Count(); i++ )
                    {
                        if (msg.Parts[4].StartsWith(ServerTypeSearch.MatchToServerType[i]))
                        {
                            this.serverType = (ServerType)i;
                            break;
                        }
                    }
                    break;
                case "005":
                    for (int i = 3; i < msg.Parts.Count(); i++)
                    {
                        string key = String.Empty;
                        string value = String.Empty;

                        if (msg.Parts[i].Contains("="))
                        {
                            string[] sep = msg.Parts[i].Split('=');
                            key = sep[0];
                            value = sep[1];
                        }
                        else
                        {
                            key = msg.Parts[i];
                            value = "true";
                        }

                        if (!this.attributes.ContainsKey(key))
                        {
                            this.attributes.Add(key, value);

                            if (key == "NAMESX")
                            {
                                this.connection.Write("PROTOCTL NAMESX");
                            }
                        }
                    }
                    break;
                case "CAP":
                    // :leguin.freenode.net CAP goooooodab LS :account-notify extended-join identify-msg multi-prefix sasl

                    if (msg.Parts.Count() < 5)
                        break;

                    if (msg.Parts[3] != "LS")
                        break;

                    // remove leading : so we can do a direct check
                    msg.Parts[4] = msg.Parts[4].Substring(1);

                    for (int i = 4; i < msg.Parts.Count(); i++)
                    {
                        if (msg.Parts[i] == "multi-prefix")
                        {
                            this.connection.Write("CAP REQ :multi-prefix");
                            break;
                        }
                    }

                    this.connection.Write("CAP END");
                    break;
            }
        }

        private List<Channel> channels = new List<Channel>();
        private User me;
        private ServerType serverType = ServerType.Unknown;
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        private Connection connection;
        private IContext ctx;
        private bool multiModes = false;
    }
}
