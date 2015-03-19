using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Extensions;

namespace VkNET.Models
{
    public class AudioAlbumInfo
    {

        public const int MAX_ALBUMS_COUNT = 100;
        public const int MAX_AUDIO_COUNT = 1000;

        public long OwnerId { get; set; }
        public long AlbumId { get; set; }
        public string Title { get; set; }

        public AudioAlbumInfo() { }

        public override string ToString()
        {
            return "{0}".FormatWith(Title);
        }

        public static AudioAlbumInfo FromJson(JToken jt)
        {
            AudioAlbumInfo album = new AudioAlbumInfo()
            {
                OwnerId = jt["owner_id"].Value<long>(),
                AlbumId = jt["album_id"].Value<long>(),
                Title = jt["title"].Value<string>().AsUTF8()
            };
            return album;
        }
    }
}
