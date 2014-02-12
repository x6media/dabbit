using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Documents;

namespace dabbit.Win
{

    internal interface IWindow
    {
        WebBrowser Window { get; }
        void AddLine(LineTypes type, dabbit.Base.User who, string message);
        void SwitchAway(bool hideControl);
        void SwitchAway();
        void SwitchTo();
    }
    internal enum LineTypes
    {
        Normal,
        Notice,
        Info,
        Action
    }
    [ComVisible(true)]
    public class JavascriptBridge
    {
        private WebBrowser browser;

        public JavascriptBridge(WebBrowser sender)
        {
            this.browser = sender;
        }

        public event OverlayVisible OnOverlayVisible;
        public event OverlayHide OnOverlayHide;

        public void OverlayVisible()
        {
            if (this.OnOverlayVisible != null)
            {
                this.OnOverlayVisible(this.browser);
            }
        }
        public void OverlayHide()
        {
            if (this.OnOverlayHide != null)
            {
                this.OnOverlayHide(this.browser);
            }
        }
    }

    public delegate void OverlayVisible(object sender);
    public delegate void OverlayHide(object sender);


    public class TreeviewHelper
    {

        static TreeviewHelper()
        {

            // Load Images into a cache.
            Stream awayStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.orb-away.png");
            awayPng = new PngBitmapDecoder(awayStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            awayStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.orb-offline.png");
            offlinePng = new PngBitmapDecoder(awayStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            awayStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.orb-online.png");
            onlinePng = new PngBitmapDecoder(awayStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        public enum IconTypes
        {
            None,
            Online,
            Offline,
            Away
        }

        //Connection cn = new Connection(new DabbitContext(), "localhost", 667, false);
        public static TreeViewItem CreateTreeViewItem(string header, IconTypes icon)
        {

            TreeViewItem child = new TreeViewItem();
            StackPanel pan = new StackPanel();

            if (icon != IconTypes.None)
            {

                pan.Orientation = Orientation.Horizontal;

                Image image = new Image();
                image.Height = 16;

                switch (icon)
                {
                    case IconTypes.Offline:
                        image.Source = offlinePng.Frames[0];
                        break;
                    case IconTypes.Online:
                        image.Source = onlinePng.Frames[0];
                        break;
                    case IconTypes.Away:
                        image.Source = awayPng.Frames[0];
                        break;
                }

                pan.Children.Add(image);
            }

            pan.Children.Add(new TextBlock(new Run("  " + header)));
            child.Header = pan;
            return child;
        }


        private static readonly PngBitmapDecoder awayPng;
        private static readonly PngBitmapDecoder offlinePng;
        private static readonly PngBitmapDecoder onlinePng;
    }

}
