using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Reflection.Metadata.BlobBuilder;

namespace KMS_Proxy_Helper
{
    public sealed class ProxyManager
    {
        public bool Active { get; private set; } = false;

        Task? autoCheckTask;
        bool _AutoCheck = false;
        public bool AutoCheck
        {
            get { return _AutoCheck; }
            set
            {
                if (value != _AutoCheck)
                {
                    _AutoCheck = value;
                    if (!_AutoCheck)
                        WriteToLog("Auto check wird deaktivert.");
                    SetAutoCheck();
                }
            }
        }

        string _Host = "";
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        string _Port = "";
        public string Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        TextBlock _log;
        List<string> _Logs = new List<string>();
        const int MAX_Logs = 10;
        const int MAX_DisplayedLogs = 2;

        internal ProxyManager(TextBlock log)
        {
            _log = log;
            Update();

            _log.MouseRightButtonDown += _log_MouseRightButtonDown;

        }
        public void Update()
        {
            SetProxy();
        }

        public void SetProxy(string host, string port)
        {

            _Port = port;
            _Host = host;
            Update();
        }
        public void Reset()
        {
            Active = false;
            AutoCheck = false;

            RegeditHelper.RemoveProxy();
            RegeditHelper.ChangeInternetSettings(0);

            WriteToLog("Proxyeinstellungen wurden zurückgesetzt.");
        }

        public bool IsProxyActive()
        {
            return (RegeditHelper.ReadInternetSettings() == 1 && RegeditHelper.ReadProxyEnableKey() == 1);
        }


        private void _log_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _Logs.Clear();
            _log.Text = "";
            _log.ToolTip = "";
        }




        void SetAutoCheck()
        {
            if (_AutoCheck)
            {
                Active = true;
                autoCheckTask = Task.Factory.StartNew(() => AutoCheckLoop(), TaskCreationOptions.LongRunning);
            }
            else if (autoCheckTask != null)
                autoCheckTask.Dispose();
        }


        void SetProxy(bool writeLog = true)
        {
            if (Host != "")
            {
                Active = true;
                RegeditHelper.ChangeInternetSettings(1);
                RegeditHelper.SetProxy(Host, Port);
                if (writeLog)
                    WriteToLog("Proxy wurde auf " + GetProxyAdress() + " gesetzt");
            }
        }

        string GetProxyAdress()
        {
            string adress = Host;
            if (Port != "")
                adress += ':' + Port;
            return adress;
        }

        async void AutoCheckLoop()
        {
            int entered_id = autoCheckTask.Id;

            WriteToLog("Auto check ist jetzt aktive.");
            while (_AutoCheck && Active && entered_id == autoCheckTask.Id)
            {
                if (!IsProxyActive())
                {
                    SetProxy(false);
                    WriteToLog("Änderung des registry werts wurde erkannt, und automatisch zurückgesetzt.");

                }

                await Task.Delay(2500);
            }
            WriteToLog("Auto check ist jetzt deaktivert.");
        }

        void WriteToLog(string msg)
        {
            string newLog = '[' + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] ";
            newLog += msg;

            _Logs.Add(newLog);
            if (_Logs.Count == MAX_Logs)
                _Logs.RemoveAt(0);


            _log.Dispatcher.BeginInvoke(new Action(() =>
            {
                _log.Text = "";
                for (int i = _Logs.Count - 1; i >= Math.Max(0, _Logs.Count - MAX_DisplayedLogs); i--)
                {
                    if (i != _Logs.Count - 1)
                        _log.Text += Environment.NewLine;

                    _log.Text += _Logs[i];
                }

                _log.ToolTip = "";
                for (int i = _Logs.Count - 1; i >= 0; i--)
                {
                    if (i != _Logs.Count - 1)
                        _log.ToolTip += Environment.NewLine;

                    _log.ToolTip += _Logs[i];
                }
            }));

        }





    }
}
