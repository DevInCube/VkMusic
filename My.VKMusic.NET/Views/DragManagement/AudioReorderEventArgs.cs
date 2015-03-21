using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;

namespace My.VKMusic.Views.DragManagement
{
    public class AudioReorderEventArgs : EventArgs
    {
        public AudioFileInfo Audio { get; private set; }
        public AudioFileInfo BeforeAudio { get; private set; }
        public AudioFileInfo AfterAudio { get; private set; }

        public AudioReorderEventArgs(
            AudioFileInfo audio,
            AudioFileInfo before,
            AudioFileInfo after)
        {
            this.Audio = audio;
            this.AfterAudio = after;
            this.BeforeAudio = before;
        }
    }
}
