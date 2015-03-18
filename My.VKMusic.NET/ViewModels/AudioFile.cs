using My.VKMusic.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.ViewModels
{
    public class AudioFile : ObservableObject
    {

        private AudioFileInfo info;
        private bool _IsPlaying;
        private AudioFileReader reader;

        public AudioFileInfo Info { get { return info; } }

        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set { _IsPlaying = value; OnPropertyChanged("IsPlaying"); }
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
