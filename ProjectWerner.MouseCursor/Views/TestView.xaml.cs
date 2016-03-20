namespace ProjectWerner.MouseCursor.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Runtime.InteropServices;
    using System.Windows.Threading;
    using ProjectWerner.Contracts.Extensions;
    using ProjectWerner.MouseCursor.Utils;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : IAppExtension
    {
        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CursorPoint
        {
            public int X;
            public int Y;

            public static implicit operator Point(CursorPoint point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, Input[] inputs, int sizeOfInputStructure);
        
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out CursorPoint lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        //[DllImport("user32.dll")]
        //static extern int GetSystemMetrics(SystemMetric smIndex);

        private static readonly int DefaultXDelta = 5;
        private static readonly int DefaultYDelta = 5;
        private static readonly int DefaultTick = 250;
        private DispatcherTimer timer;
        private Action action;

        public TestView()
        {
            this.InitializeComponent();
            this.timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(DefaultTick),
                DispatcherPriority.Send,
                (s, e) => this.SendMouseMovement(),
                Dispatcher.CurrentDispatcher);
        }

        public UserControl AppUserControl => this;

        private void SendMouseMovement()
        {
            //var input = new Input();
            //input.Type = (uint)Utils.InputType.Mouse;
            //input.Data.Mouse = new MouseInput();
            //input.Data.Mouse.Flags = (uint) MouseFlag.MOUSEEVENTF_ABSOLUTE | (uint) MouseFlag.MOUSEEVENTF_MOVE;
            //input.Data.Mouse.X = this.CalculateAbsoluteCoordinateX(5);
            //input.Data.Mouse.Y = this.CalculateAbsoluteCoordinateY(20);
            //input.Data.Mouse.Time = 0;
            //input.Data.Mouse.ExtraInfo = IntPtr.Zero;
            //var inputList = new Input[1];
            //inputList[0] = input;
            //var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof(Input)));

            CursorPoint oldPoint;
            GetCursorPos(out oldPoint);
            switch (this.action)
            {
                case Action.Left:
                    SetCursorPos(oldPoint.X -= DefaultXDelta, oldPoint.Y);
                    break;
                case Action.Right:
                    SetCursorPos(oldPoint.X += DefaultXDelta, oldPoint.Y);
                    break;
                case Action.Up:
                    SetCursorPos(oldPoint.X, oldPoint.Y += DefaultYDelta);
                    break;
                case Action.Down:
                    SetCursorPos(oldPoint.X, oldPoint.Y += DefaultYDelta);
                    break;
            }
        }

        //enum SystemMetric
        //{
        //    SM_CXSCREEN = 0,
        //    SM_CYSCREEN = 1,
        //}

        //int CalculateAbsoluteCoordinateX(int x)
        //{
        //    return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        //}

        //int CalculateAbsoluteCoordinateY(int y)
        //{
        //    return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        //}

        enum Action
        {
            None,
            Left,
            Right,
            Up,
            Down
        }

        private void ChangeAction(Action newAction) => this.action = newAction;


        private void LeftButton_OnTouchDown(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.Down);

        private void LeftButton_OnTouchUp(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.None);

        private void LeftButton_OnClick(object sender, RoutedEventArgs e)
            => this.ChangeAction(Action.Down);


        private void RightButton_TouchDown(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.Right);

        private void RightButton_TouchUp(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.None);

        private void RightButton_OnClick(object sender, RoutedEventArgs e)
            => this.ChangeAction(Action.Right);


        private void UpButton_TouchDown(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.Up);

        private void UpButton_TouchUp(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.None);

        private void UpButton_OnClick(object sender, RoutedEventArgs e)
            => this.ChangeAction(Action.Up);


        private void DownButton_OnTouchDown(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.Down);

        private void DownButton_TouchUp(object sender, TouchEventArgs e)
            => this.ChangeAction(Action.None);

        private void DownButton_OnClick(object sender, RoutedEventArgs e)
            => this.ChangeAction(Action.Down);


        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
            => this.ChangeAction(Action.None);
    }
}
