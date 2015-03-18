using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My.VKMusic.Extensions;

namespace My.VKMusic.Models
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

        public static AudioFileInfo FromJson(JToken token)
        {
            return new AudioFileInfo()
            {
                Id = token["aid"].Value<long>(),
                Artist = token["artist"].Value<string>().AsUTF8(),
                Title = token["title"].Value<string>().AsUTF8(),
                URL = token["url"].Value<string>(),
            };
        }

    }
}
