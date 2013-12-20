using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dabbit.Base
{
    public class StringMatchAttribute : Attribute
    {
        public string Match { get; set; }

        public StringMatchAttribute(string match)
        {
            this.Match = match;
        }
    }
    public enum ServerType
    {
        Unknown,

        [StringMatch("Unreal3.2")]
        Unreal3_2,

        [StringMatch("ircd-seven-1")]
        ircd7_1
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
        public event AwayEventHandler OnAway;
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
            switch (msg.Command)
            {
                case "PRIVMSG":

                    break;
                case "NOTICE":
                    
                    break;
                case "PING":
                    this.connection.Write("PONG " + msg.Parts[1]);
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
