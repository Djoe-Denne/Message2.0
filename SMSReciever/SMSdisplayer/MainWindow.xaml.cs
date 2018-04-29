using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;
using SMSdisplayer.Controler.Message.Controler.Message;
using SMSdisplayer.Model;
using SMSReceiver;

namespace SMSdisplayer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected MessagesList messages;
        protected MessageBuilder messageBuilder;
        protected MessagesObs messagesObs;
        protected Thread socketThread;
        protected bool fullscreen;

        public MainWindow()
        {
            InitializeComponent();
            messages = new MessagesList();
            messagesList.ItemsSource = Messages;

            messagesObs = new MessagesObs();
            messageBuilder = new MessageBuilder(messages);

            SMSReceiver.AsynchronousSocketListener.AddMessagePushingListener(messagesObs.TrackBuffer);

            messagesObs.Subscribe(messageBuilder);

            socketThread = new Thread(() => SMSReceiver.AsynchronousSocketListener.StartListening());
            socketThread.Start();

            fullscreen = false;
        }

        public MessagesList Messages { get => messages; set => messages = value; }

        private void BackgroundClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            bool? result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                    Stream imageStream = null;
                    if ((imageStream = openFileDialog.OpenFile()) != null)
                    {
                        using (imageStream)
                        {
                            BitmapImage background = new BitmapImage();
                            background.BeginInit();
                            background.StreamSource = imageStream;
                            background.CacheOption = BitmapCacheOption.OnLoad;
                            background.EndInit();
                            background.Freeze();
                            this.Background = new ImageBrush(background);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void FontClick(object sender, RoutedEventArgs e)
        {
            FontChooser fontChooser = new FontChooser();
            fontChooser.Owner = this;

            fontChooser.SetPropertiesFromObject(messagesList);
            fontChooser.PreviewSampleText = "Marie, tu as un trop bon boule !!!";

            if (fontChooser.ShowDialog().Value)
            {
                try
                {
                    fontChooser.ApplyPropertiesToObject(messagesList);
                } catch(Exception)
                {
                    ResourceManager rm = new ResourceManager("SMSdisplayer.ResourcesStr",Assembly.GetExecutingAssembly());
                    MessageBox.Show(this, rm.GetString("FONT_ERROR"), "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void NetworkClick(object sender, RoutedEventArgs e)
        {
            NetworkOptions net = new NetworkOptions(AsynchronousSocketListener.GetAddress().ToString(), (int) AsynchronousSocketListener.Port);

            net.ShowDialog();

            if(net.Success)
            {
                AsynchronousSocketListener.Port = (uint) net.Port;

                AsynchronousSocketListener.Stop();

                socketThread.Join();

                socketThread = new Thread(() => SMSReceiver.AsynchronousSocketListener.StartListening());
                socketThread.Start();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {
                if (!fullscreen)
                {
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    WindowState = WindowState.Maximized;
                    Menu.Visibility = Visibility.Collapsed;
                    fullscreen = true;
                }
                else
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResize;
                    WindowState = WindowState.Normal;
                    Menu.Visibility = Visibility.Visible;
                    fullscreen = false;
                }
            
            }
        }
        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AsynchronousSocketListener.Stop();

            socketThread.Join();
        }
    }
    
}
