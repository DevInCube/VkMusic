using My.VKMusic.Models;
using My.VKMusic.NET;
using My.VKMusic.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace My.VKMusic
{
    public class VkAPI
    {

        public static readonly string AppID = "4832975";        

        private string api_version = "5.29";
        private AuthData authData;

        public VkAPI()
        {          
            /* WebClient client = new WebClient();
            var data = client.DownloadString(request_url);*/
        }

        public void DoAuth(Action callback)
        {
            Auth(callback);
        }

        private void Auth(Action callback = null)
        {
            NameValueCollection qs = System.Web.HttpUtility.ParseQueryString(string.Empty);
            qs["client_id"] = AppID;
            qs["scope"] = "status,audio";
            qs["redirect_uri"] = "https://oauth.vk.com/blank.html";
            qs["display"] = "page";
            qs["v"] = api_version;
            qs["response_type"] = "token";
            string request_url = "https://oauth.vk.com/authorize?" + qs.ToString();

            AuthWindow wnd = new AuthWindow();
            wnd.GotAccessToken += (AuthData data) =>
            {
                this.authData = data;
                Timer reAuthTimer = new Timer(data.ExpiresIn * 1000);
                reAuthTimer.Elapsed += reAuthTimer_Elapsed;
                reAuthTimer.Start();
                if (callback != null)
                    callback.Invoke();
            };
            wnd.Open(request_url);
            wnd.Show();
        }

        void reAuthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Auth();
        }

        string CreateMethodRequest(string method_name, NameValueCollection parameters)
        {
            string methodUri = "https://api.vk.com/method/";
            parameters["access_token"] = authData.AccessToken;
            return String.Format("{0}{1}?{2}", methodUri, method_name, parameters.ToString());
        }

        public List<AudioFileInfo> AudioGet()
        {
            var parameters = System.Web.HttpUtility.ParseQueryString(string.Empty);
            parameters["owner_id"] = authData.UserId.ToString();
            parameters["need_user"] = 0.ToString();
            parameters["offset"] = 0.ToString();
            parameters["count"] = 10.ToString();
            string request = CreateMethodRequest("audio.get", parameters);
            WebClient client = new WebClient();
            string response = client.DownloadString(request);
            var j = JObject.Parse(response);
            List<AudioFileInfo> files = new List<AudioFileInfo>();
            long count = j["response"][0].Value<long>();
            for (int i = 0; i < j["response"].Count() - 1; i++)
            {
                JToken audioToken = j["response"][i + 1];
                AudioFileInfo af = AudioFileInfo.FromJson(audioToken);
                files.Add(af);
            }
            return files;
        }

        
    }
}
