using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models
{
    public class AudioAlbumInfo
    {

        public const int MAX_AUDIO_COUNT = 1000;

        public long AlbumId { get; set; }
        public string Title { get; set; }

        public AudioAlbumInfo() { }
    }
}
