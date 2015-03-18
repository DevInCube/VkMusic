using My.VKMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

namespace My.VKMusic.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public event Action<AuthData> GotAccessToken;

        public MainWindow()
        {
            InitializeComponent();

            this.browser.Navigated += browser_Navigated;
        }

        void browser_Navigated(object sender, NavigationEventArgs e)
        {
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
