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
using System.Windows.Shapes;
using System.Runtime.CompilerServices;

namespace wpfHelixTest
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window
    {
        public string Hostname { get; set; }
        public int Port { get; set; }

        public ConnectionDialog(ProtocolData d)
        {
            InitializeComponent();           

            this.DataContext = d;
            this.PasswordBox.Password = d.Password;

        }

        private void ConnectionOkBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelConnectionBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
