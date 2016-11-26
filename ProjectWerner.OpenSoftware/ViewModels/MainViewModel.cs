using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Win32;
using ProjectWerner.OpenSoftware.Views;
using PropertyChanged;

namespace ProjectWerner.OpenSoftware.ViewModels
{
    public class MainViewModel
    {
        private readonly List<KeyValuePair<string, string>> _softwareList;

        /// <summary>
        /// empty constructor lists applications
        /// </summary>
        public MainViewModel()
        {
            _softwareList = new List<KeyValuePair<string, string>> ();

            _softwareList.AddRange(ReadApplications(@"Applications", RegistryHive.ClassesRoot));
        }

        /// <summary>
        /// Send text to Open Software
        /// </summary>
        /// <param name="text"></param>
        public void Add(string text)
        {
            foreach (var software in _softwareList.Where(software => software.Key.Contains(text)))
            {
                string command = software.Value;
                if (software.Value.Split('\"').Any())
                {
                    command = software.Value.Split('\"')[1];
                }

                System.Diagnostics.Process.Start(command);
            }
        }



       

        /// <summary>
        /// read all applications
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="rh"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> ReadApplications(string registryKey, RegistryHive rh)
        {
            var software = new List<KeyValuePair<string, string>>();

            using (var hklm = RegistryKey.OpenBaseKey(rh, RegistryView.Registry64))
            using (var key = hklm.OpenSubKey(registryKey))
            {
                if (key == null) return software;
                foreach (var keyName in key.GetSubKeyNames())
                {
                    var key8 = key.OpenSubKey(keyName);
                    if (key8 == null) continue;

                    using (var key2 = key8.OpenSubKey("shell"))
                    {
                        if (key2 == null) continue;
                        using (var key3 = key2.OpenSubKey("edit"))
                        {
                            if (key3 != null)
                            {
                                software.Add(new KeyValuePair<string, string>(keyName.ToLower(), CommandLine(key3)));
                            }
                        }

                        using (var key4 = key2.OpenSubKey("open"))
                        {
                            if (key4 != null)
                            {
                                software.Add(new KeyValuePair<string, string>(keyName.ToLower(), CommandLine(key4)));
                            }
                        }
                    }
                }
            }

            return software;
        }


        /// <summary>
        /// read command value
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CommandLine(RegistryKey command)
        {
            using (var key10 = command.OpenSubKey("command"))
            {
                if (key10 != null)
                {
                    return key10.GetValue("").ToString();
                }
            }
            return "";
        }

        
    }
}
