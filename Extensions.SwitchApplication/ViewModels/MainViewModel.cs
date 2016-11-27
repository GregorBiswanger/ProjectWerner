using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using ProjectWerner.Contracts.API;
using ProjectWerner.ServiceLocator;
using ProjectWerner.SwitchApplication.Interfaces;
using ProjectWerner.SwitchApplication.Model;
using ProjectWerner.SwitchApplication.Utilities;

namespace ProjectWerner.SwitchApplication.ViewModels
{
    public class MainViewModel
    {
        private readonly Dictionary<string, IProcessInfo> currentProcessInfos;

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        public int SelectedIndex { get; set; }
        public ObservableCollection<ButtonItem> Buttons { get; set; }

        //Gesichterkennung
        public bool LostFace { get; set; }
        public bool MouthOpen { get; set; }
        public bool MouthClosed { get; set; }

        [Import]
        private ICamera3D camera3D;
        private readonly DispatcherTimer dispatcherTimer;
        private readonly int intervalSeconds = 3; // Werner hat 3 Sek.


        public MainViewModel()
        {
            SelectedIndex = 0;
            currentProcessInfos = new Dictionary<string, IProcessInfo>();
            Buttons = new ObservableCollection<ButtonItem>();
            var allCurrentlyRunningProcesses = WmiHelper.GetAllCurrentlyRunningProcesses();
            var counter = 0;
            foreach (var processInfo in allCurrentlyRunningProcesses)
            {
                var name = CreateValidName(processInfo.FileName);

                if (!IsProcessAvailable(processInfo.ProcessId))
                    continue;

                if (currentProcessInfos.ContainsKey(name) == false)
                    currentProcessInfos.Add(name, processInfo);

                Buttons.Add(new ButtonItem
                {
                    ProcessInfo = processInfo,
                    Command = new RelayCommand(p => this.ExecuteProcess(processInfo.ProcessId)),
                    IsActive = counter == 0,
                    Label = name
                });
                counter++;
            }

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnInterval;
            dispatcherTimer.Interval = new TimeSpan(0, 0, intervalSeconds);
            dispatcherTimer.Start();

            camera3D = MicroKernel.Get<ICamera3D>();
            camera3D.MouthOpened += OnMouthOpened;
            camera3D.MouthClosed += OnMouthClosed;
            camera3D.FaceVisible += OnFaceDetected;
            camera3D.FaceLost += OnFaceLost;
            //_camera3D.Connected += Connected;
            
        }

        private void Connected()
        {
            dispatcherTimer.Start();

            // http://pinvoke.net/default.aspx/kernel32/SetThreadExecutionState.html
            SetThreadExecutionState(ExecutionState.EsDisplayRequired | ExecutionState.EsContinuous);
            SetThreadExecutionState(ExecutionState.EsContinuous);
        }

        private static string CreateValidName(string fileName)
        {
            var process = Process.GetProcessesByName(fileName);
            if (process.Length > 0)
            {
                return process[0].MainWindowTitle;
            }
            var name = fileName.Substring(0, fileName.IndexOf(".exe", StringComparison.InvariantCultureIgnoreCase));
            return name;
        }


        private bool IsProcessAvailable(int processId)
        {
            var process = Process.GetProcessById(processId);
            return !string.IsNullOrEmpty(process?.ProcessName);
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
            dispatcherTimer.Start();
        }

        private void OnFaceLost()
        {
            LostFace = true;
            dispatcherTimer.Stop();
        }

        private void OnInterval(object sender, EventArgs e)
        {
            Buttons[SelectedIndex].IsActive = false;
            if (SelectedIndex < Buttons.Count - 1)
            {
                SelectedIndex++;
            }
            else
            {
                SelectedIndex = 0;
            }
            Buttons[SelectedIndex].IsActive = true;

            if (waitForConfirmation)
            {
                WaitForConfirmation();
            }
            else if (MouthOpen)
            {
                waitForConfirmation = true;
            }
            //else
            //{
            //}
        }

        private bool waitForConfirmation = false;
        private void WaitForConfirmation()
        {
            dispatcherTimer.Stop();

            Thread.Sleep(new TimeSpan(0, 0, intervalSeconds));
            if (camera3D.IsFaceMouthOpen)
            {
                Buttons[SelectedIndex].Command.Execute(null);
            }

            waitForConfirmation = false;
            dispatcherTimer.Start();
        }

        private void ExecuteProcess(int processId)
        {
            var process = Process.GetProcessById(processId);
            
            SetForegroundWindow(FindWindowByCaption(IntPtr.Zero, process.MainWindowTitle));
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            SendKeys.SendWait("%{TAB}");
        }

        //http://pinvoke.net/default.aspx/kernel32/SetThreadExecutionState.html
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
    }
}
