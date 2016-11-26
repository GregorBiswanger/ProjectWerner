using System.Windows.Forms;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public static class SendKeyHelper
    {
        //public static void EnterKeyAltTab()
        //{
        //    SendKeys.SendWait("%{Alt Down}");
        //}

        public static void SwitchApplication()
        {
            SendKeys.SendWait("%+{Tab}");
        }

        //public static void ExitKeyAltTab()
        //{
        //    SendKeys.SendWait("%{Alt Up}");
        //}
    }
}
