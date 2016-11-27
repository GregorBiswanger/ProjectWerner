using SHDocVw;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace BrowserExtension.Views
{
    /// <summary>
    /// Interaktionslogik für BrowserControll.xaml
    /// </summary>
    public partial class BrowserControl : UserControl
    {
        public int ContentHeight { get; set; }
        public int ScrollPos { get; set; }
        public int ScrollInteravall { get; set; }
        public int ZoomLevel { get; set; }

        object[] zoomLevels = { 20, 40, 60, 80, 100, 120, 140, 160, 180, 200 };

        public BrowserControl()
        {
            InitializeComponent();
            ScrollPos = 0;
            ScrollInteravall = 300;
            ZoomLevel = 5;
            webBrowser.Loaded += (o, s) => HideScriptErrors(webBrowser, true);
        }

        public string URL
        {
            set
            {
                if (!value.StartsWith("http://")&& !value.StartsWith("https://"))
                    value = "http://"+value;
                webBrowser.Source = !string.IsNullOrEmpty(value) ? new Uri(value) : null;
                tbLink.Text = value;
                var comBrowser = GetComBrowser(webBrowser);        
            }
        }

        public void ScaleWebBrobserContent()
        {
            try
            {
                FieldInfo webBrowserInfo = webBrowser.GetType().GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

                object comWebBrowser = null;
                object zoomPercent = zoomLevels[ZoomLevel];
                if (webBrowserInfo != null)
                    comWebBrowser = webBrowserInfo.GetValue(webBrowser);
                if (comWebBrowser != null)
                {
                    InternetExplorer ie = (InternetExplorer)comWebBrowser;
                    ie.ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref zoomPercent, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void HideScriptErrors(System.Windows.Controls.WebBrowser webBrowser, bool hide)
        {
            var fiComWebBrowser = typeof(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null)
            {
                webBrowser.Loaded += (o, s) => HideScriptErrors(webBrowser, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private SHDocVw.WebBrowser GetComBrowser(System.Windows.Controls.WebBrowser webBrowser)
        {
            FieldInfo webBrowserInfo = webBrowser.GetType().GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            object comWebBrowser = null;
            if (webBrowserInfo != null)
                comWebBrowser = webBrowserInfo.GetValue(webBrowser);

            SHDocVw.WebBrowser comBrowser = null;
            if (comWebBrowser != null)
            {
                comBrowser = (SHDocVw.WebBrowser)comWebBrowser;
            }
            return comBrowser;
        }        
    }
}
