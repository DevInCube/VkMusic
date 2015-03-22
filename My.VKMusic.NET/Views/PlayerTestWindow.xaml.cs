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
        private IAudioListSource audioSource = new VkAudioListSource();       
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
        public ICommand EditAudioCommand { get; set; }
        public ICommand DeleteAudioCommand { get; set; }
        public ICommand ScrollCommand { get; set; }

        public bool IsLoading 
        { 
            get { return _LoadingStarted; }
            set { _LoadingStarted = value; OnPropertyChanged("IsLoading"); }
        }

        public PlayerTestWindow()
        {
            InitializeComponent();
            this.Items = new ObservableCollection<ADragVM>();
            this.Items.CollectionChanged += Items_CollectionChanged;            

            this.DragManager = new DragManager();
            CanReorder = false;
            DragManager.Reorder += DragManager_Reorder;
            this.OnDrag = (MouseButtonEventHandler)((sender, e) => { DragManager.OnDragStart(sender); });
            this.PlayAudioCommand = new RelayCommand((o) => {
                var newAudio = (o as AudioFile);
                if (CurrentAudio != null && CurrentAudio!=newAudio)
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
                               
            });
            this.EditAudioCommand = new RelayCommand((o) =>
            {
                //@todo   
            });
            this.DeleteAudioCommand = new RelayCommand((o) =>
            {
                var item = (o as ADragVM);
                item.List.Remove(item);
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
            this.Player = new AudioPlayer();
            this.DataContext = this;            

            this.Loaded += PlayerTestWindow_Loaded;
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
            if (audioSource.AudioItems == null) return;
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
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        var items = this.Items;
                        this.Items = null;
                        foreach (var info in infos)
                        {
                            AudioFile file = new AudioFile(info);
                            items.Add(file);
                        }
                        this.Items = items;
                    }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                    IsLoading = false;
                    if (callback != null)
                        callback.Invoke();
                };
                loader.RunWorkerAsync();
            }
        }

        void PlayerTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            audioSource.Load();
            ReloadItems();
            LoadItems();
        }

        void DragManager_Reorder(object sender, AudioReorderEventArgs e)
        {
            return; //@todo
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
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion

    }

    class TestAudioSource : IAudioListSource
    {

        private List<AudioFileInfo> files = new List<AudioFileInfo>()
        {
            new AudioFileInfo(){Artist = "Test", Title="test"},
            new AudioFileInfo(){Artist = "Test1", Title="test"},
            new AudioFileInfo(){Artist = "Test2", Title="test"},
            new AudioFileInfo(){Artist = "Test3", Title="test"},
            new AudioFileInfo(){Artist = "Test4", Title="test"},
            new AudioFileInfo(){Artist = "Test6", Title="test"},
            new AudioFileInfo(){Artist = "Test5", Title="test"},
            new AudioFileInfo(){Artist = "Test6", Title="test"},
            new AudioFileInfo(){Artist = "Test21", Title="test"},
            new AudioFileInfo(){Artist = "Test22", Title="test"},
            new AudioFileInfo(){Artist = "Test23", Title="test"},
            new AudioFileInfo(){Artist = "Test24", Title="test"},
            new AudioFileInfo(){Artist = "Test25", Title="test"},
            new AudioFileInfo(){Artist = "Test26", Title="test"},
            new AudioFileInfo(){Artist = "Test27", Title="test"},
            new AudioFileInfo(){Artist = "Test28", Title="test"},
        };

        private List<AudioFileInfo> items;

        public IList<VkNET.Models.AudioFileInfo> AudioItems
        {
            get { return items; }
        }

        public void Load()
        {
            items = new List<AudioFileInfo>(files);
        }

        public void Shuffle()
        {
            items = new List<AudioFileInfo>(files);
            items.Shuffle();
        }
    }
}
