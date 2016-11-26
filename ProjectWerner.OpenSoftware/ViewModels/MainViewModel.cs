using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ProjectWerner.Contracts.API;
using System.Xml;
using System.Globalization;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ProjectWerner.OpenSoftware.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            List< KeyValuePair < string, string>> result = new List<KeyValuePair<string, string>> ();

            result.AddRange(ReadApplications(@"Applications", RegistryHive.ClassesRoot));

            foreach (var VARIABLE in result)
            {
                Console.WriteLine(VARIABLE.Key + "\n" + VARIABLE.Value);
            }
            
            string key = result[17].Value.Split('\"')[1];


            Outlook();

        }

        private void Outlook()
        {
            var outlook = new Outlook.Application();

            Outlook.Application app = new Outlook.Application();
            Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

            mailItem.Subject = "This is the subject";
            mailItem.To = "someone@example.com";
            mailItem.Body = "This is the message.";


            mailItem.Display(true);

        }
        private void Word()
        {
            var word = new Word.Application();

            word.Visible = true;

            var document = word.Documents.Add();
            var paragraph = document.Paragraphs.Add();
            paragraph.Range.Text = "some text";
        }




        public List<KeyValuePair<string, string>> ReadApplications(string registry_key, RegistryHive rh)
        {
            List<KeyValuePair<string, string>> software = new List<KeyValuePair<string, string>>();

            // search in: CurrentUser

            using (var hklm = RegistryKey.OpenBaseKey(rh, RegistryView.Registry64))
            using (Microsoft.Win32.RegistryKey key = hklm.OpenSubKey(registry_key))
            {
                if (key != null)
                    foreach (string keyName in key.GetSubKeyNames())
                    {
                        RegistryKey key8 = key.OpenSubKey(keyName);
                        if (key8 != null)
                            using (Microsoft.Win32.RegistryKey key2 = key8.OpenSubKey("shell"))
                            {
                                if (key2 != null)
                                {
                                    using (Microsoft.Win32.RegistryKey key3 = key2.OpenSubKey("edit"))
                                    {
                                        if (key3 != null)
                                        {
                                            software.Add(new KeyValuePair<string, string>(keyName + " edit", CommandLine(key3)));
                                        }
                                        
                                    }

                                    using (Microsoft.Win32.RegistryKey key4 = key2.OpenSubKey("open"))
                                    {
                                        if (key4 != null)
                                        {
                                            software.Add(new KeyValuePair<string, string>(keyName + " open", CommandLine(key4)));
                                        }
                                        
                                    }
                                }
                            }
                    }
            }

            return software;
        }


        public String CommandLine(RegistryKey command)
        {
            using (Microsoft.Win32.RegistryKey key10 = command.OpenSubKey("command"))
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
