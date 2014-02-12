using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using dabbit.Base;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace dabbit.Win
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            this.ctx = new WinContext(CenterGrid);

            TreeViewItem tvi = TreeviewHelper.CreateTreeViewItem("Settings", TreeviewHelper.IconTypes.None);

            treeViewServerList.Items.Add(tvi);
            MultiSelect.AllowMultiSelection(treeViewServerList);

            //ctx.Servers.Add(new Server(ctx, new User() { Nick = "dabbit", Ident = "dabitp", Name = "David" }, new Connection(ctx, new WinSocket("localhost", 6667, false))));
            //ctx.Servers[0].OnRawMessage += MainWindow_OnRawMessage;

            
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




        WinContext ctx;
        

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                //brows.InvokeScript("addLine", new object[] { "notice", "& dab", "#FF0000", textInput.Text });
                string tmp = textInput.Text;
                if (tmp[0] == '/')
                {
                    string[] parts = tmp.Split(' ');
                    if (parts[0] == "/server")
                    {
                        User temp = new User() { Nick = "dabbit", Ident = "dabitp", Name = "David"};



                        this.ctx.AddServer(this.ctx, temp, this.ctx.CreateConnection(ConnectionType.Direct, new WinSocket(parts[1], 6667, false)));
                    }
                    else
                    {
                        this.ctx.SendToActivewindow(textInput.Text);
                    }
                }
                else
                {
                    this.ctx.SendToActivewindow(textInput.Text);
                }
                textInput.Text = "";
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ctx.FormClosing();
        }
    }



}
