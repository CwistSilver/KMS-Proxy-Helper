using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Net.Mime.MediaTypeNames;

namespace KMS_Proxy_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainWindow Instance;
        ProxyManager proxyManager;
        bool isRunning = true;
        BitmapImage? redIcon, icon;
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            
            proxyManager = new ProxyManager(TextBlock_ChangeLog);
            Task.Factory.StartNew(() => CheckState(), TaskCreationOptions.LongRunning);

            LoadSettings();
            AddEvents();
        }
              
        async void CheckState()
        {
            while (isRunning)
            {
                ChangeIndicator();
                await Task.Delay(1500);
            }

        }

        void AddEvents()
        {
            TextBox_Port.PreviewTextInput += HostBox_PreviewTextInput;
            Button_SetProxy.Click += Button_SetProxy_Click;
            Button_Reset.Click += Button_Reset_Click;
            CheckBox_AutoCheck.Click += CheckBox_AutoCheck_Click;
            Closed += MainWindow_Closed;
        }

        void LoadSettings()
        {
            TextBox_Host.Text = Properties.Settings.Default.Host;
            TextBox_Port.Text = Properties.Settings.Default.Port;
            CheckBox_AutoCheck.IsChecked = Properties.Settings.Default.AutoCheck;

            proxyManager.AutoCheck = (bool)CheckBox_AutoCheck.IsChecked;
            proxyManager.Host = TextBox_Host.Text;
            proxyManager.Port = TextBox_Port.Text;

            redIcon = LoadBitmapFromResource("Resources/KMSLogoRed.ico");
            icon = LoadBitmapFromResource("Resources/KMSLogo.ico");
        }

        public static BitmapImage LoadBitmapFromResource(string pathInApplication)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute));
        }

        void SaveSettings()
        {
            Properties.Settings.Default.Host = TextBox_Host.Text;
            Properties.Settings.Default.Port = TextBox_Port.Text;
            Properties.Settings.Default.AutoCheck = (bool)CheckBox_AutoCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            isRunning = false;
            if (proxyManager != null)
                proxyManager.Reset();
        }

        private void CheckBox_AutoCheck_Click(object sender, RoutedEventArgs e)
        {
            proxyManager.AutoCheck = (bool)CheckBox_AutoCheck.IsChecked;
            ChangeIndicator();
            SaveSettings();
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            proxyManager.Reset();
            ChangeIndicator();
        }

        private void HostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_SetProxy_Click(object sender, RoutedEventArgs e)
        {
            Set();
            SaveSettings();
            ChangeIndicator();
        }

        private void Set()
        {
            proxyManager.SetProxy(TextBox_Host.Text, TextBox_Port.Text);
            proxyManager.AutoCheck = (bool)CheckBox_AutoCheck.IsChecked;
        }


        void ChangeIndicator()
        {
            bool isActive = proxyManager.IsProxyActive();



            Instance.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!isActive)
                {
                    StatusIndicator.Fill = Brushes.Red;
                    Instance.Icon = redIcon;
                }
                else
                {
                    StatusIndicator.Fill = Brushes.Green;
                    Instance.Icon = icon;
                }

            }));
        }
    }
}
