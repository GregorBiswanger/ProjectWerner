using System;
using System.Windows.Threading;
using Ninject;
using ProjectWerner.Contracts.API;
using ProjectWerner.ServiceLocator;
using PropertyChanged;
using SharpSenses.Gestures;

namespace ProjectWerner.Features.FacePreviewWindow
{
    [ImplementPropertyChanged]
    public class FacePreviewViewModel
    {
        public bool ArrowRight { get; set; }
        public bool ArrowLeft { get; set; }
        public bool ArrowDown { get; set; }
        public bool ArrowUp { get; set; }
        public bool Error { get; set; }
        public bool FaceLost { get; set; }
        public bool MouthLost { get; set; }
        public bool WindowVisible { get; set; }
        public byte[] ImageStream { get; set; }
        private readonly DispatcherTimer _selectionTimer;
        private int _intervalCount = 0;

        public FacePreviewViewModel()
        {
            var camera3D = MicroKernel.Kernel.Get<ICamera3D>();
            camera3D.MouthMoved += OnMouthMoved;
            camera3D.FaceLost += OnFaceLost;
            camera3D.FaceVisible += OnFaceVisible;
            camera3D.MouthLost += OnMouthLost;
            camera3D.MouthVisible += OnMouthVisible;
            camera3D.NewImageAvailable += UpdateImage;

            _selectionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _selectionTimer.Tick += OnSelectionTimerTick;

            ShowFacePreview();
        }

        private void OnSelectionTimerTick(object sender, EventArgs e)
        {
            if (!Error)
            {
                if (_intervalCount <= 2)
                {
                    _intervalCount++;
                }
                else
                {
                    _intervalCount = 0;
                    WindowVisible = false;
                    _selectionTimer.Stop();
                }
            }
            else
            {
                _intervalCount = 0;
            }
        }

        private void OnMouthMoved(SharpSenses.PositionEventArgs positionEventArgs)
        {
            var x = positionEventArgs.NewPosition.Image.X;
            var y = positionEventArgs.NewPosition.Image.Y;
            var middleX = 300;
            var middleY = 270;
            var limit = 80;

            ArrowRight = false;
            ArrowLeft = false;
            ArrowUp = false;
            ArrowDown = false;
            Error = false;
            
            if (x > middleX + limit)
            {
                ArrowRight = true;
                Error = true;
                ShowFacePreview();
            }
            if (x < middleX - limit)
            {
                ArrowLeft = true;
                Error = true;
                ShowFacePreview();
            }
            if (y > middleY + limit)
            {
                ArrowDown = true;
                Error = true;
                ShowFacePreview();
            }
            if (y < middleX - limit)
            {
                ArrowUp = true;
                Error = true;
                ShowFacePreview();
            }
        }

        private void OnFaceLost()
        {
            FaceLost = true;
            Error = true;
            ShowFacePreview();
        }

        private void ShowFacePreview()
        {
            WindowVisible = true;

            if (!_selectionTimer.IsEnabled)
            {
                _selectionTimer.Start();
            }
        }

        private void OnFaceVisible()
        {
            FaceLost = false;
            Error = false;
        }

        private void OnMouthLost()
        {
            if (!FaceLost)
            {
                MouthLost = true;
                Error = true;
            }
        }

        private void OnMouthVisible()
        {
            if (MouthLost)
            {
                MouthLost = false;
                Error = false;
            }
        }

        private void UpdateImage(byte[] imageToUpdate)
        {
            ImageStream = imageToUpdate;
        }
    }
}
