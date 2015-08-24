using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models.Photos
{
    public class PhotoAlbumInfo
    {

        public long Id { get; private set; }
        public long OwnerId { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Size { get; private set; }
        public string ThumbSrc { get; set; }

        public static PhotoAlbumInfo FromJson(JToken j)
        {
            PhotoAlbumInfo a = new PhotoAlbumInfo()
            {
                Id = j["aid"].Value<long>(),
                OwnerId = j["owner_id"].Value<long>(),
                Title = j["title"].Value<string>(),
                Description = j["description"].Value<string>(),
                Size = j["size"].Value<int>(),
                ThumbSrc = j["thumb_src"].Value<string>()
            };
            return a;
        }
    }
}
