using SHDocVw;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
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
                SetSilent(comBrowser, true);              
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

        public static void SetSilent(SHDocVw.WebBrowser browser, bool silent)
        {
            if (browser == null)
                return;

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
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

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }
    }
}
