using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dabbit.Base;
using System.Windows.Controls;

namespace dabbit.Win
{
    internal class GuiChannel : Channel, IWindow
    {
        public WebBrowser Window { get { return this.wb; } }

        internal GuiChannel(Server svr)
            :base(svr)
        {
            docStream = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.HTML.ChatPage.html");
            this.wb = new WebBrowser();
            this.wb.NavigateToStream(docStream);
            jsb = new JavascriptBridge(this.wb);
            this.wb.ObjectForScripting = jsb;
        }


        public void AddLine(LineTypes type, dabbit.Base.User who, string message)
        {
            this.wb.InvokeIfRequired(() =>
            {
                if (!this.wb.IsLoaded)
                {
                    tmr = new System.Timers.Timer(800);
                    tmr.AutoReset = false;
                    tmr.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
                    {
                        this.AddLine(type, who, message);
                    };
                    tmr.Start();
                    return;
                }

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

                    this.wb.InvokeScript("addLine", new object[] { type.ToString().ToLower(), "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + who.Nick, decAgain.ToString("X"), message });
                }
            });

        }


        public void SwitchAway(bool hideControl)
        {
            if (hideControl)
            {
                this.wb.Visibility = System.Windows.Visibility.Collapsed;
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

        System.Timers.Timer tmr;
        System.IO.Stream docStream;
        private WebBrowser wb;
        internal JavascriptBridge jsb;
    }
}
