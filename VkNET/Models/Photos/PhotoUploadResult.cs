using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models.Photos
{
    public class PhotoUploadResult
    {       

        public long Server { get; private set; }
        public string Hash { get; private set; }
        public long AlbumId { get; private set; }
        public string PhotosList { get; private set; }

        public static PhotoUploadResult FromJson(JObject jObject)
        {
            PhotoUploadResult r = new PhotoUploadResult()
            {
                Server = jObject["server"].Value<long>(),
                Hash = jObject["hash"].Value<string>(),
                AlbumId = jObject["aid"].Value<long>(),
                PhotosList = jObject["photos_list"].Value<string>(),
            };            
            return r;
        }
    }
}
