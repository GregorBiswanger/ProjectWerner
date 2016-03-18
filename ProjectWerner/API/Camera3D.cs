using ProjectWerner.Contracts.API;
using System;
using SharpSenses;
using SharpSenses.RealSense.Capabilities;

namespace ProjectWerner.API
{
    public class Camera3D : ICamera3D
    {
        public event Action FaceVisible;
        public event Action FaceLost;
        public event Action MouthOpened;
        public event Action MouthClosed;
        public bool IsFaceMouthOpen { get; set; }

        private readonly ICamera _camera;

        public Camera3D()
        {
            _camera = Camera.Create(Capability.FaceTracking, Capability.FacialExpressionTracking);
            _camera.Face.Visible += OnFaceVisible;
            _camera.Face.NotVisible += OnFaceLost;
            _camera.Face.Mouth.Opened += OnMouthOpened;
            _camera.Face.Mouth.Closed += OnMouthClosed;
            FacialExpressionCapability.MonthOpenThreshold = 30;
            _camera.Start();
        }

        private void OnMouthClosed(object sender, EventArgs e)
        {
            MouthClosed?.Invoke();
        }

        private void OnMouthOpened(object sender, EventArgs e)
        {
            MouthOpened?.Invoke();
        }

        private void OnFaceLost(object sender, EventArgs e)
        {
            FaceLost?.Invoke();
        }

        private void OnFaceVisible(object sender, EventArgs e)
        {
            FaceVisible?.Invoke();
        }

        public void Speech(string message)
        {
            _camera.Speech.Say(message);
        }
    }
}
