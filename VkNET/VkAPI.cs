using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using VkNET.Auth;
using VkNET.Exceptions;
using VkNET.Extensions;
using VkNET.Models;

namespace VkNET
{
    public class VkAPI
    {

        public static readonly string AppID = "4832975";        

        private string api_version = "5.29";
        private AuthData authData;
        private IAuthProvider authProvider;

        public VkAPI(IAuthProvider authProvider)
        {
            this.authProvider = authProvider;
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

            authProvider.DoAuth(request_url, (AuthData data) =>
            {
                this.authData = data;
                Timer reAuthTimer = new Timer(data.ExpiresIn * 1000);
                reAuthTimer.Elapsed += reAuthTimer_Elapsed;
                reAuthTimer.Start();
                if (callback != null)
                    callback.Invoke();
            });
        }

        void reAuthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Auth();
        }

        public static AuthData GetAuthData(string url)
        {
            NameValueCollection qscoll = HttpUtility.ParseQueryString(url);
            if (qscoll["error"] != null)
                throw new AccessDeniedVkException(qscoll["error_description"]);
            AuthData data = new AuthData()
            {
                AccessToken = qscoll["#access_token"],
                ExpiresIn = long.Parse(qscoll["expires_in"]),
                UserId = long.Parse(qscoll["user_id"]),
            };
            return data;
        }

        string CreateMethodRequest(string method_name, NameValueCollection parameters)
        {
            string methodUri = "https://api.vk.com/method/";
            parameters["access_token"] = authData.AccessToken;
            return String.Format("{0}{1}?{2}", methodUri, method_name, parameters.ToString());
        }

        NameValueCollection CreateParameters(Dictionary<string, object> dict)
        {
            NameValueCollection parameters = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (string key in dict.Keys)
                parameters[key] = dict[key] == null ? "" : dict[key].ToString();
            return parameters;
        }

        JToken GetJSONResponse(string request)
        {
            WebClient client = new WebClient();
            string response = client.DownloadString(request);
            return JObject.Parse(response);
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
            var res = JObject.Parse(response)["response"];

            List<AudioFileInfo> files = new List<AudioFileInfo>();
            long count = res[0].Value<long>();
            for (int i = 0; i < res.Count() - 1; i++)
            {
                JToken audioToken = res[i + 1];
                AudioFileInfo af = AudioFileInfo.FromJson(audioToken);
                files.Add(af);
            }
            return files;
        }

        public AudioFileInfo AudioGetById(long owner_id, long audio_id)
        {
            var parameters = CreateParameters(new Dictionary<string, object> { 
                {"audios", "{0}_{1}".Format(owner_id, audio_id)}
            });
            string request = CreateMethodRequest("audio.getById", parameters);
            var j = GetJSONResponse(request);
            var res = j["response"];
            if (res.Count() == 0) return null;
            return  AudioFileInfo.FromJson(j["response"][0]);
        }

        public List<AudioFileInfo> AudioSearch(string q)
        {
            var parameters = CreateParameters(new Dictionary<string, object> { 
                {"q", q},
                {"auto_complete", 0},
                {"performer_only", 0},
                {"offset", 0},
                {"count", 10},
            });
            string request = CreateMethodRequest("audio.search", parameters);
            var res = GetJSONResponse(request)["response"];

            List<AudioFileInfo> files = new List<AudioFileInfo>();
            long count = res[0].Value<long>();
            for (int i = 0; i < res.Count() - 1; i++)
            {
                JToken audioToken = res[i + 1];
                AudioFileInfo af = AudioFileInfo.FromJson(audioToken);
                files.Add(af);
            }
            return files;
        }

        
    }
}
