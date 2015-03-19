using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Extensions;

namespace VkNET.Models
{
    public class AudioFileInfo
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public long Duration { get; set; }
        public string URL { get; set; }
        public long LyricsId { get; set; }
        public long GenreId { get; set; }

        public override string ToString()
        {
            return "{0} - {1}".FormatWith(Artist, Title);
        }

        public static AudioFileInfo FromJson(JToken token)
        {
            return new AudioFileInfo()
            {
                Id = token["aid"].Value<long>(),
                Artist = token["artist"].Value<string>().AsUTF8(),
                Title = token["title"].Value<string>().AsUTF8(),
                URL = token["url"].Value<string>(),
                Duration = token["duration"].Value<long>(),
            };
        }

    }
}
