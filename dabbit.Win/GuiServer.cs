using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dabbit.Base;
using System.Windows.Controls;
using System.Timers;
namespace dabbit.Win
{
    internal class GuiServer : Server, IWindow
    {
        public WebBrowser Window { get { return this.wb; } }

        public TreeViewItem TreeItem { get; set; }

        public GuiServer(IContext ctx, User me, Connection connection)
            :base(ctx, me, connection)
        {
            this.wb = new WebBrowser();

            docStream = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.HTML.ChatPage.html");
            this.wb.NavigateToStream(docStream);
            jsb = new JavascriptBridge(this.wb);
            this.wb.ObjectForScripting = jsb;
            this.wb.Visibility = System.Windows.Visibility.Collapsed;
            

            this.OnUnhandledEvent += GuiServer_OnUnhandledEvent;
            this.OnQueryMessageNotice += GuiServer_OnQueryMessageNotice;
            this.OnChannelMessage += GuiServer_OnChannelMessage;
            
            this.OnJoin += GuiServer_OnJoin;
            this.OnPart += GuiServer_OnPart;
            this.OnNewChannelJoin += GuiServer_OnNewChannelJoin;
            this.OnRawMessage += GuiServer_OnRawMessage;
            this.OnConnectionEstablished += GuiServer_OnConnectionEstablished;
            this.OnModeChange += GuiServer_OnModeChange;

            systemTimer.Elapsed += systemTimer_Elapsed;
            systemTimer.Start();

        }

        void GuiServer_OnPart(object sender, Message e)
        {
            e.MessageLine = e.MessageLine.Replace("&", "&nbsp;").Replace("<", "&lt;").Replace(">", "&gt;");

            ((WinContext)this.ctx).SetUserList((IWindow)this.Channels[e.Parts[2]], this.Channels[e.Parts[2]].Users.ToArray());
        }

        void GuiServer_OnModeChange(object sender, ModeMessage e)
        {
            if (e.Parts[2][0] == '#')
            {

                User tmp = this.Channels[e.Parts[2]].Users.Where(p => p.Nick == e.From.Parts[0]).First();
                if (tmp == null)
                    ((GuiChannel)this.Channels[e.Parts[2]]).AddLine(LineTypes.Info, new User(e.From), (e.Mode.ModificationType == ModeModificationType.Adding ? "adds mode " : "removes mode ") + e.Mode.Character + " " + e.Mode.Argument);
                else
                    ((GuiChannel)this.Channels[e.Parts[2]]).AddLine(LineTypes.Info, tmp, (e.Mode.ModificationType == ModeModificationType.Adding ? "adds mode " : "removes mode ") + e.Mode.Character + " " + e.Mode.Argument);

                ((WinContext)this.ctx).SetUserList((IWindow)this.Channels[e.Parts[2]], this.Channels[e.Parts[2]].Users.ToArray());
            }
            
        }


        void GuiServer_OnConnectionEstablished(object sender, Message e)
        {
            this.wb.InvokeIfRequired(() =>
            {
                TreeviewHelper.UpdateIcon(this.TreeItem, TreeviewHelper.IconTypes.Online);
                ((TextBlock)((StackPanel)this.TreeItem.Header).Children[1]).Text = " " + this.Display;
            });

            this.Connection.Write("JOIN #dab,#opers");
            
        }

        void systemTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Sends a message
            this.wb.InvokeIfRequired(() =>
            {
                if (!this.wb.IsLoaded || waitingMessages.Count == 0)
                {
                    return;
                }

                MessageQueueItem msgQueueItm = waitingMessages.Dequeue();
                User who = msgQueueItm.Who;
                LineTypes type = msgQueueItm.Type;
                string message = msgQueueItm.Message;

                int maxColor = 16764108;
                int decAgain = int.Parse(who.Host.ToMd5().Substring(0, 6), System.Globalization.NumberStyles.HexNumber);

                decAgain = decAgain % maxColor;
                string colorHex = decAgain.ToString("X");
                colorHex = colorHex.PadRight(6, '0');
                colorHex = "#" + colorHex;

                if (who.Modes.Count != 0)
                {
                    this.wb.InvokeScript("addLine", new object[] { type.ToString().ToLower(), who.Modes[0] + who.Nick, colorHex, message });
                }
                else
                {
                    string[] omgtest = new string[] { type.ToString(), who.Nick, decAgain.ToString("X"), message };

                    this.wb.InvokeScript("addLine", new object[] { type.ToString().ToLower(), who.Nick, colorHex, message });
                }
            });
        }

        void GuiServer_OnNewChannelJoin(object sender, JoinMessage e)
        {
            this.wb.InvokeIfRequired(() =>
            {
                GuiChannel chan = (GuiChannel)this.Channels[e.Channel];

                ((TextBlock)((StackPanel)chan.TreeItem.Header).Children[0]).Text = " " + e.Channel;

                ((WinContext)this.ctx).SwitchView(this, e.Channel);
            });

            
        }

