using System;
using System.ComponentModel;
using System.Windows;
using EyeXFramework;
using ProjectWerner.Contracts.API;
using ProjectWerner.TobiiEyeTracking.InputSimulation;
using ProjectWerner.TobiiEyeTracking.Logic;
using Tobii.EyeX.Framework;

namespace ProjectWerner.TobiiEyeTracking.API
{
    public class CameraEyeTracking : IEyeTracking
    {
        #region Private members
        private EyeXHost _eyeXHost;
        private GazePointDataStream _gazeStream;
        private EyePositionDataStream _eyePositionDataStream;

        private readonly MouseSimulator _mouseSimulator = new MouseSimulator();
        private readonly PositionFilter _positionfilter = new PositionFilter();

        // todo: Detect if both eyes are closed. Suppress all blink actions if both eyes are involved
        // todo: Use 3D eye positions/movements to detect if the user just moved out of detection
        private readonly EyeBlinkAnalyzer _rightEye = new EyeBlinkAnalyzer();
        private readonly EyeBlinkAnalyzer _leftEye = new EyeBlinkAnalyzer();
        #endregion

        #region Actions
        /// <summary>
        /// when a new eye position is available
        /// </summary>
        public event Action<Point> EyePositionUpdate;

        /// <summary>
        /// when the left eye blinked for several hundert milliseconds
        /// </summary>
        public event Action LeftEyeBlinked;

        /// <summary>
        /// when the right eye blinked for several hundert milliseconds
        /// </summary>
        public event Action RightEyeBlinked;
        #endregion

        // todo: Find a better way to integrate mouse movement and clicks into project Werner
        public bool EnableMouseMove { get; set; }
        public bool EnableLeftMouseClickOnBlinkLeftEye { get; set; }
        public bool EnableRightMouseClickOnBlinkRightEye { get; set; }
        
        public void Initialize()
        {
            // todo: Init and shutdown ALL members, such as _positionFilter, _rightEye etc. to prevent funny effects when re-Initializing

            // Initialize the EyeX Host
            _eyeXHost = new EyeXHost();
            _eyeXHost.Start();
 
            // Listen to gaze data stream
            _gazeStream = _eyeXHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            _gazeStream.Next += GazeStreamOnNext;

            // Listen to eye position data stream
            _eyePositionDataStream = _eyeXHost.CreateEyePositionDataStream();
            _eyePositionDataStream.Next += EyePositionStreamOnNext;

            // set up EyeClick events
            _rightEye.EyeClick += EyeClick_RightEye;
            _leftEye.EyeClick += EyeClick_LeftEye;
        }

        public void ShutDown(CancelEventArgs e)
        {
            _gazeStream.Dispose();
            _gazeStream = null;

            _eyePositionDataStream.Dispose();
            _eyePositionDataStream = null;

            _eyeXHost.Dispose();
            _eyeXHost = null;
        }

        private void EyeClick_RightEye(Point clickPoint)
        {
            OnRightEyeBlinked();

            if (EnableRightMouseClickOnBlinkRightEye)
            {
                _mouseSimulator.MoveMouseTo(clickPoint);
                _mouseSimulator.MouseRightClick();
            }
        }

        private void EyeClick_LeftEye(Point clickPoint)
        {
            OnLeftEyeBlinked();

            if (EnableLeftMouseClickOnBlinkLeftEye)
            {
                _mouseSimulator.MoveMouseTo(clickPoint);
                _mouseSimulator.MouseLeftClick();
            }
        }

        private void EyePositionStreamOnNext(object sender, EyePositionEventArgs e)
        {
            var point = _positionfilter.GetFilteredPosition();
            if (point == null)
                return;

            _rightEye.SetEyeStatus(e.RightEye.IsValid, (Point)point, e.Timestamp);
            _leftEye.SetEyeStatus(e.LeftEye.IsValid, (Point)point, e.Timestamp);
        }

        private void GazeStreamOnNext(object sender, GazePointEventArgs e)
        {
            _positionfilter.AddRawPosition(new Point(e.X, e.Y));

            // todo: only calculate point, if somebody is interested in it
            Point? point = _positionfilter.GetFilteredPosition();
            if (point == null)
                return;
            OnEyePositionUpdate((Point) point);

            if (EnableMouseMove)
            {
                // don't move mouse when eye is closed!
                if (_rightEye.IsEyeClosed())
                    return;
                if (_leftEye.IsEyeClosed())
                    return;

                _mouseSimulator.MoveMouseTo((Point) point);
            }
        }

        /// <summary>
        /// fires when a new eye position is available
        /// </summary>
        /// <param name="point">the position to report</param>
        private void OnEyePositionUpdate(Point point)
        {
            if (EyePositionUpdate == null)
                return;

            EyePositionUpdate.Invoke(point);
        }

        /// <summary>
        /// fires when the left eye blinked for several hundert milliseconds
        /// </summary>
        private void OnLeftEyeBlinked()
        {
            if (LeftEyeBlinked != null)
                LeftEyeBlinked.Invoke();
        }

        /// <summary>
        /// fires when the left eye blinked for several hundert milliseconds
        /// </summary>
        private void OnRightEyeBlinked()
        {
            if (RightEyeBlinked != null)
                RightEyeBlinked.Invoke();
        }
    }
}
