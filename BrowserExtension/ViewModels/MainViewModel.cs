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
using PropertyChanged;
using System.Xml;
using System.Globalization;
using ProjectWerner.ServiceLocator;
using BrowserExtension.Models;
using System.Windows.Input;
using BrowserExtension.Helper;
using BrowserExtension.JsonModels;
using BrowserExtension.Enums;
using BrowserExtension.Views;

namespace BrowserExtension.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public int ButtonsSelectedIndex { get; set; }
        public int SearchResultsSelectedIndex { get; set; }
        public List<ButtonItem> Buttons { get; set; }
        public List<ListItem> SearchResults { get; set; }
        public UserControl CurrentContentControl { get; set; }

        //Gesichterkennung
        public bool LostFace { get; set; }
        public bool MouthOpen { get; set; }
        public bool MouthClosed { get; set; }

        [Import]
        private ICamera3D _camera3D;
        private DispatcherTimer _buttonsDispatcherTimer;
        private DispatcherTimer _searchResultsDispatcherTimer;
        private readonly int _intervalSeconds = 2; // Werner hat 2 Sek.

        public MainViewModel(string searchFor)
        {
            var completeSearchView = new CompleteSearchView();
            CurrentContentControl = new CompleteSearchView();
            System.Net.HttpWebRequest request = BingSearchHelper.BuildRequest(searchFor, 7, 0, SearchLanguage.German, SafeSearchFilter.Moderate);
            System.Net.HttpWebResponse response = BingSearchHelper.GetSearchResponse(request);
            List<value> searchResults = BingSearchHelper.GetSearchResultsFrom<List<value>>(response);

            SearchResults = new List<ListItem>();
            foreach (var result in searchResults)
            {
                var listItem = new ListItem()
                {
                    SearchResult = result,
                    IsActive = false,
                    Command = OpenSiteCommand
                };
                SearchResults.Add(listItem);
            }
            SearchResults[0].IsActive = true;

            ButtonsSelectedIndex = 0;
            SearchResultsSelectedIndex = 0;
            Buttons = new List<ButtonItem>();
            Buttons.Add(new ButtonItem() { Label = "Zurück", IsActive = true, Command = BackToSearchCommand });
            Buttons.Add(new ButtonItem() { Label = "Hoch", IsActive = false, Command = ScrollUpCommand });
            Buttons.Add(new ButtonItem() { Label = "Runter", IsActive = false, Command = ScrollDownCommand });
            Buttons.Add(new ButtonItem() { Label = "Zoom +", IsActive = false, Command = ZoomInCommand });
            Buttons.Add(new ButtonItem() { Label = "Zoom -", IsActive = false, Command = ZoomOutCommand });
            //Buttons.Add(new ButtonItem() { Label = "List Links Button", IsActive = false, Command = BackToSearchCommand });

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _searchResultsDispatcherTimer = new DispatcherTimer();
                _searchResultsDispatcherTimer.Tick += OnSearchResultsInterval;
                _searchResultsDispatcherTimer.Interval = new TimeSpan(0, 0, _intervalSeconds);
                _searchResultsDispatcherTimer.Start();

                _buttonsDispatcherTimer = new DispatcherTimer();
                _buttonsDispatcherTimer.Tick += OnButtonsInterval;
                _buttonsDispatcherTimer.Interval = new TimeSpan(0, 0, _intervalSeconds);

                _camera3D = MicroKernel.Get<ICamera3D>();
                _camera3D.MouthOpened += OnMouthOpened;
                _camera3D.MouthClosed += OnMouthClosed;
                _camera3D.FaceVisible += OnFaceDetected;
                _camera3D.FaceLost += OnFaceLost;
                //_camera3D.Connected += Connected;
            }
        }

        private void Connected()
        {
            _searchResultsDispatcherTimer.Start();
            //_buttonsDispatcherTimer.Start();

            // http://pinvoke.net/default.aspx/kernel32/SetThreadExecutionState.html
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
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
            //_searchResultsDispatcherTimer.Start();
            //_buttonsDispatcherTimer.Start();
        }

        private void OnFaceLost()
        {
            LostFace = true;
            //_searchResultsDispatcherTimer.Stop();
            //_buttonsDispatcherTimer.Stop();
        }

        private void OnSearchResultsInterval(object sender, EventArgs e)
        {
            if (MouthOpen)
            {
                _waitForConfirmationInSearchResults = true;
            }
            if (_waitForConfirmationInSearchResults)
            {
                WaitForConfirmationInSearchResults();
            }

            SearchResults[SearchResultsSelectedIndex].IsActive = false;
            if (SearchResultsSelectedIndex < (SearchResults.Count - 1))
            {
                SearchResultsSelectedIndex++;
            }
            else
            {
                SearchResultsSelectedIndex = 0;
            }
            SearchResults[SearchResultsSelectedIndex].IsActive = true;
        }

        private void OnButtonsInterval(object sender, EventArgs e)
        {
            if (MouthOpen)
            {
                _waitForConfirmationInButtons = true;
            }
            if (_waitForConfirmationInButtons)
            {
                WaitForConfirmationInButtons();
            }

            Buttons[ButtonsSelectedIndex].IsActive = false;
            if (ButtonsSelectedIndex < (Buttons.Count - 1))
            {
                ButtonsSelectedIndex++;
            }
            else
            {
                ButtonsSelectedIndex = 0;
            }
            Buttons[ButtonsSelectedIndex].IsActive = true;
        }

        private bool _waitForConfirmationInSearchResults = false;
        private void WaitForConfirmationInSearchResults()
        {
            _searchResultsDispatcherTimer.Stop();

            Thread.Sleep(new TimeSpan(0, 0, _intervalSeconds));
            if (_camera3D.IsFaceMouthOpen)
            {
                SearchResults[SearchResultsSelectedIndex].Command.Execute(null);
            }
            {
                _waitForConfirmationInSearchResults = false;
                _searchResultsDispatcherTimer.Start();
            }
        }

        private bool _waitForConfirmationInButtons = false;
        private void WaitForConfirmationInButtons()
        {
            _buttonsDispatcherTimer.Stop();

            Thread.Sleep(new TimeSpan(0, 0, _intervalSeconds));
            if (_camera3D.IsFaceMouthOpen)
            {
                Buttons[ButtonsSelectedIndex].Command.Execute(null);
            }

            _waitForConfirmationInButtons = false;
            _buttonsDispatcherTimer.Start();
        }

        ICommand _openSiteCommandCommand;
        public ICommand OpenSiteCommand
        {
            get
            {
                if (_openSiteCommandCommand == null)
                {
                    _openSiteCommandCommand = new RelayCommand(p => this.OpenSite());
                }
                return _openSiteCommandCommand;
            }
        }

        private void OpenSite()
        {
            var browserControl = new BrowserControl();
            browserControl.URL = SearchResults[SearchResultsSelectedIndex].SearchResult.displayUrl;
            SearchResults[SearchResultsSelectedIndex].IsActive = false;
            SearchResultsSelectedIndex = 0;
            CurrentContentControl = browserControl;

            _searchResultsDispatcherTimer.Stop();
            Application.Current.Dispatcher.Invoke(() => {
                CurrentContentControl = browserControl;
            });
            Thread.Sleep(new TimeSpan(0, 0, 2));

            _buttonsDispatcherTimer.Start();
        }

        ICommand _backToMainCommand;
        public ICommand BackToMainCommand
        {
            get
            {
                if (_backToMainCommand == null)
                {
                    _backToMainCommand = new RelayCommand(p => this.BackToMain());
                }
                return _backToMainCommand;
            }
        }

        private void BackToMain()
        {

        }

        ICommand _backToSearchCommand;
        public ICommand BackToSearchCommand
        {
            get
            {
                if (_backToSearchCommand == null)
                {
                    _backToSearchCommand = new RelayCommand(p => this.BackToSearchResults());
                }
                return _backToSearchCommand;
            }
        }

        private void BackToSearchResults()
        {
            _buttonsDispatcherTimer.Stop();
            Buttons[ButtonsSelectedIndex].IsActive = false;
            ButtonsSelectedIndex = 0;
            Application.Current.Dispatcher.Invoke(() => {
                CurrentContentControl = new CompleteSearchView();
            });

            _searchResultsDispatcherTimer.Start();
            OnMouthClosed();
            Thread.Sleep(new TimeSpan(0, 0, 2));
        }

        ICommand _scrollUpCommand;
        public ICommand ScrollUpCommand
        {
            get
            {
                if (_scrollUpCommand == null)
                {
                    _scrollUpCommand = new RelayCommand(p => this.ScrollUp());
                }
                return _scrollUpCommand;
            }
        }

        private void ScrollUp()
        {
            var browserControl = CurrentContentControl as BrowserControl;

            if (browserControl != null)
            {
                if (browserControl.ScrollPos > browserControl.ScrollInteravall)
                    browserControl.ScrollPos -= browserControl.ScrollInteravall;
                else if (browserControl.ScrollPos <= browserControl.ScrollInteravall)
                    browserControl.ScrollPos = 0;

                var html = browserControl.webBrowser.Document as mshtml.HTMLDocument;
                if (html != null)
                {
                    html.parentWindow?.scroll(0, browserControl.ScrollPos);
                }
            }
        }

        ICommand _scrollDownCommand;
        public ICommand ScrollDownCommand
        {
            get
            {
                if (_scrollDownCommand == null)
                {
                    _scrollDownCommand = new RelayCommand(p => this.ScrollDown());
                }
                return _scrollDownCommand;
            }
        }

        private void ScrollDown()
        {
            var browserControl = CurrentContentControl as BrowserControl;

            var html = browserControl?.webBrowser?.Document as mshtml.HTMLDocument;
            if (html != null)
            {
                var elem = html.activeElement as mshtml.IHTMLElement2;
                browserControl.ContentHeight = elem.scrollHeight;

                if (browserControl.ScrollPos < browserControl.ContentHeight)
                    browserControl.ScrollPos += browserControl.ScrollInteravall;
                else
                    browserControl.ScrollPos = browserControl.ContentHeight;

                html.parentWindow.scroll(0, browserControl.ScrollPos);
            }

        }

        ICommand _zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                if (_zoomInCommand == null)
                {
                    _zoomInCommand = new RelayCommand(p => this.ZoomIn());
                }
                return _zoomInCommand;
            }
        }

        private void ZoomIn()
        {
            var browserControl = CurrentContentControl as BrowserControl;
            if (browserControl != null)
            {
                if (browserControl.ZoomLevel < 9)
                    browserControl.ZoomLevel++;
                browserControl.ScaleWebBrobserContent();
            }
        }

        ICommand _zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                if (_zoomOutCommand == null)
                {
                    _zoomOutCommand = new RelayCommand(p => this.ZoomOut());
                }
                return _zoomOutCommand;
            }
        }

        private void ZoomOut()
        {
            var browserControl = CurrentContentControl as BrowserControl;
            if (browserControl != null)
            {
                if (browserControl.ZoomLevel > 0)
                    browserControl.ZoomLevel--;
                browserControl.ScaleWebBrobserContent();
            }
        }

        //http://pinvoke.net/default.aspx/kernel32/SetThreadExecutionState.html
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
    }
}
