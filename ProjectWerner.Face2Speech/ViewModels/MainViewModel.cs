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
using ProjectWerner.Face2Speech.Functions;
using ProjectWerner.ServiceLocator;
using ProjectWerner.Face2Speech.Models;

namespace ProjectWerner.Face2Speech.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public CultureInfo selectedCulture { get; set; }
        public bool AcitvateFirstProspalWord { get; set; }

        public string DisplayText { get; set; }
        public ObservableCollection<Line> KeyboardLines { get; set; }
        public int SelectedKeyboardLineIndex { get; set; }
        public ObservableCollection<LanguageDictionary> AllLanguages { get; set; }

        public LanguageDictionary SelectedLanguage { get; set; }
        public ObservableCollection<WordDictionary> AllWords { get; set; }
        public ObservableCollection<WordDictionary> ProposalWords { get; set; }
        public WordDictionary SelectedProposalWord { get; set; }
        public int SelectedProposalWordIndex { get; set; }

        //Gesichterkennung
        public bool LostFace { get; set; }
        public bool MouthOpen { get; set; }
        public bool MouthClosed { get; set; }

        [Import]
        private ICamera3D _camera3D;
        private DispatcherTimer _dispatcherTimer;
        private readonly int _intervalSeconds = 1; // Werner hat 3 Sek.

        public MainViewModel()
        {
            LoadConfig();

            DictionaryManager myReadDictionary = new DictionaryManager();
            //ProposalWords = new ObservableCollection<Words>();

            KeyboardLines = myReadDictionary.LoadKeyboardDictionary(selectedCulture);

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                myReadDictionary.LoadAllWords(selectedCulture);
                SelectedLanguage = myReadDictionary.AllLanguages[0];
                AllWords = SelectedLanguage.Words;
            }
            else
            {
                AllWords = new ObservableCollection<WordDictionary>();
            }

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dispatcherTimer = new DispatcherTimer();
                _dispatcherTimer.Tick += OnInterval;
                _dispatcherTimer.Interval = new TimeSpan(0, 0, _intervalSeconds);

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
            
            _dispatcherTimer.Start();

            // http://pinvoke.net/default.aspx/kernel32/SetThreadExecutionState.html
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void LoadConfig()
        {
            AcitvateFirstProspalWord = true;
            DisplayText = string.Empty;
            SelectedKeyboardLineIndex = -1;
            selectedCulture = CultureInfo.CurrentCulture;
            //selectedCulture = "fr-FR";
        }



        private void OnMouthOpened()
        {
            MouthClosed = false;
            MouthOpen = true;
        }

        private void OnMouthClosed()
        {
            if (SelectedKeyboardLineIndex != -1)
            {
                if (KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex != -1 &&
                    KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex != KeyboardLines[SelectedKeyboardLineIndex].Words.Count)
                {
                    KeyboardLines[SelectedKeyboardLineIndex].Words[KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex].IsActive = false;
                }
            }

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

        private bool _multiSelectActive = true;
        private int _multiSelectCount = 0;
        private bool _lineSelected;

        private void OnInterval(object sender, EventArgs e)
        {
            if (_waitForConfirmation)
            {
                WaitForConfirmation();
            }
            else if (_multiSelectCount == 8)
            {
                KeyboardLines.ToList().ForEach(line => line.IsSelected = false);
                SelectedKeyboardLineIndex = -1;

                if (_camera3D.IsFaceMouthOpen)
                {
                    _multiSelectCount = 0;
                }
            }
            else if (_multiSelectActive)
            {
                if (_camera3D.IsFaceMouthOpen)
                {
                    if (KeyboardLines[0].IsSelected)
                    {
                        KeyboardLines.ToList().ForEach(line => line.IsSelected = false);

                        SelectedKeyboardLineIndex = 0;
                    }
                    else
                    {
                        KeyboardLines.ToList().ForEach(line => line.IsSelected = false);

                        SelectedKeyboardLineIndex = KeyboardLines.Count / 2;
                    }

                    _multiSelectActive = false;
                    _multiSelectCount = 0;
                }
                else if (KeyboardLines[0].IsSelected)
                {
                    KeyboardLines.ToList().ForEach(line => line.IsSelected = false);

                    int halfCount = KeyboardLines.Count / 2;
                    for (int i = halfCount; i < KeyboardLines.Count; i++)
                    {
                        KeyboardLines[i].IsSelected = true;
                    }

                    _multiSelectCount = _multiSelectCount + 1;
                }
                else
                {
                    KeyboardLines.ToList().ForEach(line => line.IsSelected = false);

                    int halfCount = KeyboardLines.Count / 2;
                    for (int i = 0; i < halfCount; i++)
                    {
                        KeyboardLines[i].IsSelected = true;
                    }

                    _multiSelectCount = _multiSelectCount + 1;
                }
            }
            else
            {
                if (!_lineSelected && !_camera3D.IsFaceMouthOpen)
                {
                    if (SelectedKeyboardLineIndex == KeyboardLines.Count - 1 ||
                        (KeyboardLines.Count - 1) / 2 == SelectedKeyboardLineIndex)
                    {
                        SelectedKeyboardLineIndex = -1;
                        _multiSelectActive = true;
                    }
                    else
                    {
                        SelectedKeyboardLineIndex = SelectedKeyboardLineIndex + 1;
                    }
                }
                else if (!_lineSelected &&
                    SelectedKeyboardLineIndex != -1 &&
                    _camera3D.IsFaceMouthOpen)
                {
                    _lineSelected = true;
                    KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex = 0;
                    SetProposalWordIndex();
                }
                else if (_lineSelected)
                {
                    if (KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex == KeyboardLines[SelectedKeyboardLineIndex].Words.Count)
                    {
                        if (SelectedKeyboardLineIndex == KeyboardLines.Count)
                        {
                            SelectedKeyboardLineIndex = -1;
                        }
                        else
                        {
                            KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex = -1;
                            _lineSelected = false;

                            if (SelectedKeyboardLineIndex <= (KeyboardLines.Count - 1) / 2)
                            {
                                SelectedKeyboardLineIndex = -1;
                            }
                            else
                            {
                                SelectedKeyboardLineIndex = (KeyboardLines.Count) / 2;
                            }
                        }
                    }
                    else
                    {
                        if (_camera3D.IsFaceMouthOpen)
                        {
                            KeyboardLines[SelectedKeyboardLineIndex].Words[KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex].IsActive = true;
                            _waitForConfirmation = true;
                        }
                        else
                        {
                            KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex = KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex + 1;
                            SetProposalWordIndex();
                        }
                    }
                }
            }
        }

        private bool _waitForConfirmation = false;

        private void WaitForConfirmation()
        {
            _dispatcherTimer.Stop();

            Thread.Sleep(new TimeSpan(0, 0, _intervalSeconds));
            if (_camera3D.IsFaceMouthOpen)
            {
                KeyboardLines[SelectedKeyboardLineIndex].Words[KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex].IsActive = false;
                SetTextActivity(KeyboardLines[SelectedKeyboardLineIndex].Words[KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex]);
                KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex = -1;
                _lineSelected = false;
                SelectedKeyboardLineIndex = -1;
                _multiSelectActive = true;
                _multiSelectCount = 0;
            }

            _waitForConfirmation = false;
            _dispatcherTimer.Start();
        }

        private void SetProposalWordIndex()
        {
            if (KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex == KeyboardLines[SelectedKeyboardLineIndex].Words.Count)
            {
                SelectedProposalWordIndex = -1;
            }
            else
            {
                string text = KeyboardLines[SelectedKeyboardLineIndex].Words[KeyboardLines[SelectedKeyboardLineIndex].SelectedWordIndex].Text;
                int result;
                if (int.TryParse(text, out result))
                {
                    if (result == 0)
                    {
                        SelectedProposalWordIndex = 9;
                    }
                    else
                    {
                        SelectedProposalWordIndex = (result - 1);
                    }
                }
                else
                {
                    SelectedProposalWordIndex = -1;
                }
            }
        }

        public void OnButtonClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            Button button = (Button)sender;

            SetTextActivity((KeyboardChars)button.Tag);
        }

        private void SetTextActivity(KeyboardChars Chars)
        {
            ProspalWords myProspalWords = new ProspalWords();

            Dictionary<String, String> AddedWords = new Dictionary<string, string>();
            int result = 0;
            string[] text;
            if (Chars.Type == "delete")
            {
                if (DisplayText.Length > 0)
                {
                    DisplayText = DisplayText.Remove(DisplayText.Length - 1, 1);
                    text = DisplayText.Split(' ');
                    ProposalWords = myProspalWords.GetFirstLines(AllWords, text[text.Length - 1], ProspalWords.SearchType.All);
                }
            }
            else if (Chars.Type == "enter")
            {
                DisplayText = DisplayText + "\n";
            }
            else if (Chars.Type == "speak")
            {
                _camera3D.Speech(DisplayText);
            }
            else if (Chars.Type == "space")
            {
                DisplayText = DisplayText + " ";
                ProposalWords.Clear();
            }
            else if (Chars.Type == "mark")
            {
                DisplayText = DisplayText.Trim() + Chars.Text + " ";
            }
            else if (int.TryParse(Chars.Text, out result) || Chars.Type == "ok")
            {
                if (int.TryParse(Chars.Text, out result)) {
                    int clickedNumber = int.Parse(Chars.Text);
                    if (clickedNumber == 0)
                    {
                        clickedNumber = 10;
                    }
                    if (clickedNumber - 1 <= ProposalWords.Count)
                    {
                        SelectedProposalWordIndex = clickedNumber - 1;
                    }
                }
                if (SelectedProposalWord != null)
                {
                    text = DisplayText.Split(' ');
                    if (text[text.Length - 1] != SelectedProposalWord.Text.Substring(3))
                    {
                        if (int.TryParse(Chars.Text, out result))
                        {
                            if (text[text.Length - 1] == "")
                            {
                                text[text.Length - 1] = text[text.Length - 2].Replace(text[text.Length - 2], SelectedProposalWord.Text.Substring(3)) + " ";
                            } else
                            {
                                text[text.Length - 1] = text[text.Length - 1].Replace(text[text.Length - 1], SelectedProposalWord.Text.Substring(3)) + " ";
                            }

                            DisplayText = string.Join(" ", text);
                        }
                        else
                        {
                            DisplayText = DisplayText + SelectedProposalWord.Text.Substring(3) + " ";
                        }
                            
                    }
                    else
                    {
                        DisplayText = DisplayText + " ";
                    }
                    if (SelectedProposalWord.NextWords != null)
                    {
                        //List<String> MyNextWords = SelectedProposalWord.NextWords;
                        //myProspalWords.Number = 0;
                        //ProposalWords.Clear();
                        //foreach (String NextWord in MyNextWords)
                        //{
                        //    foreach (WordDictionary myWords in myProspalWords.GetFirstLines(AllWords, NextWord, ProspalWords.SearchType.OnlyEqual))
                        //    {
                        //        ProposalWords.Add(myWords);
                        //    }
                        //}
                    }
                    else
                    {
                        SelectedProposalWordIndex = -1;
                        myProspalWords.Number = 0;
                        ProposalWords.Clear();
                    }

                }
            }
            else
            {
                DisplayText = DisplayText + Chars.Text;
                string lastWord = DisplayText.Trim().Split(' ').Last();
                ProposalWords = myProspalWords.GetFirstLines(AllWords, lastWord, ProspalWords.SearchType.All);
            }
            //if (ProposalWords.Count > 0 && AcitvateFirstProspalWord)
            //{
            //    SelectedProposalWordIndex = 0;
            //}
            //else
            //{
            //    SelectedProposalWordIndex = -1;
            //}
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