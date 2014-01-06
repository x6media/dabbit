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
using System.ComponentModel;

namespace dabbit.Win
{
    
    /// <summary>
    /// Interaction logic for ServerChannelNode.xaml
    /// </summary>
    public partial class ServerChannelNode : UserControl, INotifyPropertyChanged
    {
        public string ChannelName { get { return this.channelName; } set { this.RaisePropertyChanged("ChannelName"); this.channelName = value; } }
        private string channelName = "#channel";

        public ServerChannelNode()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private System.Diagnostics.Stopwatch stopwtch = new System.Diagnostics.Stopwatch();
        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (rejoinBtn.Visibility == Visibility.Collapsed)
            {
                rejoinBtn.Visibility = System.Windows.Visibility.Visible;
                closeBtn.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                rejoinBtn.Visibility = System.Windows.Visibility.Collapsed;
                closeBtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void TextBlock_TouchDown(object sender, TouchEventArgs e)
        {
            stopwtch.Reset();
            stopwtch.Start();
        }

        private void TextBlock_TouchUp(object sender, TouchEventArgs e)
        {
            stopwtch.Stop();

            if (stopwtch.ElapsedMilliseconds > 3000)
            {
                if (rejoinBtn.Visibility == Visibility.Collapsed)
                {
                    rejoinBtn.Visibility = System.Windows.Visibility.Visible;
                    closeBtn.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    rejoinBtn.Visibility = System.Windows.Visibility.Collapsed;
                    closeBtn.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