        void GuiServer_OnRawMessage(object sender, Message e)
        {
            
        }

        void GuiServer_OnChannelMessage(object sender, PrivmsgMessage e)
        {
            e.MessageLine = e.MessageLine.Replace("&", "&nbsp;").Replace("<", "&lt;").Replace(">", "&gt;");

            User tmp = this.Channels[e.To.Parts[0]].Users.Where(p => p.Nick == e.From.Parts[0]).FirstOrDefault();
            if (tmp == null)
                ((GuiChannel)this.Channels[e.To.Parts[0]]).AddLine(LineTypes.Normal, new User(e.From), e.MessageLine);
            else
                ((GuiChannel)this.Channels[e.To.Parts[0]]).AddLine(LineTypes.Normal, tmp, e.MessageLine);
        }

        void GuiServer_OnJoin(object sender, JoinMessage e)
        {
            //((GuiChannel)this.Channels[e.Channel]).jsb.OnOverlayHide += jsb_OnOverlayHide;
            //((GuiChannel)this.Channels[e.Channel]).jsb.OnOverlayVisible += jsb_OnOverlayVisible;
            ((WinContext)this.ctx).SetUserList((IWindow)this.Channels[e.Channel], this.Channels[e.Channel].Users.ToArray());
            ((GuiChannel)this.Channels[e.Channel]).AddLine(LineTypes.Info, new User(e.From), "has joined the channel");
            
        }

        void GuiServer_OnQueryMessageNotice(object sender, PrivmsgMessage e)
        {
            e.MessageLine = e.MessageLine.Replace("&", "&nbsp;").Replace("<", "&lt;").Replace(">", "&gt;");
            this.AddLine(LineTypes.Notice, new User(e.From), e.MessageLine);
        }

        void GuiServer_OnUnhandledEvent(object sender, Message e)
        {
            e.MessageLine = e.MessageLine.Replace("&", "&nbsp;").Replace("<", "&lt;").Replace(">", "&gt;");
            this.AddLine(LineTypes.Normal, new User(e.From), e.MessageLine);
        }

        public void AddLine(LineTypes type, dabbit.Base.User who, string message)
        {
            this.waitingMessages.Enqueue(new MessageQueueItem(type, who, message));
        }

        public void SwitchAway(bool hideControl)
        {
            if (hideControl)
            {
                this.wb.InvokeIfRequired(() =>
                {
                    this.wb.Visibility = System.Windows.Visibility.Collapsed;
                });
                
            }

            this.SwitchAway();
        }
        public void ScrollToEnd()
        {

            this.wb.InvokeIfRequired(() =>
            {
                if (!this.wb.IsLoaded)
                {
                    tmr = new System.Timers.Timer(800);
                    tmr.AutoReset = false;
                    tmr.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
                    {
                        this.ScrollToEnd();
                    };
                    tmr.Start();
                    return;
                }

                this.wb.InvokeScript("scrollToEnd", new object[] { });

            });
        }

        public void SwitchAway()
        {

            this.wb.InvokeIfRequired(() =>
                {
                    if (!this.wb.IsLoaded)
                    {
                        tmr = new System.Timers.Timer(800);
                        tmr.AutoReset = false;
                        tmr.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
                        {
                            this.SwitchAway();
                        };
                        tmr.Start();
                        return;
                    }

                    this.wb.InvokeScript("toggleOverlay", new object[] { true });

                });
        }

        public void SwitchTo()
        {

            this.wb.InvokeIfRequired(() =>
                {
                    if (!this.wb.IsLoaded)
                    {
                        tmr = new System.Timers.Timer(800);
                        tmr.AutoReset = false;
                        tmr.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
                        {
                            this.SwitchTo();
                        };
                        tmr.Start();
                        return;
                    }

                    this.wb.InvokeScript("toggleOverlay", new object[] { false });
                    this.wb.Visibility = System.Windows.Visibility.Visible;
                });

        }


        Timer tmr;
        Timer systemTimer = new Timer(300);
        Queue<MessageQueueItem> waitingMessages = new Queue<MessageQueueItem>();
        string waitingJoinSwitches = String.Empty;

        private WebBrowser wb; 
        internal JavascriptBridge jsb;

        System.IO.Stream docStream;
    }

    internal class MessageQueueItem
    {
        public MessageQueueItem(LineTypes type, User who, string message)
        {
            this.Type = type;
            this.Who = who;
            this.Message = message;
        }

        public LineTypes Type { get; private set; }
        public User Who { get; private set; }
        public string Message { get; private set; }
    }

    internal class JoinSwitchQueueItem
    {
        public string Channel { get; private set; }

        public JoinSwitchQueueItem(string channel)
        {
            this.Channel = channel;
        }
    }
}
