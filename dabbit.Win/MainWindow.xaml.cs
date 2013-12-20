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
            //Socket sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);

            //sock.Connect(ip);
            //NetworkStream ns = new NetworkStream(sock);
            //cn.Reader = new System.IO.StreamReader(ns);
            //cn.Writer = new System.IO.StreamWriter(ns);
        }
    }

}
