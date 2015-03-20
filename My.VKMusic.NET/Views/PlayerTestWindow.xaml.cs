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


        public PlayerTestWindow()
        {
            InitializeComponent();
            this.Items = new ObservableCollection<ADragVM>();
            this.Items.CollectionChanged += Items_CollectionChanged;
            this.Items.Add(new DragTestItem(1));
            this.Items.Add(new DragTestItem(2));
            this.Items.Add(new DragTestItem(3));
            this.Items.Add(new DragTestItem(10));

            this.DragManager = new DragManager();

            this.DataContext = this;
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
