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

        public GuiServer(IContext ctx, User me, Connection connection)
            :base(ctx, me, connection)
        {
            this.wb = new WebBrowser();

            docStream = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.HTML.ChatPage.html");
            this.wb.NavigateToStream(docStream);
            jsb = new JavascriptBridge(this.wb);
            this.wb.ObjectForScripting = jsb;


            this.OnUnhandledEvent += GuiServer_OnUnhandledEvent;
            this.OnQueryMessageNotice += GuiServer_OnQueryMessageNotice;
            this.OnChannelMessage += GuiServer_OnChannelMessage;
            
            this.OnJoin += GuiServer_OnJoin;
            this.OnNewChannelJoin += GuiServer_OnNewChannelJoin;
            this.OnRawMessage += GuiServer_OnRawMessage;

            systemTimer.Elapsed += systemTimer_Elapsed;
            systemTimer.Start();

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
                if (who.Modes.Count != 0)
                {
                    this.wb.InvokeScript("addLine", new object[] { type.ToString().ToLower(), who.Modes[0] + " " + who.Nick, decAgain.ToString("X"), message });
                }
                else
                {
                    string[] omgtest = new string[] { type.ToString(), who.Nick, decAgain.ToString("X"), message };

                    this.wb.InvokeScript("addLine", new object[] { type.ToString().ToLower(), who.Nick, decAgain.ToString("X"), message });
                }
            });
        }

        void GuiServer_OnNewChannelJoin(object sender, JoinMessage e)
        {
            ((WinContext)this.ctx).SwitchView(this, e.Channel);
        }

        void GuiServer_OnRawMessage(object sender, Message e)
        {
            
        }

        void GuiServer_OnChannelMessage(object sender, PrivmsgMessage e)
        {
            User tmp = this.Channels[e.To.Parts[0]].Users.Where(p => p.Nick == e.From.Parts[0]).First();
            if (tmp == null)
                ((GuiChannel)this.Channels[e.To.Parts[0]]).AddLine(LineTypes.Normal, new User(e.From), e.MessageLine);
            else
                ((GuiChannel)this.Channels[e.To.Parts[0]]).AddLine(LineTypes.Normal, tmp, e.MessageLine);
        }

        void GuiServer_OnJoin(object sender, JoinMessage e)
        {
            //((GuiChannel)this.Channels[e.Channel]).jsb.OnOverlayHide += jsb_OnOverlayHide;
            //((GuiChannel)this.Channels[e.Channel]).jsb.OnOverlayVisible += jsb_OnOverlayVisible;
            ((GuiChannel)this.Channels[e.Channel]).AddLine(LineTypes.Info, new User(e.From), "has joined the channel");
            
        }

        void GuiServer_OnQueryMessageNotice(object sender, PrivmsgMessage e)
        {
            this.AddLine(LineTypes.Notice, new User(e.From), e.MessageLine);
        }

        void GuiServer_OnUnhandledEvent(object sender, Message e)
        {
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
        Timer systemTimer = new Timer(150);
        Queue<MessageQueueItem> waitingMessages = new Queue<MessageQueueItem>();
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
}
