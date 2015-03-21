using My.VKMusic.ViewModels;
using My.VKMusic.Views.DragManagement;
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
    /// Interaction logic for PlayerTestWindow.xaml
    /// </summary>
    public partial class PlayerTestWindow : Window, INotifyPropertyChanged
    {
        public DragManager DragManager { get; set; }
        public ObservableCollection<ADragVM> Items { get; set; }
        public MouseButtonEventHandler OnDrag { get; set; }
        public ICommand PlayAudioCommand { get; set; }
        public ICommand EditAudioCommand { get; set; }
        public ICommand DeleteAudioCommand { get; set; }

        public PlayerTestWindow()
        {
            InitializeComponent();
            this.Items = new ObservableCollection<ADragVM>();
            this.Items.CollectionChanged += Items_CollectionChanged;
            this.Items.Add(new DragTestItem2("Muse","Resistance"));
            this.Items.Add(new DragTestItem2("Nickelback", "op"));
            this.Items.Add(new DragTestItem2("Tool", "Tool"));
            this.Items.Add(new DragTestItem2("Muse", "Uprising"));

            this.DragManager = new DragManager();
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
            this.DataContext = this;
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
