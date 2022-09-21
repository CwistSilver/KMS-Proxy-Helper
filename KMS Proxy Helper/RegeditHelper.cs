using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS_Proxy_Helper
{
    internal static class RegeditHelper
    {

        internal static void ChangeInternetSettings(int newValue)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            string keyName = "ProxySettingsPerUser";
            //adding/editing a value 
            key.SetValue(keyName, newValue);
            key.Close();
        }

        internal static int ReadInternetSettings()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings", true);

            //getting the value
            int data = int.Parse(key.GetValue("ProxySettingsPerUser").ToString());  //returns the text found in 'someValue'

            key.Close();
            return data;
        }

        internal static string ReadCurrentUserKeyValue(in string path, in string keyName)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);

            //getting the value
            string data = key.GetValue(keyName).ToString();  //returns the text found in 'someValue'

            key.Close();
            return data;
        }

        internal static string ReadLocalMachineKeyValue(in string path, in string keyName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true);

            //getting the value
            string data = key.GetValue(keyName).ToString();  //returns the text found in 'someValue'

            key.Close();
            return data;
        }

        internal static int ReadProxyEnableKey()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);

            key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            string ProxyEnableName = "ProxyEnable";
            //adding/editing a value 
            int data = int.Parse(key.GetValue(ProxyEnableName).ToString());
            key.Close();

            return data;

        }

        internal static void SetProxy(string host, string port)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            string ProxyServerName = "ProxyServer";
            //adding/editing a value 
            string host_port = host;
            if (port != "")
                host_port += ':' + port;

            key.SetValue(ProxyServerName, host_port);
            key.Close();

            key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            string ProxyEnableName = "ProxyEnable";
            //adding/editing a value 
            key.SetValue(ProxyEnableName, 1);
            key.Close();
        }

        internal static void RemoveProxy()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            string ProxyServerName = "ProxyServer";
            //adding/editing a value 
            key.SetValue(ProxyServerName, "");
            key.Close();

            key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            string ProxyEnableName = "ProxyEnable";
            //adding/editing a value 
            key.SetValue(ProxyEnableName, 0);
            key.Close();
        }
    }

}
