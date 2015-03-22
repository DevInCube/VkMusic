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

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for PlayerTestWindow.xaml
    /// </summary>
    public partial class PlayerTestWindow : Window, INotifyPropertyChanged
    {

        private bool _LoadingStarted = true;

        public DragManager DragManager { get; set; }
        public bool CanReorder { get; set; }
        public ObservableCollection<ADragVM> Items { get; set; }
        public MouseButtonEventHandler OnDrag { get; set; }
        public ICommand PlayAudioCommand { get; set; }
        public ICommand EditAudioCommand { get; set; }
        public ICommand DeleteAudioCommand { get; set; }
        public ICommand ScrollCommand { get; set; }

        public bool IsLoading { 
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
                (o as DragTestItem2).IsPlaying = !(o as DragTestItem2).IsPlaying;
            });
            this.EditAudioCommand = new RelayCommand((o) =>
            {
                //@todo   
            });
            this.DeleteAudioCommand = new RelayCommand((o) =>
            {
                var item = (o as DragTestItem2);
                item.List.Remove(item);
            });
            ScrollCommand = new RelayCommand((args) =>
            {
                var e = args as ScrollChangedEventArgs;
                ScrollViewer sb = e.OriginalSource as ScrollViewer;
                double offset = sb.VerticalOffset;
                double maximum = sb.ScrollableHeight;
                if (offset == maximum)
                {
                    if (!IsLoading)
                    {
                        IsLoading = true;
                        BackgroundWorker loader = new BackgroundWorker();
                        loader.DoWork += (args2, e2) => {
                            Thread.Sleep(1000);
                        };
                        loader.RunWorkerCompleted += (args2, e2) => {
                            Items.Add(new DragTestItem2("New1", "new"));
                            Items.Add(new DragTestItem2("New2", "new"));
                            Items.Add(new DragTestItem2("New3", "new"));
                            Items.Add(new DragTestItem2("New4", "new"));
                            Items.Add(new DragTestItem2("New5", "new"));
                            IsLoading = false;
                        };
                        loader.RunWorkerAsync();
                    }
                }
            });
            this.DataContext = this;

            this.Loaded += PlayerTestWindow_Loaded;
        }

        void PlayerTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Items.Add(new DragTestItem2("Muse", "Resistance"));
            this.Items.Add(new DragTestItem2("Nickelback", "op"));
            this.Items.Add(new DragTestItem2("Tool", "Tool"));
            this.Items.Add(new DragTestItem2("Muse", "Uprising"));
            this.Items.Add(new DragTestItem2("Muse", "Test"));
            this.Items.Add(new DragTestItem2("Test", "test"));
            this.Items.Add(new DragTestItem2("Me", "Ok"));
            this.Items.Add(new DragTestItem2("Me", "NotOk"));
            this.Items.Add(new DragTestItem2("Me", "KukuOk"));
            this.Items.Add(new DragTestItem2("Me", "QwertyOk"));
            this.Items.Add(new DragTestItem2("Blablablaadasdasdasdasd", "asdasdasd"));
            IsLoading = false;
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
}
