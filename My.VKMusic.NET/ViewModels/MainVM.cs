using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;

namespace My.VKMusic.ViewModels
{
    public class MainVM: ObservableObject, IWindowVM
    {

        private AudioFile _SelectedAudio;

        public AudioFile SelectedAudio
        {
            get { return _SelectedAudio; }
            set { _SelectedAudio = value; OnPropertyChanged("SelectedAudio"); }
        }

        public ObservableCollection<AudioFile> Playlist { get; private set; }

        public MainVM()
        {
            Playlist = new ObservableCollection<AudioFile>();
        }

        public void Loaded(object sender)
        {
            for (int i = 0; i < 100; i++)
            {
                string artist = "Artist" + i;
                string title = "Title" + (i * i);
                Playlist.Add(new AudioFile(new AudioFileInfo() { Artist = artist, Title = title }));
            }
        }
    }
}
