using My.VKMusic.Models;
using My.VKMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for testWindow.xaml
    /// </summary>
    public partial class TestWindow : Window, INotifyPropertyChanged
    {

        private AudioPlayer player;
        private AudioFile _SelectedAudio;

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand PrevCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PlayAudioCommand { get; set; }

        public AudioFile SelectedAudio {
            get { return _SelectedAudio; }
            set {
                if (_SelectedAudio != null) SelectedAudio.IsSelected = false;
                _SelectedAudio = value;
                _SelectedAudio.IsSelected = true;
                InitAudio(_SelectedAudio); 
                OnPropertyChanged("SelectedAudio"); 
            }
        }
        public ObservableCollection<AudioFile> AudioList { get; set; }

        public TestWindow()
        {
            InitializeComponent();

            player = new AudioPlayer();
            AudioList = new ObservableCollection<AudioFile>();            
            PlayCommand = new SimpleCommand(() => { player.Play(); });
            PauseCommand = new SimpleCommand(() => { player.Pause(); });
            PrevCommand = new SimpleCommand(() => { SelectedAudio = GetPrevAudio(); });
            NextCommand = new SimpleCommand(() => { SelectedAudio = GetNextAudio(); });
            PlayAudioCommand = new RelayCommand((audio) => { SelectedAudio = audio as AudioFile; player.Play(); });

            this.DataContext = this;
        }

        public AudioFile GetPrevAudio()
        {
            int pos = AudioList.IndexOf(SelectedAudio) - 1;
            if(pos < 0) pos = AudioList.Count - 1;
            return AudioList[pos];
        }

        public AudioFile GetNextAudio()
        {
            int pos = AudioList.IndexOf(SelectedAudio) + 1;
            pos %= AudioList.Count;
            return AudioList[pos];
        }

        public void InitAudioList(IEnumerable<AudioFileInfo> audios)
        {
            this.AudioList.Clear();
            foreach (var audio in audios)
                this.AudioList.Add(new AudioFile(audio));
            SelectedAudio = AudioList.FirstOrDefault();
        }

        public void InitAudio(AudioFile info)
        {
            player.Init(info);
        }

        #region propchanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion

    }
}
