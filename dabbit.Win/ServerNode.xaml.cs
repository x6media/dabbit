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
using dabbit.Base;

namespace dabbit.Win
{
    /// <summary>
    /// Interaction logic for ServerNode.xaml
    /// </summary>
    public partial class ServerNode : UserControl, INotifyPropertyChanged
    {

        public Server server;

        
        public ServerNode()
        {
            InitializeComponent();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void RaisePropertyChanged(string propertyName) 
        { 
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
