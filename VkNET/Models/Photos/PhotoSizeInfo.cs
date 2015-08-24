using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VkNET.Models.Photos
{
    public class PhotoSizeInfo
    {

        public string Src { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Type { get; set; }

        public static PhotoSizeInfo FromJson(JObject j)
        {
            PhotoSizeInfo s = new PhotoSizeInfo()
            {
                Src = j["src"].Value<string>(),
                Width = j["width"].Value<int>(),
                Height = j["height"].Value<int>(),
                Type = j["type"].Value<string>(),
            };
            return s;
        }
    }
}
