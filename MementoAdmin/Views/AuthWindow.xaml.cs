using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNET.Models;

namespace MementoAdmin.Views
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
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
            if (String.IsNullOrWhiteSpace(e.Uri.Fragment)) return;
            try
            {
                var data = VkNET.VkAPI.GetAuthData(e.Uri.Fragment);
                this.Close();
                if (GotAccessToken != null)
                    GotAccessToken.Invoke(data);
            }
            catch (VkNET.Exceptions.AccessDeniedVkException ex)
            {
                //@todo
                MessageBox.Show(ex.Message);
                if (GotAccessToken != null)
                    GotAccessToken.Invoke(null);
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
