using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models.Photos
{
    public class PhotoInfo
    {

        public long Id { get; private set; }
        public long AlbumId { get; set; }
        public long OwnerId { get; set; }
        public string Text { get; set; }
        public string Src { get; set; }
        public IList<PhotoSizeInfo> Sizes { get; set; }

        public static PhotoInfo FromJson(JObject j)
        {
            string source;
            PhotoSizeInfo[] sizes = null;
            if (j["sizes"] != null)
            {
                JArray sizesArr = j["sizes"].Value<JArray>();
                sizes = new PhotoSizeInfo[sizesArr.Count];
                for (int i = 0; i < sizesArr.Count; i++)
                {
                    JToken size = sizesArr[i];
                    sizes[i] = PhotoSizeInfo.FromJson((JObject)size);
                }
                source = sizes[0].Src;
            }
            else
            {
                source = j["src"].Value<string>();
                List<PhotoSizeInfo> ss = new List<PhotoSizeInfo>();
                ss.Add(new PhotoSizeInfo() { Type = "src", Src = source });
                foreach (var stype in new string[] { "src_big", "src_xbig", "src_xxbig", "src_small" })
                    if (j[stype] != null)
                        ss.Add(new PhotoSizeInfo() { Type = stype, Src = j[stype].Value<string>() });
                sizes = ss.ToArray();
            }
            PhotoInfo p = new PhotoInfo()
            {
                Id = j["pid"].Value<long>(),
                AlbumId = j["aid"].Value<long>(),
                OwnerId = j["owner_id"].Value<long>(),
                Text = j["text"].Value<string>(),
                Src = source,
            };            
            p.Sizes = sizes;
            return p;
        }

        public override string ToString()
        {
            return String.Format("Image {0} : {1} sizes;", this.Id, this.Sizes.Count);
        }
    }
}
