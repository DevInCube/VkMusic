using My.VKMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;
using VkNET.Extensions;

namespace My.VKMusic.Test
{
    class TestAudioSource : IAudioListSource
    {

        private List<AudioFileInfo> files = new List<AudioFileInfo>()
        {
            new AudioFileInfo(){Artist = "Test", Title="test"},
            new AudioFileInfo(){Artist = "Test1", Title="test"},
            new AudioFileInfo(){Artist = "Test2", Title="test"},
            new AudioFileInfo(){Artist = "Test3", Title="test"},
            new AudioFileInfo(){Artist = "Test4", Title="test"},
            new AudioFileInfo(){Artist = "Test6", Title="test"},
            new AudioFileInfo(){Artist = "Test5", Title="test"},
            new AudioFileInfo(){Artist = "Test6", Title="test"},
            new AudioFileInfo(){Artist = "Test21", Title="test"},
            new AudioFileInfo(){Artist = "Test22", Title="test"},
            new AudioFileInfo(){Artist = "Test23", Title="test"},
            new AudioFileInfo(){Artist = "Test24", Title="test"},
            new AudioFileInfo(){Artist = "Test25", Title="test"},
            new AudioFileInfo(){Artist = "Test26", Title="test"},
            new AudioFileInfo(){Artist = "Test27", Title="test"},
            new AudioFileInfo(){Artist = "Test28", Title="test"},
        };

        private List<AudioFileInfo> items;

        public IList<VkNET.Models.AudioFileInfo> AudioItems
        {
            get { return items; }
        }

        public void Load()
        {
            items = new List<AudioFileInfo>(files);
        }

        public void Shuffle()
        {
            items = new List<AudioFileInfo>(files);
            items.Shuffle();
        }


        public void Reorder(AudioFileInfo audioFileInfo1, AudioFileInfo audioFileInfo2, AudioFileInfo audioFileInfo3)
        {
            //throw new NotImplementedException();
        }


        public void Delete(AudioFileInfo audioFileInfo)
        {
            files.Remove(audioFileInfo);
        }
    }
}
