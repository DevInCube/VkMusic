using My.VKMusic.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;

namespace My.VKMusic.ViewModels
{
    public class AudioFile : ObservableObject
    {

        private AudioFileInfo info;
        private bool _IsPlaying, _IsSelected;
        private AudioFileReader reader;

        public AudioFileInfo Info { get { return info; } }
        public string Duration { get { return new TimeSpan(0, 0, (int)info.Duration).ToString(); } }

        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set { _IsPlaying = value; OnPropertyChanged("IsPlaying"); }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public AudioFile(AudioFileInfo info)
        {
            this.info = info;
            reader = new AudioFileReader(this.info.URL);
        }

        public AudioFileReader GetReader()
        {
            return reader;
        }
    }
}
