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

namespace dabbit.Win
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : UserControl
    {

        public ChatWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuItem mnuItem1 = new MenuItem();
            mnuItem1.Header = "New Package";
            MenuItem mnuItem2 = new MenuItem();
            mnuItem2.Header = "Show Package Details";
            MenuItem mnuItem3 = new MenuItem();
            mnuItem3.Header = "Edit Package";
            MenuItem mnuItem4 = new MenuItem();
            mnuItem4.Header = "Delete Package";
            MenuItem mnuItem5 = new MenuItem();
            mnuItem5.Header = "Add to Queue";

            ContextMenu menu = new ContextMenu() { };
            menu.Items.Add(mnuItem1);
            menu.Items.Add(mnuItem2);
            menu.Items.Add(mnuItem3);
            menu.Items.Add(mnuItem4);
            menu.Items.Add(mnuItem5);
            ((Hyperlink)sender).ContextMenu = menu;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Rectangle)sender).Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;

        }

    }
}
