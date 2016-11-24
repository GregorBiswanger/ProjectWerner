using System;
using System.Windows;
using System.Windows.Input;
using Ninject;
using ProjectWerner.Contracts.API;
using ProjectWerner.ServiceLocator;
using PropertyChanged;

namespace ProjectWerner.Features.Camera3DSimulator
{
    [ImplementPropertyChanged]
    public class Camera3DSimulatorViewModel
    {
        public bool FaceRecognized
        {
            get { return _faceRecognized; }
            set
            {
                if (value)
                {
                    _camera3D.OnFaceVisible(this, new EventArgs());
                }
                else
                {
                    _camera3D.OnFaceLost(this, new EventArgs());
                }

                _faceRecognized = value;
            }
        }
        private bool _faceRecognized;
        private readonly ICamera3D _camera3D;

        public Camera3DSimulatorViewModel()
        {
            _camera3D = MicroKernel.Kernel.Get<ICamera3D>();
            FaceRecognized = true;
        }

        public void MouthOpen()
        {
            _camera3D.OnMouthOpened(this, new EventArgs());
        }

        public void MouthClose()
        {
            _camera3D.OnMouthClosed(this, new EventArgs());
        }
    }
}