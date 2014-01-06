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
    /// Interaction logic for Treeview.xaml
    /// </summary>
    public partial class Treeview : UserControl, INotifyPropertyChanged
    {
        public Treeview()
        {
            InitializeComponent();
            
            //other.server.Name = "Gamer Galaxy";
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
