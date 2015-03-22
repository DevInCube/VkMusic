using My.VkMusic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET;
using VkNET.Models;
using VkNET.Extensions;

namespace My.VKMusic.ViewModels
{
    public class VkAudioListSource : IAudioListSource
    {
        VkAPI api = new VkAPI(new IEAuthProvider());
        List<AudioFileInfo> files;
        bool loaded = false;
        int position = 0, loadCount = 100;

        public VkAudioListSource()
        {
            api.DoAuth(() => {
                loaded = true;
            });
        }

        public IList<VkNET.Models.AudioFileInfo> AudioItems
        {
            get { return files; }
        }

        public void Load()
        {
            if (loaded)
            {
                int totalCount = 0;
                files = api.AudioGet(null, null, out totalCount);
            }
        }

        public void Shuffle()
        {
            if (files != null)
                files.Shuffle();
        }
    }
}
