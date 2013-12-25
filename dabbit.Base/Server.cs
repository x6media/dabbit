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
        public Dictionary<string, Channel> Channels { get; protected set; }
        public Connection Connection { get { return this.connection; } }
        public string Display { get { return this.Attributes["NETWORK"]; } }
        public string Password { get; set; }

        public bool MultiModes { get { return this.multiModes; } }
        public bool HostInNames { get { return this.hostInNames; } }
        
        public Server(IContext ctx, User me, Connection connection)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

            this.me = me;
            if (me.Modes == null)
                me.Modes = new List<string>();

            this.ctx = ctx;
            this.Channels = new Dictionary<string, Channel>();
            
            this.connection = connection;
            this.connection.RawMessageReceived = this.rawMessageReceived;

            // Add prefined and used attributes
            this.Attributes.Add("NETWORK", this.connection.Host);
            this.Attributes.Add("STATUSMSG", "");
            this.Attributes.Add("CHANTYPES", "");

            this.connection.Connect();

            this.connection.Write("CAP LS"); // Get list of extras (For multi prefix)
            
            if (!String.IsNullOrEmpty(this.Password))
            {
                this.connection.Write("PASS " + this.Password);
            }

            this.connection.Write("NICK " + this.Me.Nick);
            this.connection.Write("USER " + this.Me.Ident + " * * :" + this.Me.Name);
        }




#region Events

        public event IrcEventHandler OnRawMessage; //
        public event ErrorEventHandler OnError; //
        public event JoinEventHandler OnNewChannelJoin; //
        public event PartEventHandler OnCloseChannelPart; //
        public event JoinEventHandler OnJoin; //
        public event NamesEventHandler OnNames;
        public event ListEventHandler OnList;
        public event PartEventHandler OnPart; //
        public event QuitEventHandler OnQuit;
        public event KickEventHandler OnKick;
        public event IrcEventHandler OnUnAway;
        public event IrcEventHandler OnAway;
        public event InviteEventHandler OnInvite;
        public event BanEventHandler OnBan;
        public event UnbanEventHandler OnUnban;

        public event WhoEventHandler OnWho;
        public event MotdEventHandler OnMotd;
        public event TopicEventHandler OnTopic;
        public event TopicChangeEventHandler OnTopicChange;
        public event NickChangeEventHandler OnNickChange;
        public event ModeChangeEventHandler OnModeChange;
        public event IrcEventHandler OnUserModeChange;
        public event IrcEventHandler OnChannelModeChange;

        public event PrivmsgEventHandler OnChannelMessage; // Channel->No Action->Not Notice
        public event PrivmsgEventHandler OnChannelMessageNotice; // Channel->No Action->Notice
        public event ActionEventHandler OnChannelAction; // Channel->Action->Not Notice
        public event ActionEventHandler OnChannelActionNotice; // Channel->Action->Notice

        public event PrivmsgEventHandler OnQueryMessage; // Query->No Action->Not Notice
        public event PrivmsgEventHandler OnQueryMessageNotice; // Query->No Action->Notice
        public event ActionEventHandler OnQueryAction; // Query->Action->Not Notice
        public event ActionEventHandler OnQueryActionNotice; // Query->Action->Notice

        public event CtcpEventHandler OnCtcpRequest;
        public event CtcpEventHandler OnCtcpReply;

