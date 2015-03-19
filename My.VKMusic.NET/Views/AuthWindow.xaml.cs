using My.VKMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {

        private bool isSilent = false;
        public event Action<AuthData> GotAccessToken;

        public AuthWindow()
        {
            InitializeComponent();
            
            this.browser.Navigated += browser_Navigated;
        }

        void browser_Navigated(object sender, NavigationEventArgs e)
        {
            if (!isSilent)
            {
                SetSilent(browser, true);
            }
            NameValueCollection qscoll = HttpUtility.ParseQueryString(e.Uri.Fragment);
            string stoken = qscoll["#access_token"];
            if (!String.IsNullOrWhiteSpace(stoken))
            {
                AuthData data = new AuthData()
                {
                    AccessToken = qscoll["#access_token"],
                    ExpiresIn = long.Parse(qscoll["expires_in"]),
                    UserId = long.Parse(qscoll["user_id"]),
                };
                this.Close();
                if (GotAccessToken != null)
                    GotAccessToken.Invoke(data);
            }
        }


        public static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

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

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }

        public void Open(string url)
        {
            this.browser.Source = new Uri(url);
        }

        public void ShowPage(string html)
        {
            this.browser.NavigateToString(html);
        }
    }
}
