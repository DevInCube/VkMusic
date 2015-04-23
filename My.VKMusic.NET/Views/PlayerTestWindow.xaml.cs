using My.VKMusic.ViewModels;
using My.VKMusic.Views.DragManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VkNET.Models;
using VkNET.Extensions;
using My.VKMusic.Models;
using System.IO;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for PlayerTestWindow.xaml
    /// </summary>
    public partial class PlayerTestWindow : Window, INotifyPropertyChanged
    {

        public const double SCROLL_LOAD_KOEF = 1.0D;

        private bool _LoadingStarted, _Shuffle;
        public AudioPlayer Player { get; set; }
        private AudioFile _CurrentAudio;
        private IAudioListSource audioSource;
        private int position = 0;
        private int loadCount = 10;
        public AudioFile CurrentAudio {
            get { return _CurrentAudio; }
            set { _CurrentAudio = value; OnPropertyChanged("CurrentAudio"); }
        }

        public DragManager DragManager { get; set; }
        public bool CanReorder { get; set; }
        public bool Shuffle {
            get { return _Shuffle; }
            set
            {
                _Shuffle = value;
                if (_Shuffle)
                    audioSource.Shuffle();
                else
                    audioSource.Load();
                ReloadItems();
            }
        }
        public ObservableCollection<ADragVM> Items { get; set; }
        public MouseButtonEventHandler OnDrag { get; set; }
        public ICommand PlayAudioCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand PrevCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand EditAudioCommand { get; set; }
        public ICommand DeleteAudioCommand { get; set; }
        public ICommand ScrollCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand CancelDownloadCommand { get; set; }

        public bool IsLoading 
        { 
            get { return _LoadingStarted; }
            set { _LoadingStarted = value; OnPropertyChanged("IsLoading"); }
        }

        public int DownloadProgress
        {
            get { return _DownloadPosition; }
            private set { _DownloadPosition = value; OnPropertyChanged("DownloadProgress"); }
        }

        public string DownloadAudioTitle
        {
            get { return _DownloadAudioTitle; }
            private set { _DownloadAudioTitle = value; OnPropertyChanged("DownloadAudioTitle"); }
        }

        private System.Net.WebClient dClient;

        public PlayerTestWindow()
        {
            InitializeComponent();            

            this.Loaded += PlayerTestWindow_Loaded;
        }

        void Init()
        {
            this.Items = new ObservableCollection<ADragVM>();
            this.Items.CollectionChanged += Items_CollectionChanged;

            this.DragManager = new DragManager();
            this.CanReorder = true;
            DragManager.Reorder += DragManager_Reorder;
            this.OnDrag = (MouseButtonEventHandler)((sender, e) => { DragManager.OnDragStart(sender); });
            this.PlayAudioCommand = new RelayCommand((o) =>
            {
                PlayAudio(o as AudioFile);
            });
            this.StopCommand = new RelayCommand((o) =>
            {
                Player.Stop();
                Player.Position = 0;
            });
            this.PrevCommand = new RelayCommand((o) =>
            {
                PlayAudio(GetPrevAudio());
            });
            this.NextCommand = new RelayCommand((o) =>
            {
                PlayAudio(GetNextAudio());
            });
            this.EditAudioCommand = new RelayCommand((o) =>
            {
                //@todo   
            });
            this.DeleteAudioCommand = new RelayCommand((o) =>
            {
                var item = (o as ADragVM);
                item.List.Remove(item);
                audioSource.Delete((item as AudioFile).Info);
            });
            ScrollCommand = new RelayCommand((args) =>
            {
                var e = args as ScrollChangedEventArgs;
                ScrollViewer sb = e.OriginalSource as ScrollViewer;
                double offset = sb.VerticalOffset;
                double maximum = sb.ScrollableHeight;
                if (offset == maximum * SCROLL_LOAD_KOEF)
                {
                    LoadItems();
                }
            });
            DownloadCommand = new RelayCommand((args) =>
            {
                string urlStr = CurrentAudio.Info.URL;
                this.DownloadAudioTitle = CurrentAudio.Info.ToString();                
                using (var dClient = new System.Net.WebClient())
                {
                    Uri url = new Uri(urlStr);
                    dClient.DownloadDataCompleted += dClient_DownloadDataCompleted;
                    dClient.DownloadProgressChanged += dClient_DownloadProgressChanged;
                    dClient.DownloadDataAsync(url, CurrentAudio);                    
                    this.dClient = dClient;
                }
            });
            CancelDownloadCommand = new RelayCommand((args) =>
            {
                this.dClient.CancelAsync();
            });
            this.Player = new AudioPlayer();
            Player.TrackFinished += Player_TrackFinished;
            this.DataContext = this;           
        }

        void dClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {            
            int progress = e.ProgressPercentage;
            this.DownloadProgress = progress;
        }

        void dClient_DownloadDataCompleted(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.DownloadProgress = 0;
                return;
            }
            byte[] mp3 = e.Result;
            string dPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
            AudioFile audio = e.UserState as AudioFile;
            string userDirName = audio.Info.OwnerId.ToString();
            string audioName = String.Format("{0}.mp3", audio.Info.Id);
            string userDir = System.IO.Path.Combine(dPath, userDirName);
            if(!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }
            string filePath = System.IO.Path.Combine(userDir, audioName);
            System.IO.File.WriteAllBytes(filePath, mp3);
        }

        void Player_TrackFinished(AudioFile obj)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (true) //not loop @todo
                {
                    PlayAudio(GetNextAudio());
                }
            }));
        }

        private void PlayAudio(AudioFile newAudio)
        {
            if (CurrentAudio != null && CurrentAudio != newAudio)
            {
                Player.Stop();
                CurrentAudio.IsPlaying = false;
            }
            SetCurrentAudio(newAudio);
            if (newAudio.IsPlaying)
            {
                Player.Pause();
            }
            else
            {
                Player.Play();
            }
            //newAudio.IsPlaying = !newAudio.IsPlaying;//@todo
        }

        public AudioFile GetPrevAudio()
        {
            int pos = this.Items.IndexOf(CurrentAudio) - 1;
            if (pos < 0) pos = this.Items.Count - 1;
            return this.Items[pos] as AudioFile;
        }

        public AudioFile GetNextAudio()
        {
            int pos = this.Items.IndexOf(CurrentAudio) + 1;
            pos %= this.Items.Count;
            return this.Items[pos] as AudioFile;
        }

        private void SetCurrentAudio(AudioFile newAudio)
        {
            if (CurrentAudio != null
                && newAudio != CurrentAudio)
                CurrentAudio.IsCurrent = false;
            newAudio.IsCurrent = true;
            if (CurrentAudio != newAudio) Player.Init(newAudio);
            CurrentAudio = newAudio;
        }

        void ReloadItems()
        {
            this.Items.Clear();
            position = 0;
            LoadItems(() => {
                if (CurrentAudio == null)
                {
                    var audio = Items.FirstOrDefault();
                    if (audio != null)
                        SetCurrentAudio(audio as AudioFile);
                }
            });
        }

        void LoadItems(Action callback = null)
        {
            if (audioSource == null || audioSource.AudioItems == null) return;
            int totalItemsCount = audioSource.AudioItems.Count;
            if (!IsLoading && position < totalItemsCount)
            {
                IsLoading = true;
                BackgroundWorker loader = new BackgroundWorker();
                loader.DoWork += (args2, e2) =>
                {
                    List<AudioFileInfo> infos = new List<AudioFileInfo>();
                    for (int i = position; i < position + loadCount; i++)
                    {
                        if (i >= totalItemsCount) break;
                        AudioFileInfo info = audioSource.AudioItems[i];
                        infos.Add(info);
                    }
                    position += loadCount;
                    if (position >= totalItemsCount)
                        position = totalItemsCount;
                    e2.Result = infos;
                };
                loader.RunWorkerCompleted += (args2, e2) =>
                {
                    List<AudioFileInfo> infos = e2.Result as List<AudioFileInfo>;
                    foreach (var info in infos)
                    {
                        AudioFile file = new AudioFile(info);
                        this.Items.Add(file);
                    }                    
                    IsLoading = false;
                    if (callback != null)
                        callback.Invoke();
                };
                loader.RunWorkerAsync();
            }
        }

        void PlayerTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            audioSource = new VkAudioListSource();
            Init();
            this.IsLoading = true;
            BackgroundWorker loader = new BackgroundWorker();
            loader.DoWork += (s2, e2) => { audioSource.Load(); };
            loader.RunWorkerCompleted += (s2, e2) => {
                IsLoading = false;
                ReloadItems();
                LoadItems();
            };
            loader.RunWorkerAsync();
        }

        void DragManager_Reorder(object sender, AudioReorderEventArgs e)
        {
            audioSource.Reorder(e.Audio, e.BeforeAudio, e.AfterAudio);
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                (e.NewItems[0] as ADragVM).List = sender as ObservableCollection<ADragVM>;
            }
        }


        #region propchanged

        public event PropertyChangedEventHandler PropertyChanged;
        private int _DownloadPosition;
        private string _DownloadAudioTitle;
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
