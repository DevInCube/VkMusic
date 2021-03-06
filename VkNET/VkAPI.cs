﻿using Newtonsoft.Json.Linq;
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
    public partial class VkAPI
    {

        public static readonly string AppID = "4832975";        

        private string api_version = "5.30";
        private AuthData authData;
        private IAuthProvider authProvider;

        public AuthData AuthData { get { return authData; } }

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
            string permissionsString = "status,audio,video,messages," + Permissions.PHOTOS;
            string[] permissions = permissionsString.Split(',');

            NameValueCollection qs = System.Web.HttpUtility.ParseQueryString(string.Empty);
            qs["client_id"] = AppID;
            qs["scope"] = permissionsString;
            qs["redirect_uri"] = "https://oauth.vk.com/blank.html";
            qs["display"] = "page";
            qs["v"] = api_version;
            qs["response_type"] = "token";
            string request_url = "https://oauth.vk.com/authorize?" + qs.ToString();

            authProvider.DoAuth(request_url, (AuthData data) =>
            {
                this.authData = data;
                this.authData.Permissions = new List<string>(permissions);
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
            data.ExpiresAt = DateTime.Now.AddSeconds(data.ExpiresIn);
            return data;
        }

        string CreateMethodRequest(string method_name, ParametersCollection parameters = null)
        {
            string methodUri = "https://api.vk.com/method/";
            if (parameters == null)
                parameters = new ParametersCollection();
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

        public List<AudioFileInfo> ToList(JToken res)
        {
            List<AudioFileInfo> files = new List<AudioFileInfo>();            
            for (int i = 0; i < res.Count() - 1; i++)
            {
                JObject audioToken = res[i + 1] as JObject;
                AudioFileInfo af = AudioFileInfo.FromJson(audioToken);
                files.Add(af);
            }
            return files;
        }

        public List<AudioFileInfo> AudioGet(int? offset, int? count, out int totalCount)
        {
            var parameters = new ParametersCollection() {
                {"owner_id", authData.UserId},
                {"need_user", 0},
                {"offset", offset},
                {"count", count},
            };
            string request = CreateMethodRequest("audio.get", parameters);
            WebClient client = new WebClient();
            string response = client.DownloadString(request);
            var res = JObject.Parse(response)["response"];
            totalCount = res[0].Value<int>(); //
            return ToList(res);
        }

        public string VideoGet(long owner, string video_ids)
        {
            var parameters = new ParametersCollection() {
                {"owner_id", owner},
                {"videos", video_ids},
            };
            string request = CreateMethodRequest("video.get", parameters);
            WebClient client = new WebClient();
            string response = client.DownloadString(request);
            var res = JObject.Parse(response)["response"];
            return res.ToString();
        }

        public string GetDialogs()
        {
            var offset = 0;
            var count = 20;
            var parameters = new ParametersCollection() {
                {"offset", offset},
                {"count", count},
            };
            string request = CreateMethodRequest("messages.getDialogs", parameters);
            WebClient client = new WebClient();
            string response = client.DownloadString(request);
            var res = JObject.Parse(response)["response"];
            return res.ToString();
        }

        public string GetMessages()
        {
            var offset = 0;
            var count = 200;
            var uid = 20872353;
            var parameters = new ParametersCollection() {
                {"offset", offset},
                {"count", count},
                {"user_id", uid},
            };
            string request = CreateMethodRequest("messages.getHistory", parameters);
            WebClient client = new WebClient();
            string response = client.DownloadString(request);
            var res = JObject.Parse(response)["response"];
            return res.ToString();
        }

        public long AudioGetCount(long owner_id)
        {
            var parameters = new ParametersCollection { 
                {"owner_id", owner_id}
            };
            string request = CreateMethodRequest("audio.getCount", parameters);
            var res = GetJSONResponse(request)["response"];
            return int.Parse(res.Value<string>());
        }

        public void AudioDelete(long audio_id, long owner_id)
        {
            var parameters = new ParametersCollection() {
                {"audio_id", audio_id},
                {"owner_id", owner_id},
            };
            string request = CreateMethodRequest("audio.delete", parameters);
            var res = GetJSONResponse(request)["response"];
            CheckForGlobalError(res);
        }

        public AudioFileInfo AudioGetById(long owner_id, long audio_id)
        {
            var parameters = new ParametersCollection { 
                {"audios", "{0}_{1}".FormatWith(owner_id, audio_id)}
            };
            string request = CreateMethodRequest("audio.getById", parameters);
            var res = GetJSONResponse(request)["response"];            
            if (res.Count() == 0) return null;
            JObject obj = res[0] as JObject;
            return AudioFileInfo.FromJson(obj);
        }

        public string AudioGetLyrics(long lyrics_id)
        {
            var parameters = new ParametersCollection() {
                {"lyrics_id", lyrics_id},
            };
            string request = CreateMethodRequest("audio.getLyrics", parameters);
            var res = GetJSONResponse(request)["response"];
            return res["text"].Value<string>().AsUTF8();
        }

        public List<AudioFileInfo> AudioSearch(string q, AudioSearchSettings settings = null)
        {
            var parameters = new ParametersCollection() {
                {"q", q},
                {"offset", 0},
                {"count", 10},
            };
            parameters = parameters.MergeWith(settings);
            string request = CreateMethodRequest("audio.search", parameters);
            var res = GetJSONResponse(request)["response"];

            return ToList(res);
        }

        public List<AudioFileInfo> AudioGetPopular(AudioPopularSettings settings = null)
        {
            var parameters = new ParametersCollection() {
                {"offset", 0},
                {"count", 10},
            };
            parameters = parameters.MergeWith(settings);
            string request = CreateMethodRequest("audio.getPopular", parameters);
            var res = GetJSONResponse(request)["response"];

            return ToList(res);
        }

        public void AudioReorder(long audio_id, long owner_id, long? before, long? after)
        {
            var parameters = new ParametersCollection() {
                {"audio_id", audio_id},
                {"owner_id", owner_id},
                {"before", before},
                {"after", after},
            };            
            string request = CreateMethodRequest("audio.reorder", parameters);
            var res = GetJSONResponse(request)["response"];
            CheckForGlobalError(res);
        }

        public void AudioSetBroadcast(long audio_id)
        {
            var parameters = new ParametersCollection() {
                {"audio", "{0}_{1}".FormatWith(authData.UserId, audio_id)},                
            };
            string request = CreateMethodRequest("audio.setBroadcast", parameters);
            var res = GetJSONResponse(request)["response"];
            long settingValue = res[0].Value<long>(); //@todo
        }

        public void AudioRemoveBroadcast()
        {
            string request = CreateMethodRequest("audio.setBroadcast");
            var res = GetJSONResponse(request)["response"];
        }

        public List<AudioAlbumInfo> AudioGetAlbums()
        {
            var parameters = new ParametersCollection() {
                {"offset", 0},
                {"count", 10},
            };
            string request = CreateMethodRequest("audio.getAlbums", parameters);
            var res = GetJSONResponse(request)["response"];

            List<AudioAlbumInfo> albums = new List<AudioAlbumInfo>();
            for (int i = 0; i < res.Count() - 1; i++)
            {
                JToken albumToken = res[i + 1];
                AudioAlbumInfo af = AudioAlbumInfo.FromJson(albumToken);
                albums.Add(af);
            }
            return albums;
        }

        public long AudioAddAlbum(string title)
        {
            var parameters = new ParametersCollection() {
                {"title", title},
            };
            string request = CreateMethodRequest("audio.addAlbum", parameters);
            var res = GetJSONResponse(request)["response"];
            return res["album_id"].Value<long>();
        }

        public void AudioDeleteAlbum(long album_id)
        {
            var parameters = new ParametersCollection() {
                {"album_id", album_id},
            };
            string request = CreateMethodRequest("audio.deleteAlbum", parameters);
            var j = GetJSONResponse(request);
            CheckForGlobalError(j);
            var res = j["response"];
            long settingValue = res.Value<long>(); //@todo = 1
        }

        private void CheckForGlobalError(JToken res)
        {
            if (!res.HasValues) return;
            var error = res["error"];
            if (error != null)
            {
                int error_code = error["error_code"].Value<int>();
                string error_msg = error["error_msg"].Value<string>();
                VkException ex = null;
                switch (error_code)
                {
                    case (1): ex = new UnknownVkException(error_msg); break;
                    case (15): ex = new AccessDeniedVkException(error_msg); break;
                    case (200): ex = new AccessToAlbumDeniedVkException(error_msg); break;
                    case (201): ex = new AccessToAudioDeniedVkException(error_msg); break;
                    case (300): ex = new AlbumFullVkException(error_msg); break;
                }
                var request_params = error["request_params"] as JArray;
                foreach (var item in request_params)
                {
                    string key = item["key"].Value<string>();
                    string val = item["value"].Value<string>().AsUTF8();
                    ex.Data[key] = val;
                }
                throw ex;
            }
        }

    }
}
