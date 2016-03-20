using System.Runtime.InteropServices;
using System.Windows;
using WindowsInput;

namespace ProjectWerner.TobiiEyeTracking.InputSimulation
{
    class MouseSimulator
    {
        // todo: use native methods to remove NuGet dependency on InputSimulator
        readonly InputSimulator _inputSimulator = new InputSimulator();

        public void MoveMouseTo(Point point)
        {
            SetCursorPos((int)point.X, (int)point.Y);
        }

        public void MouseLeftClick()
        {
            _inputSimulator.Mouse.LeftButtonClick();
        }

        public void MouseRightClick()
        {
            _inputSimulator.Mouse.RightButtonClick();
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);
    }
}
