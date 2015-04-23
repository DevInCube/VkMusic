using My.VKMusic.Models;
using My.VKMusic.Views.DragManagement;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VkNET.Models;

namespace My.VKMusic.ViewModels
{
    public class AudioFile : ADragVM
    {

        private AudioFileInfo info;
        private bool _IsPlaying, _IsCurrent;
        private AudioFileReader reader;
        private bool _DetailsExpanded;
        private string _Lyrics;

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

        public bool DetailsExpanded
        {
            get { return _DetailsExpanded; }
            set { _DetailsExpanded = value; OnPropertyChanged("DetailsExpanded"); }
        }

        public string Lyrics
        {
            get { return _Lyrics; }
            set { _Lyrics = value; OnPropertyChanged("Lyrics"); }
        }

        public ICommand ExpandCommand { get; set; }
       

        public AudioFile(AudioFileInfo info)
        {
            this.info = info;
            ExpandCommand = new SimpleCommand(() => {
                DetailsExpanded = !DetailsExpanded;
            });
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
