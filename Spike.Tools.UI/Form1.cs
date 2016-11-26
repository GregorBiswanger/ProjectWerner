using ProjectWerner.SwitchApplication.Utilities;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace Spike.Tools.UI
{
    public partial class Form1 : Form
    {
        private bool isCanceled;
        private readonly DispatcherTimer dispatcherTimer;
        private int interval = 0;

        public Form1()
        {
            InitializeComponent();
            isCanceled = false;
            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };

            dispatcherTimer.Tick += OnSeletionTimerTick;
            dispatcherTimer.Start();
        }

        ~Form1()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= OnSeletionTimerTick;
        }

        private void OnSeletionTimerTick(object sender, EventArgs e)
        {
            if (interval <= 4)
            {
                interval++;
            }
            else
            {
                interval = 0;
                dispatcherTimer.Stop();

                KeyboardHelper.AltTab();
                //SendKeyHelper.SwitchApplication();

                var modifiers = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyToggled(Key.RightAlt);
                if (modifiers)
                {
                    // Keydown: Alt left or Alt right
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        WindowHelper.ShowWindow(this.Handle, WindowHelper.SW_RESTORE);
                    }
                    else
                    {
                        WindowHelper.SetForegroundWindow(this.Handle);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //do
            //{
            //    if (this.WindowState == FormWindowState.Minimized)
            //    {
            //        WindowHelper.ShowWindow(this.Handle, WindowHelper.SW_RESTORE);
            //    }
            //    else
            //    {
            //        WindowHelper.SetForegroundWindow(this.Handle);
            //    }


            //    KeyboardHelper.AltTab();
            //    Thread.Sleep(5 * 1000);
            //    if (isCanceled)
            //    {
            //        break;
            //    }
            //} while (isCanceled == false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isCanceled = true;
        }
    }
}
