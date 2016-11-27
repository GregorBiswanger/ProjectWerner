using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using ProjectWerner.Contracts.API;
using ProjectWerner.EmailClient.Views;
using ProjectWerner.ServiceLocator;
using PropertyChanged;

namespace ProjectWerner.EmailClient.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : INotifyPropertyChanged
    {
        public UserControl Content { get; set; }

        public int SelectedIndex { get; set; }
        public List<ButtonItem> Buttons { get; set; }

        //Gesichterkennung
        public bool LostFace { get; set; }
        public bool MouthOpen { get; set; }
        public bool MouthClosed { get; set; }

        [Import] private ICamera3D _camera3D;
        private DispatcherTimer _dispatcherTimer;
        private readonly int _intervalSeconds = 3; // Werner hat 3 Sek.

        public MainViewModel()
        {
           
        }

        public void Start()
        {
            SelectedIndex = 0;
            Buttons = new List<ButtonItem>();
            Buttons.Add(new ButtonItem() { Label = "Write", IsActive = true });
            Buttons.Add(new ButtonItem() { Label = "Read", IsActive = false });

            OnPropertyChanged("Buttons");

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
                OnPropertyChanged("Buttons");
                OnPropertyChanged("SelectedIndex");
            }

            
            //else
            //{
            //}
        }

        private bool _waitForConfirmation = false;

        private void WaitForConfirmation()
        {
            _dispatcherTimer.Stop();

            Thread.Sleep(new TimeSpan(0, 0, _intervalSeconds));
            if (_camera3D.IsFaceMouthOpen)
            {
                switch (SelectedIndex)
                {
                    case 0:
                        Content = new WriteMailView();
                        OnPropertyChanged("Content");
                        break;
                    case 1:
                        Content = new ReadMailView();
                        OnPropertyChanged("Content");
                        break;
                }
            }
            else
            {

                _waitForConfirmation = false;
                _dispatcherTimer.Start();
            }

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