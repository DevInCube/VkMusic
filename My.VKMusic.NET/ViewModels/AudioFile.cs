using My.VKMusic.Models;
using My.VKMusic.Views.DragManagement;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;

namespace My.VKMusic.ViewModels
{
    public class AudioFile : ADragVM
    {

        private AudioFileInfo info;
        private bool _IsPlaying, _IsCurrent;
        private AudioFileReader reader;

        public AudioFileInfo Info { get { return info; } }
        public string Duration { get { return new TimeSpan(0, 0, (int)info.Duration).ToString(); } }

        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set { _IsPlaying = value; OnPropertyChanged("IsPlaying"); }
        }

        public bool IsCurrent
        {
            get { return _IsCurrent; }
            set { _IsCurrent = value; OnPropertyChanged("IsCurrent"); }
        }
       

        public AudioFile(AudioFileInfo info)
        {
            this.info = info;
        }

        public AudioFileReader GetReader()
        {
            if (this.info.URL != null && reader == null)
                reader = new AudioFileReader(this.info.URL);
            return reader;
        }

        public override object Clone()
        {
            return new AudioFile(info);
        }

        public override bool Equals(ADragVM other)
        {
            if (this == other) return true;
            AudioFile af = other as AudioFile;
            return af.info.Equals(this.info);
        }

    }
}
