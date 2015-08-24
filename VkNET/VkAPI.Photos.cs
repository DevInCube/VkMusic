using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNET.Exceptions;
using VkNET.Models;
using VkNET.Models.Photos;

namespace VkNET
{
    public partial class VkAPI
    {

        private void CheckPermissions(params string[] permissions)
        {
            foreach(var permission in permissions) {
                if(!this.authData.Permissions.Contains(permission))
                    throw new PermissionException(permission);
            }
        }

        public string GetAlbums_Json()
        {
            CheckPermissions(Permissions.PHOTOS);
            var owner_id = this.authData.UserId;
            var offset = 0;
            var count = 20;
            var parameters = new ParametersCollection() {
                {"owner_id", owner_id},
                {"offset", offset},
                {"count", count},
                {"need_covers", 1},
            };
            string request = CreateMethodRequest("photos.getAlbums", parameters);
            JToken res = GetJSONResponse(request);            
            return res.ToString();
        }

        public IList<PhotoAlbumInfo> Photos_GetAlbums()
        {
            string res = GetAlbums_Json();
            JObject data = JObject.Parse(res);
            JArray response = data["response"].Value<JArray>();
            JEnumerable<JToken> children = response.Children();
            PhotoAlbumInfo[] albums = new PhotoAlbumInfo[children.Count()];
            for (int i = 0; i < children.Count(); i++)
            {
                JToken child = children.ElementAt(i);
                albums[i] = PhotoAlbumInfo.FromJson((JObject)child);
            }
            return albums;
        }

        public string PhotosGet_JSON(PhotoAlbumInfo album)
        {
            var offset = 0;
            var count = 1000;
            var parameters = new ParametersCollection() {
                {"owner_id", album.OwnerId},
                {"album_id", album.Id},
                {"offset", offset},
                {"count", count},
                {"photo_sizes", 1},
            };
            string request = CreateMethodRequest("photos.get", parameters);
            JToken res = GetJSONResponse(request);
            return res.ToString();
        }

        public IList<PhotoInfo> Photos_Get(PhotoAlbumInfo album)
        {
            string res = PhotosGet_JSON(album);
            JObject data = JObject.Parse(res);
            JArray response = data["response"].Value<JArray>();
            JEnumerable<JToken> children = response.Children();
            PhotoInfo[] photos = new PhotoInfo[children.Count()];
            for (int i = 0; i < children.Count(); i++)
            {
                JToken child = children.ElementAt(i);
                photos[i] = PhotoInfo.FromJson((JObject)child);
            }
            return photos;
        }

        public string Photos_GetUploadServer(PhotoAlbumInfo album)
        {
            var parameters = new ParametersCollection() {                
                {"album_id", album.Id},               
            };
            string request = CreateMethodRequest("photos.getUploadServer", parameters);
            JToken res = GetJSONResponse(request);
            JObject response = res["response"].Value<JObject>();
            string upload_url = response["upload_url"].Value<string>();
            return upload_url;
        }

        public IList<PhotoInfo> Photos_Save(PhotoUploadResult pre)
        {
            IList<PhotoInfo> photos = new List<PhotoInfo>();           
            var parameters = new ParametersCollection() {                
                {"album_id", pre.AlbumId},
                {"server", pre.Server},
                {"photos_list", pre.PhotosList},
                {"hash", pre.Hash},
            };
            string request = CreateMethodRequest("photos.save", parameters);
            JToken res = GetJSONResponse(request);
            JArray response = res["response"].Value<JArray>();
            foreach (var p in response)
                photos.Add(PhotoInfo.FromJson((JObject)p));
            return photos;
        }

        public IList<PhotoInfo> UploadPhoto(string filePath, string uploadUrl)
        {            
            string fileName = Path.GetFileName(filePath);
            string fileExt = Path.GetExtension(fileName);

            // Read file data
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("filename", fileName);
            postParameters.Add("fileformat", fileExt);
            postParameters.Add("file", new FormUpload.FileParameter(data, fileName, "image"));

            // Create request and receive response
            string postURL = uploadUrl;
            string userAgent = "Memento";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters);

            // Process response
            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            webResponse.Close();

            var preRes = PhotoUploadResult.FromJson(JObject.Parse(fullResponse));
            return Photos_Save(preRes);
        }

        public void Photos_Delete(PhotoInfo photo)
        {         
            var parameters = new ParametersCollection() {                
                {"owner_id", photo.OwnerId},
                {"photo_id", photo.Id},             
            };
            string request = CreateMethodRequest("photos.delete", parameters);
            JToken res = GetJSONResponse(request);
            if (res["response"].Value<int>() != 1)
            {
                throw new UnknownVkException("Photo remove returned " + res.ToString());
            }
        }

        public void Photos_MakeCover(PhotoInfo photo)
        {
            var parameters = new ParametersCollection() {                
                {"owner_id", photo.OwnerId},
                {"photo_id", photo.Id},             
                {"album_id", photo.AlbumId},    
            };
            string request = CreateMethodRequest("photos.makeCover", parameters);
            JToken res = GetJSONResponse(request);
            if (res["response"].Value<int>() != 1)
            {
                throw new UnknownVkException("Photo remove returned " + res.ToString());
            }
        }

        public void Photos_CreateAlbum(ref PhotoAlbumInfo album)
        {
            CheckPermissions(Permissions.PHOTOS);
            if (album.Title.Length < 2)
                throw new ArgumentException("title.length < 2");
            var parameters = new ParametersCollection() {                 
                {"title", album.Title},
                {"description", album.Description},             
                {"privacy_view", "onlyme"},    
                {"privacy_comment", "onlyme"},    
            };
            string request = CreateMethodRequest("photos.createAlbum", parameters);
            JToken res = GetJSONResponse(request);
            album = PhotoAlbumInfo.FromJson(res["response"]);
        }
    }
}
