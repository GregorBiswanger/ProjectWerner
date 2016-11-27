using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using ProjectWerner.Contracts.API;
using ProjectWerner.OpenSoftware.Views;
using ProjectWerner.ServiceLocator;
using PropertyChanged;

namespace ProjectWerner.OpenSoftware.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : INotifyPropertyChanged
    {
        public int SelectedIndex { get; set; }

        //Gesichterkennung
        public bool LostFace { get; set; }
        public bool MouthOpen { get; set; }
        public bool MouthClosed { get; set; }

        [Import]
        private ICamera3D _camera3D;
        private DispatcherTimer _dispatcherTimer;
        private readonly int _intervalSeconds = 3; // Werner hat 3 Sek.

        public List<KeyValuePair<string, string>> SoftwareList { get; set; }

        /// <summary>
        /// empty constructor lists applications
        /// </summary>
        public MainViewModel()
        {
            SoftwareList = new List<KeyValuePair<string, string>> ();

            SoftwareList.AddRange(ReadApplications(@"Applications", RegistryHive.ClassesRoot));

            SelectedIndex = 0;
        }

        public void Start()
        {
            string text = Clipboard.GetText();

            text = text.ToLower();

            foreach (var software in SoftwareList)
            {
                if (software.Key.Contains(text))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(software.Value);
                        return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dispatcherTimer = new DispatcherTimer();
                _dispatcherTimer.Tick += OnInterval;
                _dispatcherTimer.Interval = new TimeSpan(0, 0, _intervalSeconds);
                _dispatcherTimer.Start();

                _camera3D = MicroKernel.Get<ICamera3D>();
                _camera3D.MouthOpened += OnMouthOpened;
                _camera3D.MouthClosed += OnMouthClosed;
                _camera3D.FaceVisible += OnFaceDetected;
                _camera3D.FaceLost += OnFaceLost;
                //_camera3D.Connected += Connected;
            }
        }

        private void OnMouthOpened()
        {
            MouthClosed = false;
            MouthOpen = true;
        }

        private void OnMouthClosed()
        {
            MouthClosed = true;
            MouthOpen = false;
        }

        private void OnFaceDetected()
        {
            LostFace = false;
            _dispatcherTimer.Start();
        }

        private void OnFaceLost()
        {
            LostFace = true;
            _dispatcherTimer.Stop();
        }

        private void OnInterval(object sender, EventArgs e)
        {
            if (_waitForConfirmation)
            {
                WaitForConfirmation();
            }
            else if (MouthOpen)
            {
                _waitForConfirmation = true;
            }
            else
            {
                if (SelectedIndex < SoftwareList.Count - 1)
                {
                    SelectedIndex++;

                }
                else
                {
                    SelectedIndex = 0;

                }

                OnPropertyChanged("SelectedIndex");
            }
            
        }

        private bool _waitForConfirmation = false;
        private void WaitForConfirmation()
        {
            _dispatcherTimer.Stop();

            Thread.Sleep(new TimeSpan(0, 0, _intervalSeconds));

            if (_camera3D.IsFaceMouthOpen)
            {
                try
                {
                    System.Diagnostics.Process.Start(SoftwareList[SelectedIndex].Value);
                }
                catch (Exception)
                {
                    _waitForConfirmation = false;
                    _dispatcherTimer.Start();
                }
            }
            else
            {
                _waitForConfirmation = false;
                _dispatcherTimer.Start();
            }
            

            //  
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



                        var key3 = key2.OpenSubKey("edit");

                        if (key3 != null)
                        {
                            string command = CommandLine(key3);

                            if (command.StartsWith("\""))
                            {
                                command = command.Split('\"')[1];
                            }
                            else
                            {
                                command = command.Split('\"')[0];
                            }

                            if (!(String.IsNullOrEmpty(command) || String.IsNullOrWhiteSpace(command)))
                            {
                                software.Add(new KeyValuePair<string, string>(keyName.ToLower().Replace(".exe", ""),
                                    command));
                            }


                        }
                        else
                        {
                            using (var key4 = key2.OpenSubKey("open"))
                            {
                                if (key4 != null)
                                {
                                    string command = CommandLine(key4);
                                    if (command.StartsWith("\""))
                                    {
                                        command = command.Split('\"')[1];
                                    }
                                    else 
                                    {
                                        command = command.Split('\"')[0];
                                    }

                                    if (!(String.IsNullOrEmpty(command) || String.IsNullOrWhiteSpace(command)))
                                    {
                                        software.Add(new KeyValuePair<string, string>(
                                        keyName.ToLower().Replace(".exe", ""), command));
                                    }

                                    
                                }
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


      
    
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            var handler = System.Threading.Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

    }
}
