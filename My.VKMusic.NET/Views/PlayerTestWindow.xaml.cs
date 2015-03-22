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

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for PlayerTestWindow.xaml
    /// </summary>
    public partial class PlayerTestWindow : Window, INotifyPropertyChanged
    {

        public const double SCROLL_LOAD_KOEF = 1.0D;

        private bool _LoadingStarted, _Shuffle;
        private IAudioListSource audioSource = new TestAudioSource();       
        private int position = 0;
        private int loadCount = 10;
        private AudioFile currentAudio = null;

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
                if (currentAudio != null) 
                    currentAudio.IsPlaying = false; //@todo
                var newAudio = (o as AudioFile);
                newAudio.IsPlaying = !newAudio.IsPlaying;//@todo
                currentAudio = newAudio;
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
            this.DataContext = this;

            audioSource.Load();                        

            this.Loaded += PlayerTestWindow_Loaded;
        }

        void ReloadItems()
        {
            this.Items.Clear();
            position = 0;
            LoadItems();
        }

        void LoadItems()
        {
            int totalItemsCount = audioSource.AudioItems.Count;
            if (!IsLoading && position < totalItemsCount)
            {
                IsLoading = true;
                BackgroundWorker loader = new BackgroundWorker();
                loader.DoWork += (args2, e2) =>
                {
                    Thread.Sleep(1000);
                };
                loader.RunWorkerCompleted += (args2, e2) =>
                {
                    for (int i = position; i < position + loadCount; i++)
                    {
                        if (i >= totalItemsCount) break;
                        AudioFileInfo info = audioSource.AudioItems[i];
                        AudioFile file = new AudioFile(info);
                        this.Items.Add(file);
                    }
                    position += loadCount;
                    if (position >= totalItemsCount)
                        position = totalItemsCount;
                    IsLoading = false;
                };
                loader.RunWorkerAsync();
            }
        }

        void PlayerTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
