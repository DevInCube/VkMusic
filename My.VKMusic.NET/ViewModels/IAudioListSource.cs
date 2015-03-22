using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;

namespace My.VKMusic.ViewModels
{
    public interface IAudioListSource
    {
        IList<AudioFileInfo> AudioItems { get; }
        void Load();
        void Shuffle();


        void Reorder(AudioFileInfo audioFileInfo1, AudioFileInfo audioFileInfo2, AudioFileInfo audioFileInfo3);
    }
}
