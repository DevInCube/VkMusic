using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        public AudioFileInfo() { }        

        public override string ToString()
        {
            return "{0} - {1}".FormatWith(Artist, Title);
        }

        public static AudioFileInfo FromJson(JObject t)
        {
            return new AudioFileInfo()
            {
                Id = t["aid"].Value<long>(),
                OwnerId = t["owner_id"].Value<long>(),
                Artist = HttpUtility.HtmlDecode(t["artist"].Value<string>().AsUTF8()),
                Title = HttpUtility.HtmlDecode(t["title"].Value<string>().AsUTF8()),
                URL = t["url"].Value<string>(),
                Duration = t["duration"].Value<long>(),
                LyricsId = (t["lyrics_id"] != null) ? t["lyrics_id"].Value<long>() : 0,
                GenreId = (t["genre"] != null) ? t["genre"].Value<long>() : 0,
            };
        }

        public JToken ToJson()
        {
            JObject t = new JObject();
            t.Add("aid", new JValue(this.Id));
            t.Add("owner_id", new JValue(this.OwnerId));
            t.Add("artist", JValue.CreateString(HttpUtility.HtmlEncode(this.Artist)));
            t.Add("title", JValue.CreateString(HttpUtility.HtmlEncode(this.Title)));
            t.Add("url", JValue.CreateString(this.URL));
            t.Add("duration", new JValue(this.Duration));
            t.Add("lyrics_id", new JValue(this.LyricsId));
            t.Add("genre", new JValue(this.GenreId));
            return t;
        }

    }
}
