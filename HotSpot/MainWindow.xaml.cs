using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotSpot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AsyncServer asyncServer;
        private Thread Server;
        private bool Connected = false;

        public MainWindow()
        {
            InitializeComponent();
            asyncServer = new AsyncServer(LogBox);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!Connected)
            {
                Server = new Thread(() => asyncServer.StartListening("Server has been Connected"));
                Server.Start();
                Connected = true;
            }
            else
            {
                LogBox.Text = "Server is already Connected and Running...";
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (Connected)
            {
                Server.Abort();
                Connected = false;
            }
            else
            {
                LogBox.Text = "Server is already Disconnected...";
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (Connected)
            {
                Server.Abort();
                Server = new Thread(() => asyncServer.StartListening("Server has been Disconnected.\nServer has been Restarted."));
                Server.Start();
            }
            else
            {
                Server = new Thread(() => asyncServer.StartListening("Server has been Restarted"));
                Server.Start();
                Connected = true;
            }
        }
        
    }
}
