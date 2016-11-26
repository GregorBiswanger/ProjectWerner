using System;
using System.Management;
using Microsoft.Win32;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public static class RegistryHelper
    {
        public static void ReadAllAvailableSoftware()
        {
            var regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products";
            using (var rk = Registry.LocalMachine.OpenSubKey(regKey))
            {
                foreach (var subKeyName in rk.GetSubKeyNames())
                {
                    var name =
                        Registry.LocalMachine.OpenSubKey(regKey).OpenSubKey(subKeyName).OpenSubKey("InstallProperties")
                            .GetValue("DisplayName");
                }
            }
        }

        public static void Test()
        {
            var computerName = Environment.MachineName;
            ManagementScope scope = new ManagementScope($@"\{computerName}\root\CIMV2");

            scope.Connect();


            var query = new ObjectQuery("SELECT * FROM Win32_Process");
        }


    }
}
