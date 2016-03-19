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
using ProjectWerner.ServiceLocator;
using PropertyChanged;

namespace ProjectWerner.Face2Speech.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public ObservableCollection<Line> Lines { get; set; }

        public ObservableCollection<string> ProposalWords { get; set; }

        public string Text { get; set; }

        public int SelectedLineIndex { get; set; }

        public int SelecredProposalWordIndex { get; set; }

        public string SelecredProposalWord { get; set; }

        public bool LostFace { get; set; }

        public bool MouthOpen { get; set; }
        public bool MouthClosed { get; set; }

        private readonly ICamera3D _camera3D;
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly int _intervalSeconds = 1; // Werner hat 3 Sek.

        string[] _words;


        public MainViewModel()
        {
            Lines = new ObservableCollection<Line>();
            ProposalWords = new ObservableCollection<string>();
            Text = string.Empty;
            SelectedLineIndex = -1;

            Line lineA = new Line();
            lineA.Words.Add(new Word { Text = "delete" });
            lineA.Words.Add(new Word { Text = "a" });
            lineA.Words.Add(new Word { Text = "i" });
            lineA.Words.Add(new Word { Text = "c" });
            lineA.Words.Add(new Word { Text = "b" });
            lineA.Words.Add(new Word { Text = "e" });
            lineA.Words.Add(new Word { Text = "g" });

            Line lineB = new Line();
            lineB.Words.Add(new Word { Text = "t" });
            lineB.Words.Add(new Word { Text = "s" });
            lineB.Words.Add(new Word { Text = "d" });
            lineB.Words.Add(new Word { Text = "f" });
            lineB.Words.Add(new Word { Text = "m" });
            lineB.Words.Add(new Word { Text = "k" });
            lineB.Words.Add(new Word { Text = "j" });

            Line lineC = new Line();
            lineC.Words.Add(new Word { Text = "w" });
            lineC.Words.Add(new Word { Text = "l" });
            lineC.Words.Add(new Word { Text = "h" });
            lineC.Words.Add(new Word { Text = "n" });
            lineC.Words.Add(new Word { Text = "p" });
            lineC.Words.Add(new Word { Text = "q" });
            lineC.Words.Add(new Word { Text = "x" });

            Line lineD = new Line();
            lineD.Words.Add(new Word { Text = "o" });
            lineD.Words.Add(new Word { Text = "y" });
            lineD.Words.Add(new Word { Text = "r" });
            lineD.Words.Add(new Word { Text = "u" });
            lineD.Words.Add(new Word { Text = "v" });
            lineD.Words.Add(new Word { Text = "z" });
            lineD.Words.Add(new Word { Text = "es" });

            Line lineE = new Line();
            lineE.Words.Add(new Word { Text = "1" });
            lineE.Words.Add(new Word { Text = "2" });
            lineE.Words.Add(new Word { Text = "3" });
            lineE.Words.Add(new Word { Text = "4" });
            lineE.Words.Add(new Word { Text = "5" });
            lineE.Words.Add(new Word { Text = "6" });
            lineE.Words.Add(new Word { Text = "space" });

            Line lineF = new Line();
            lineF.Words.Add(new Word { Text = "7" });
            lineF.Words.Add(new Word { Text = "8" });
            lineF.Words.Add(new Word { Text = "9" });
            lineF.Words.Add(new Word { Text = "0" });
            lineF.Words.Add(new Word { Text = "." });
            lineF.Words.Add(new Word { Text = "," });
            lineF.Words.Add(new Word { Text = "X" });

            Line lineG = new Line();
            lineG.Words.Add(new Word { Text = "X" });
            lineG.Words.Add(new Word { Text = "X" });
            lineG.Words.Add(new Word { Text = "X" });
            lineG.Words.Add(new Word { Text = "X" });
            lineG.Words.Add(new Word { Text = "X" });
            lineG.Words.Add(new Word { Text = "X" });
            lineG.Words.Add(new Word { Text = "X" });

            Line lineH = new Line();
            lineH.Words.Add(new Word { Text = "enter" });
            lineH.Words.Add(new Word { Text = "#" });
            lineH.Words.Add(new Word { Text = "speak" });
            lineH.Words.Add(new Word { Text = "X" });
            lineH.Words.Add(new Word { Text = "X" });
            lineH.Words.Add(new Word { Text = "X" });
            lineH.Words.Add(new Word { Text = "X" });

            Lines.Add(lineA);
            Lines.Add(lineB);
            Lines.Add(lineC);
            Lines.Add(lineD);
            Lines.Add(lineE);
            Lines.Add(lineF);
            Lines.Add(lineG);
            Lines.Add(lineH);

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _camera3D = MicroKernel.Get<ICamera3D>();
                _camera3D.MouthOpened += OnMouthOpened;
                _camera3D.MouthClosed += OnMouthClosed;
                _camera3D.FaceVisible += OnFaceDetected;
                _camera3D.FaceLost += OnFaceLost;

                _words = File.ReadAllLines("Extensions\\Face2Speech\\Dictionary\\german.txt", Encoding.UTF8);

                _dispatcherTimer = new DispatcherTimer();
                _dispatcherTimer.Tick += OnInterval;
                _dispatcherTimer.Interval = new TimeSpan(0, 0, _intervalSeconds);
                _dispatcherTimer.Start();

                // http://pinvoke.net/default.aspx/kernel32/SetThreadExecutionState.html
                SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            }
        }

        private void OnMouthOpened()
        {
            MouthClosed = false;
            MouthOpen = true;
        }

        private void OnMouthClosed()
        {
            if (SelectedLineIndex != -1)
            {
                if (Lines[SelectedLineIndex].SelectedWordIndex != -1 &&
                    Lines[SelectedLineIndex].SelectedWordIndex != Lines[SelectedLineIndex].Words.Count)
                {
                    Lines[SelectedLineIndex].Words[Lines[SelectedLineIndex].SelectedWordIndex].IsActive = false;
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
                Lines.ToList().ForEach(line => line.IsSelected = false);
                SelectedLineIndex = -1;

                if (_camera3D.IsFaceMouthOpen)
                {
                    _multiSelectCount = 0;
                }
            }
            else if (_multiSelectActive)
            {
                if (_camera3D.IsFaceMouthOpen)
                {
                    if (Lines[0].IsSelected)
                    {
                        Lines.ToList().ForEach(line => line.IsSelected = false);

                        SelectedLineIndex = 0;
                    }
                    else
                    {
                        Lines.ToList().ForEach(line => line.IsSelected = false);

                        SelectedLineIndex = Lines.Count / 2;
                    }

                    _multiSelectActive = false;
                    _multiSelectCount = 0;
                }
                else if (Lines[0].IsSelected)
                {
                    Lines.ToList().ForEach(line => line.IsSelected = false);

                    int halfCount = Lines.Count / 2;
                    for (int i = halfCount; i < Lines.Count; i++)
                    {
                        Lines[i].IsSelected = true;
                    }

                    _multiSelectCount = _multiSelectCount + 1;
                }
                else
                {
                    Lines.ToList().ForEach(line => line.IsSelected = false);

                    int halfCount = Lines.Count / 2;
                    for (int i = 0; i < halfCount; i++)
                    {
                        Lines[i].IsSelected = true;
                    }

                    _multiSelectCount = _multiSelectCount + 1;
                }
            }
            else
            {
                if (!_lineSelected && !_camera3D.IsFaceMouthOpen)
                {
                    if (SelectedLineIndex == Lines.Count - 1 ||
                        (Lines.Count - 1) / 2 == SelectedLineIndex)
                    {
                        SelectedLineIndex = -1;
                        _multiSelectActive = true;
                    }
                    else
                    {
                        SelectedLineIndex = SelectedLineIndex + 1;
                    }
                }
                else if (!_lineSelected &&
                    SelectedLineIndex != -1 &&
                    _camera3D.IsFaceMouthOpen)
                {
                    _lineSelected = true;
                    Lines[SelectedLineIndex].SelectedWordIndex = 0;
                    SetProposalWordIndex();
                }
                else if (_lineSelected)
                {
                    if (Lines[SelectedLineIndex].SelectedWordIndex == Lines[SelectedLineIndex].Words.Count)
                    {
                        if (SelectedLineIndex == Lines.Count)
                        {
                            SelectedLineIndex = -1;
                        }
                        else
                        {
                            Lines[SelectedLineIndex].SelectedWordIndex = -1;
                            _lineSelected = false;

                            if (SelectedLineIndex <= (Lines.Count - 1) / 2)
                            {
                                SelectedLineIndex = -1;
                            }
                            else
                            {
                                SelectedLineIndex = (Lines.Count) / 2;
                            }
                        }
                    }
                    else
                    {
                        if (_camera3D.IsFaceMouthOpen)
                        {
                            Lines[SelectedLineIndex].Words[Lines[SelectedLineIndex].SelectedWordIndex].IsActive = true;
                            _waitForConfirmation = true;
                        }
                        else
                        {
                            Lines[SelectedLineIndex].SelectedWordIndex = Lines[SelectedLineIndex].SelectedWordIndex + 1;
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
                Lines[SelectedLineIndex].Words[Lines[SelectedLineIndex].SelectedWordIndex].IsActive = false;
                SetTextActivity(Lines[SelectedLineIndex].Words[Lines[SelectedLineIndex].SelectedWordIndex].Text);
                Lines[SelectedLineIndex].SelectedWordIndex = -1;
                _lineSelected = false;
                SelectedLineIndex = -1;
                _multiSelectActive = true;
                _multiSelectCount = 0;
            }

            _waitForConfirmation = false;
            _dispatcherTimer.Start();
        }

        private void SetProposalWordIndex()
        {
            if (Lines[SelectedLineIndex].SelectedWordIndex == Lines[SelectedLineIndex].Words.Count)
            {
                SelecredProposalWordIndex = -1;
            }
            else
            {
                string text = Lines[SelectedLineIndex].Words[Lines[SelectedLineIndex].SelectedWordIndex].Text;
                int result;
                if (int.TryParse(text, out result))
                {
                    if (result == 0)
                    {
                        SelecredProposalWordIndex = 9;
                    }
                    else
                    {
                        SelecredProposalWordIndex = (result - 1);
                    }
                }
                else
                {
                    SelecredProposalWordIndex = -1;
                }
            }
        }

        public void OnButtonClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            Button button = (Button)sender;

            SetTextActivity(button.Tag.ToString());
        }

        private void SetTextActivity(string content)
        {
            int result = 0;

            if (content == "delete")
            {
                if (Text.Length > 0)
                {
                    Text = Text.Remove(Text.Length - 1, 1);
                    SetProposalWords();
                }
            }
            else if (content == "space")
            {
                Text = Text + " ";
                ProposalWords.Clear();
            }
            else if (content == "enter")
            {
                Text = Text + "\n";
                ProposalWords.Clear();
            }
            else if (content == "speak")
            {
                _camera3D.Speech(Text);
            }
            else if (int.TryParse(content, out result))
            {
                if (SelecredProposalWord != null)
                {
                    string[] text = Text.Split(' ');
                    text[text.Length - 1] = SelecredProposalWord.Replace(result + ": ", "") + " ";
                    Text = string.Join(" ", text);
                    SelecredProposalWordIndex = -1;
                    ProposalWords.Clear();
                }
            }
            else
            {
                Text = Text + content;

                SetProposalWords();
            }
        }

        private void SetProposalWords()
        {
            ProposalWords.Clear();

            if (!string.IsNullOrEmpty(Text) ||
                !string.IsNullOrWhiteSpace(Text))
            {
                string lastWord = Text.Split(' ').Last();
                IEnumerable<string> proposalWords =
                    _words.Where(word => word.ToUpper().StartsWith(lastWord.ToUpper())).OrderBy(x => x.Length).Take(10);
                int number = 0;
                proposalWords.ToList().ForEach(word =>
                {
                    number = number + 1;
                    if (number == 10)
                    {
                        number = 0;
                    }
                    ProposalWords.Add(string.Format("{0}: {1}", number, word));
                });
            }

            SelecredProposalWordIndex = -1;
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