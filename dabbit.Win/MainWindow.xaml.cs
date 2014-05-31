using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using dabbit.Base;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

            this.ctx = new WinContext(CenterGrid, treeViewServerList, userBox);

            TreeViewItem tvi = TreeviewHelper.CreateTreeViewItem("Settings", TreeviewHelper.IconTypes.None);

            treeViewServerList.Items.Add(tvi);
            MultiSelect.AllowMultiSelection(treeViewServerList, this.OnSelectedItemsChanged);
            //ctx.Servers.Add(new Server(ctx, new User() { Nick = "dabbit", Ident = "dabitp", Name = "David" }, new Connection(ctx, new WinSocket("localhost", 6667, false))));
            //ctx.Servers[0].OnRawMessage += MainWindow_OnRawMessage;
        }

        void OnSelectedItemsChanged(object sender, System.Collections.Generic.List<TreeViewItem> selectedItems)
        {
            if (selectedItems.Count >= 1)
            {
                if (selectedItems[0].Parent.GetType() == typeof(TreeView))
                {
                    GuiServer server = getServerFromTreeView(selectedItems[0]);

                    if (server != null)
                    {
                        this.ctx.SwitchView(server, null);
                    }
                    else
                    {
                        TextBlock txt = ((StackPanel)selectedItems[0].Header).Children[0] as TextBlock;
                        if (txt != null)
                        {

                        }
                    }
                }
                else
                {
                    GuiServer server = getServerFromTreeView((TreeViewItem)selectedItems[0].Parent);

                    if (server != null)
                    {
                        this.ctx.SwitchView(server, ((TextBlock)((StackPanel)selectedItems[0].Header).Children[0]).Text);
                    }
                }
            }

            for(int i = 1; i < selectedItems.Count; i++)
            {
                if (selectedItems[i].Parent.GetType() == typeof(TreeView))
                {
                    GuiServer server = getServerFromTreeView(selectedItems[i]);

                    if (server != null)
                    {
                        this.ctx.AppendView(server, null);
                    }
                }
                else
                {
                    GuiServer server = getServerFromTreeView((TreeViewItem)selectedItems[i].Parent);

                    if (server != null)
                    {
                        this.ctx.AppendView(server, ((TextBlock)((StackPanel)selectedItems[i].Header).Children[0]).Text);
                    }
                }
            }


        }

        private GuiServer getServerFromTreeView(TreeViewItem item)
        {
            foreach(GuiServer svr in this.ctx.Servers)
            {
                if (svr.TreeItem == item)
                    return svr;
            }

            return null;
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
                string tmp = textInput.Text.Trim();
                if (String.IsNullOrEmpty(tmp))
                    return;

                if (tmp[0] == '/')
                {
                    string[] parts = tmp.Split(' ');
                    if (parts[0] == "/server")
                    {
                        User temp = new User() { Nick = "dabbit", Ident = "dabitp", Name = "David"};



                        this.ctx.AddServer(this.ctx, temp, this.ctx.CreateConnection(ConnectionType.Direct, new WinSocket(parts[1], 1333, true)));
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            this.DataContext = this.ctx;
            userBox.ItemsSource = this.ctx.ActiveUserList;
        }
        
    }



}
