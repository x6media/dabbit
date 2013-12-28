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
using System.Windows.Navigation;
using System.Windows.Shapes;
using dabbit.Base;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace dabbit.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Connection cn = new Connection(new DabbitContext(), "localhost", 667, false);

        public MainWindow()
        {
            InitializeComponent();
            
        }

        void svr_OnRawMessage(object sender, Message e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
    }



}