#endregion

        private void rawMessageReceived(Message msg)
        {
            if (msg == null)
            {
                return;
            }


            if (this.OnRawMessage != null)
                this.OnRawMessage(this, msg);

            switch (msg.Command)
            {
                #region PRIVMSG
                case "PRIVMSG":
                    PrivmsgMessage pvm = new PrivmsgMessage(msg);

                    // We are parsing a message to a channel
                    pvm.To = new SourceEntity(new string[] { msg.Parts[2] }, SourceEntityType.Channel);

                    if (this.Attributes["STATUSMSG"].Contains(msg.Parts[2][0].ToString()))
                    {
                        // Check for a wallops message (+#channel)
                        pvm.Wall = msg.Parts[2][0].ToString();
                        msg.Parts[2] = msg.Parts[2].Substring(1);

                        pvm.To = new SourceEntity(new string[] { msg.Parts[2] }, SourceEntityType.Channel);
                    }
    
                    if (this.Attributes["CHANTYPES"].Contains(pvm.Parts[2][0].ToString()))
                    {
                        if (msg.Parts[3] == ":\001ACTION")
                        {
                            msg.MessageLine = msg.MessageLine.Substring(8, msg.MessageLine.Length - 10);
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

                    if (msg.Parts[3] == ":\001ACTION")
                    {
                        // Remove ending \001
                        if (msg.MessageLine[msg.MessageLine.Length - 1] == '\u0001')
                        {
                            msg.MessageLine = msg.MessageLine.Substring(9, msg.MessageLine.Length - 1);
                            string lastpart = msg.Parts[msg.Parts.Count() - 1];
                            msg.Parts[msg.Parts.Count() - 1] = lastpart.Substring(0, lastpart.Length - 1);
                        }

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
                #endregion
                #region NOTICE
                case "NOTICE":
                    pvm = new PrivmsgMessage(msg);

                    // We are parsing a message to a channel
                    pvm.To = new SourceEntity(new string[] { msg.Parts[2] }, SourceEntityType.Channel);

                    if (this.Attributes["STATUSMSG"].Contains(msg.Parts[2][0].ToString()))
                    {
                        // Check for a wallops message (+#channel)
                        pvm.Wall = msg.Parts[2][0].ToString();
                        msg.Parts[2] = msg.Parts[2].Substring(1);

                        pvm.To = new SourceEntity(new string[] { msg.Parts[2] }, SourceEntityType.Channel);
                    }
    
                    if (this.Attributes["CHANTYPES"].Contains(pvm.Parts[2][0].ToString()))
                    {
                        if (msg.Parts[3] == ":\001ACTION")
                        {
                            msg.MessageLine = msg.MessageLine.Substring(9, msg.MessageLine.Length - 10);
                            // CTCP Action
                            if (this.OnChannelActionNotice != null)
                            {
                                this.OnChannelActionNotice(this, pvm);
                            }
                        }
                        else
                        {
                            if (this.OnChannelMessageNotice != null)
                            {
                                this.OnChannelMessageNotice(this, pvm);
                            }
                        }

                        break;
                    }

                    // A message is being sent to a non-channel which means it HAS to be going to us.
                    pvm.To = new SourceEntity(pvm.To.Parts, SourceEntityType.Client);

                    if (msg.Parts[3] == ":\001ACTION")
                    {
                        // Remove ending \001
                        if (msg.MessageLine[msg.MessageLine.Length - 1] == '\u0001')
                        {
                            msg.MessageLine = msg.MessageLine.Substring(8, msg.MessageLine.Length - 1);
                            string lastpart = msg.Parts[msg.Parts.Count() - 1];
                            msg.Parts[msg.Parts.Count() - 1] = lastpart.Substring(0, lastpart.Length - 1);
                        }

                        // CTCP Action
                        if (this.OnQueryActionNotice != null)
                        {
                            this.OnQueryActionNotice(this, pvm);
                        }
                    }
                    else
                    {
                        if (this.OnQueryMessageNotice != null)
                        {
                            this.OnQueryMessageNotice(this, pvm);
                        }
                    }

                    break;
                #endregion
                case "PING":
                    this.connection.Write("PONG " + msg.Parts[1]);
                    break;
                case "ERROR":
                    this.OnError(this, msg);
                    break;
                #region JOIN
                case "JOIN":
                    JoinMessage jm = new JoinMessage(msg);
                    if (msg.From.Parts[0] == this.Me.Nick)
                    {
                        Channel value;
                        this.Channels.TryGetValue(msg.Parts[2], out value);

                        if (value == null)
                        {
                            value = new Channel();
                            value.Modes = new List<Mode>();
                            value.Users = new List<User>();
                            value.Topic = new Topic();

                            this.Channels.Add(msg.Parts[2], value);
                        }
                        value.Name = msg.Parts[2];
                        value.Display = value.Name;

                        this.connection.Write("MODE " + jm.Channel);

                        this.Channels[msg.Parts[2]] = value;

                        if (value.ChannelLoaded && this.OnNewChannelJoin != null)
                        {
                            this.OnNewChannelJoin(this, jm);
                        }
                    }
                    else
                    {
                        User usr = new User();
                        usr.Nick = jm.From.Parts[0];
                        usr.Ident = jm.From.Parts[1];
                        usr.Host = jm.From.Parts[2];
                        usr.Modes = new List<string>();

                        this.Channels[jm.Channel].Users.Add(usr);


                        this.Channels[jm.Channel].Users.Sort(sortuser);
                        if (this.OnJoin != null)
                        {
                            this.OnJoin(this, jm);
                        }
                        
                    }
                    break;
                #endregion
                #region 324
                case "324": // :hyperion.gamergalaxy.net 324 dabbbb #dab +r
                    Channel chnl;
                    this.Channels.TryGetValue(msg.Parts[3], out chnl);

                    if (chnl == null)
                    {
                        // do nothing because we aren't a member of this channel
                        return;
                    }

                    //chnl.Modes = new List<Mode>();

                    string modes = msg.Parts[4];
                    int paramidx = 5;

                    // 1 was from the + sign in the model.
                    for (int i = 1; i < modes.Length; i++)
                    {
                        Mode mode = new Mode();

                        // Is this a mode with a parameter?
                        if (this.Attributes["CHANMODES_B"].Contains(modes[i].ToString()))
                        {
                            mode.Argument = msg.Parts[paramidx++];
                        }
                        else
                        {
                            mode.Argument = String.Empty;
                        }

                        mode.Character = modes[i];
                        mode.Type = ModeType.Channel;
                        chnl.Modes.Add(mode);
                    }

                    this.Channels[msg.Parts[3]] = chnl;

                    break;
                #endregion
                #region 353
                case "353": // /Names list item :hyperion.gamergalaxy.net 353 badddd = #dab :badddd BB-Aso
                    //msg.Parts[4] = msg.Parts[4].Substring(1);
                    
                    Channel vall;
                    this.Channels.TryGetValue(msg.Parts[4], out vall);

                    // Should NEVER HAPPEN
                    if (vall == null)
                    {
                        // We don't want to execute this
                        return;
                    }

                    if (vall.Users == null)
                    {
                        vall.Users = new List<User>();
                    }

                    msg.Parts[5] = msg.Parts[5].Substring(1);
                    string prefixes = this.Attributes["PREFIX_PREFIXES"];

                    for (int i = 5; i < msg.Parts.Count(); i++)
                    {
                        User tempuser = new User();
                        tempuser.Modes = new List<string>();

                        if (this.HostInNames)
                        {
                            string[] nick = msg.Parts[i].Split('!');
                            string[] identhost = nick[1].Split('@');
                            tempuser.Nick = nick[0];
                            tempuser.Ident = identhost[0];
                            tempuser.Host = identhost[1];
                        }
                        else
                        {
                            tempuser.Nick = msg.Parts[i];
                        }
                        
                        while (prefixes.Contains(tempuser.Nick[0].ToString()))
                        {
                            tempuser.Modes.Add(tempuser.Nick[0].ToString());
                            tempuser.Nick = tempuser.Nick.Substring(1);

                            tempuser.Modes.Sort(delegate(string s1, string s2)
                            {
                                return prefixes.IndexOf(s1[0]).CompareTo(prefixes.IndexOf(s2[0]));
                            });
                        }
                        

                        //JoinMessage xinmsg = new JoinMessage(msg);
                        //joinmsg.Channel = msg.Parts[3];


                        vall.Users.Add(tempuser);

                        vall.Users.Sort(sortuser);
                    }


                    this.Channels[msg.Parts[5]] = vall;

                    break;
                #endregion
                case "PART":

                    this.Channels[msg.Parts[2]].Users.Remove
                        (this.Channels[msg.Parts[2]].Users.Where(u => u.Nick == msg.From.Parts[0]).First());

                    if (this.OnPart != null)
                    {
                        this.OnPart(this, msg);
                    }


                    if (msg.From.Parts[0] == me.Nick)
                    {
                        this.Channels.Remove(msg.Parts[2]);

                        if (this.OnCloseChannelPart != null)
                        {
                            this.OnCloseChannelPart(this, msg);
                        }
                    }

                    break;
                case "QUIT":
                    List<string> channels = new List<string>();

                    foreach (var chn in this.Channels)
                    {
                        User usr = chn.Value.Users.Where(u => u.Nick == msg.From.Parts[0]).FirstOrDefault();

                        if (usr != null)
                        {
                            chn.Value.Users.Remove(usr);
                            channels.Add(chn.Key);
                        }

                    }
                    this.OnQuit(this, new QuitMessage(msg) { Channels = channels.ToArray() });
                    
                    break;
                #region MODE
                case "MODE":

                    string modesstring = msg.Parts[3];
                    int paramsindex = 4;
                    bool adding = false;

                    string prefixz = this.Attributes["PREFIX_PREFIXES"];

                    for (int i = 0; i < modesstring.Length; i++)
                    {
                        if (modesstring[i] == '+')
                        {
                            adding = true;
                            continue;
                        }
                        else if (modesstring[i] == '-')
                        {
                            adding = false;
                            continue;
                        }


                        Mode mode = new Mode();
                        mode.Display = mode.Character;
                        mode.ModificationType = adding ? ModeModificationType.Adding : ModeModificationType.Removing;

                        if (this.Attributes["CHANMODES_A"].Contains(modesstring[i].ToString()) ||
                            this.Attributes["CHANMODES_B"].Contains(modesstring[i].ToString()) ||
                            this.Attributes["CHANMODES_C"].Contains(modesstring[i].ToString()))
                        {
                            mode.Argument = msg.Parts[paramsindex++];
                        }

                        mode.Character = modesstring[i];

                        if (msg.Parts[2] != me.Nick && this.Attributes["PREFIX_MODES"].Contains(modesstring[i].ToString()))
                        {
                            mode.Type = ModeType.User;
                            mode.Argument = msg.Parts[paramsindex++];

                            mode.Character = this.Attributes["PREFIX_PREFIXES"][this.Attributes["PREFIX_MODES"].IndexOf(mode.Character)];

                            int userid = this.Channels[msg.Parts[2]].Users.
                                Select((item, index) => new { Index = index, Item = item })
                                .Where(u => u.Item.Nick == mode.Argument).First().Index;

                            if (mode.ModificationType == ModeModificationType.Removing)
                            {
                                this.Channels[msg.Parts[2]].Users[userid].Modes.
                                    Remove(this.Channels[msg.Parts[2]].Users[userid].Modes.
                                    Where(m => m == mode.Character.ToString()).First());


                                this.Channels[msg.Parts[2]].Users[userid].Modes.Sort(delegate(string s1, string s2)
                                {
                                    return prefixz.IndexOf(s1[0]).CompareTo(prefixz.IndexOf(s2[0]));
                                });
                            }
                            else
                            {
                                this.Channels[msg.Parts[2]].Users[userid].Modes.Add(mode.Character.ToString());
                            }

                            this.Channels[msg.Parts[2]].Users.Sort(sortuser);
                        }
                        else if (msg.Parts[2] == me.Nick)
                        {
                            mode.Type = ModeType.UMode;
                            if (mode.ModificationType == ModeModificationType.Adding)
                            {
                                me.Modes.Add(mode.Character.ToString());
                            }
                            else
                            {
                                me.Modes.Remove(me.Modes.Where(m => m[0] == mode.Character).First());
                            }
                        }
                        else
                        {
                            mode.Type = ModeType.Channel;
                            if (mode.ModificationType == ModeModificationType.Adding)
                            {
                                this.Channels[msg.Parts[2]].Modes.Add(mode);
                            }
                            else
                            {
                                this.Channels[msg.Parts[2]].Modes.Remove
                                    (this.Channels[msg.Parts[2]].Modes.Where(m => m.Character == mode.Character).First());
                            }
                        }

                        if (this.OnModeChange != null)
                        {
                            this.OnModeChange(this, new ModeMessage(msg) { Mode = mode });
                        }

                    }

                    break;
                #endregion
                case "366": // End of /names list

                    JoinMessage jm_ = new JoinMessage(msg);
                    jm_.Channel = msg.Parts[3];

                    if (this.Channels[jm_.Channel].ChannelLoaded && this.OnNewChannelJoin != null)
                    {
                        this.OnNewChannelJoin(this, jm_);
                    }
                    break;
                case "332":
                    Channel tmpchan;

                    this.Channels.TryGetValue(msg.Parts[3], out tmpchan);

                    if (tmpchan == null)
                    {
                        // We don't want to execute in case the user called this command outside of a channel
                        return;
                    }

                    tmpchan.Topic = new Topic();
                    tmpchan.Topic.Display = msg.MessageLine;

                    this.Channels[msg.Parts[3]] = tmpchan;
                    break;
                case "333": // Who set the topic and when they set it
                    
                    Channel tmpchan2;

                    this.Channels.TryGetValue(msg.Parts[3], out tmpchan2);

                    if (tmpchan2 == null)
                    {
                        // If a user calls topic by themselves we don't want to execute this
                        return;
                    }

                    tmpchan2.Topic.SetBy = msg.Parts[4];
                    // Set a dateTime to the beginning of unix epoch
                    DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                    // Add the # of seconds (the date of which we set the channel topic)
                    dateTime = dateTime.AddSeconds(Int32.Parse(msg.Parts[5]));

                    tmpchan2.Topic.DateSet = dateTime;

                    this.Channels[msg.Parts[3]] = tmpchan2;
                    break;
                case "372": // MOTD 376 eomtd
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
                #region 005
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
                        }
                        else
                        {
                            this.attributes[key] = value;
                        }

                        if (key == "NAMESX")
                        {
                            this.connection.Write("PROTOCTL NAMESX");
                            this.multiModes = true;
                        }
                        else if (key == "UHNAMES")
                        {
                            this.connection.Write("PROTOCTL UHNAMES");
                        } 
                        else if (key == "PREFIX")
                        {
                            string tosplit = value.Substring(1);
                            string[] split = tosplit.Split(')');
                            this.Attributes.Add("PREFIX_MODES", split[0]);
                            this.Attributes.Add("PREFIX_PREFIXES", split[1]);
                            this.hostInNames = true;
                        }
                        else if (key == "CHANMODES")
                        {
                            string[] chanmodes = value.Split(',');

                            // Mode that adds or removes nick or address to a list
                            this.Attributes.Add("CHANMODES_A", chanmodes[0]);
                            // Changes a setting and always had a parameter
                            this.Attributes.Add("CHANMODES_B", chanmodes[1]);
                            // Only has a parameter when set
                            this.Attributes.Add("CHANMODES_C", chanmodes[2]);
                            // Never has a parameter
                            this.Attributes.Add("CHANMODES_D", chanmodes[3]);
                        }
                    }
                    break;
                #endregion
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

        private int sortuser(User u1, User u2)
        {

            string prefixes = this.Attributes["PREFIX_PREFIXES"];

            if (u1.Modes.Count() == 0)
            {
                if (u2.Modes.Count() == 0)
                {
                    return u1.Nick.CompareTo(u2.Nick);
                }
                return 1;
            }

            if (u2.Modes.Count() == 0)
            {
                return -1;
            }

            int res = prefixes.IndexOf(u1.Modes[0][0]).CompareTo(prefixes.IndexOf(u2.Modes[0][0]));

            if (res == 0)
            {
                res = u1.Nick.CompareTo(u2.Nick);
            }

            return res;
        }

        private List<Channel> channels = new List<Channel>();
        private User me;
        private ServerType serverType = ServerType.Unknown;
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        private Connection connection;
        private IContext ctx;
        private bool multiModes = false;
        private bool hostInNames = false;
    }
}
