using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using dabbit.Base;
using System.IO;
using System.Reflection;

namespace dabbit.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        enum IconTypes
        {
            None,
            Online,
            Offline,
            Away
        }
        //Connection cn = new Connection(new DabbitContext(), "localhost", 667, false);
        private TreeViewItem CreateTreeViewItem(string header, IconTypes icon)
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

        public MainWindow()
        {
            InitializeComponent();

            // Load Images into a cache.
            Stream awayStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.orb-away.png");
            this.awayPng = new PngBitmapDecoder(awayStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            awayStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.orb-offline.png");
            this.offlinePng = new PngBitmapDecoder(awayStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            awayStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.orb-online.png");
            this.onlinePng = new PngBitmapDecoder(awayStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            TreeViewItem tvi = CreateTreeViewItem("Settings", IconTypes.None);


            Stream docStream = Assembly.GetEntryAssembly().GetManifestResourceStream("dabbit.Win.Assets.HTML.ChatPage.html");
            brows.NavigateToStream(docStream);


            treeViewServerList.Items.Add(tvi);
            MultiSelect.AllowMultiSelection(treeViewServerList);

            //ctx.Servers.Add(new Server(ctx, new User() { Nick = "dabbit", Ident = "dabitp", Name = "David" }, new Connection(ctx, new WinSocket("localhost", 6667, false))));
            //ctx.Servers[0].OnRawMessage += MainWindow_OnRawMessage;
        }

        void MainWindow_OnRawMessage(object sender, Message e)
        {
            
        }


        private void dabbitLogo_Loaded(object sender, RoutedEventArgs e)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file = thisExe.GetManifestResourceStream("dabbit.Win.Assets.logo.png");

            // ... Create a new BitmapImage.
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.StreamSource = file;
            b.EndInit();

            // ... Get Image reference from sender.
            var image = sender as Image;
            // ... Assign Source.
            image.Source = b;
        }



        PngBitmapDecoder awayPng;
        PngBitmapDecoder offlinePng;
        PngBitmapDecoder onlinePng;

        WinContext ctx = new WinContext();
    }



}
