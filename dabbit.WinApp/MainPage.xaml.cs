using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Networking.Sockets;
using System.Reflection;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using dabbit.Base;

namespace dabbit.WinApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Stream awayStream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("dabbit.WinApp.Assets.HTML.ChatPage.html");

            using (StreamReader reader = new StreamReader(awayStream))
            {
                this.readPage = reader.ReadToEnd();
            }

            brows.NavigateToString(this.readPage);
            browsb.NavigateToString(this.readPage);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        //Connection con = new Connection(new DabbitContext(), "localhost", 6667, false);
        StreamSocket ss = new StreamSocket();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DoThis();
        }

        private async void DoThis()
        {
            
            await ss.ConnectAsync(new Windows.Networking.HostName("localhost"), "6667");
            //con.Reader = new StreamReader(ss.OutputStream.AsStreamForWrite());
            //con.Writer = new StreamWriter(ss.InputStream.AsStreamForRead());

        }

        private void appBarBottom_Opened(object sender, object e)
        {

        }

        private string readPage = String.Empty;
    }
}
