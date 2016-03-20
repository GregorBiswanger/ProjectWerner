using System.Windows;

namespace ProjectWerner.TobiiEyeTracking.Logic
{
    class EyeBlinkAnalyzer
    {
        public EyeBlinkAnalyzer(int clickThresholdTimeInMilliseconds = 1000)
        {
            _clickThresholdTimeInMilliseconds = clickThresholdTimeInMilliseconds;
        }

        public bool IsEyeClosed()
        {
            return _timestampEyeClosedSince != null;
        }

        /// <summary>
        /// Sets the current eye status
        /// </summary>
        /// <param name="isEyeOpen">The current eye open status</param>
        /// <param name="gazePoint">The current gaze point on the screen, used to determine the correct click position</param>
        /// <param name="timestamp">The timestamp of the eye open status</param>
        public void SetEyeStatus(bool isEyeOpen, Point gazePoint, double timestamp)
        {
            if (isEyeOpen)
            {
                _timestampEyeClosedSince = null;
            }
            else
            {
                if (_timestampEyeClosedSince == null)
                {
                    _timestampEyeClosedSince = timestamp;
                    _clickpoint = gazePoint;
                }
                else if (timestamp - _timestampEyeClosedSince > _clickThresholdTimeInMilliseconds)
                {
                    if (_clickpoint != null)
                        OnEyeClick();

                    _clickpoint = null;
                }
            }
        }


        public delegate void EyeClickEventHandler(Point clickPoint);

        public event EyeClickEventHandler EyeClick;

        protected void OnEyeClick()
        {
            if (EyeClick != null && _clickpoint != null)
            {
                EyeClick((Point)_clickpoint);
            }
        }

        private int _clickThresholdTimeInMilliseconds;
        private double? _timestampEyeClosedSince;
        private Point? _clickpoint;
    }
}
