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

namespace SMSdisplayer
{
    /// <summary>
    /// Logique d'interaction pour NetworkOptions.xaml
    /// </summary>
    public partial class NetworkOptions : Window
    {
        protected int port;
        private bool success;
        public int Port
        {
            get => port;
        }
        public bool Success { get => success; set => success = value; }

        public NetworkOptions(String address, int port)
        {
            InitializeComponent();

            AddressText.Text = address;
            PortInput.Value = port;
            success = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            port = PortInput.Value.Value;
            success = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            success = false;
            Close();
        }
    }
}
