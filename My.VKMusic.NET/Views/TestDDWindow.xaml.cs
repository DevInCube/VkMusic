﻿using My.VKMusic.Tools;
using My.VKMusic.ViewModels;
using My.VKMusic.Views.DragManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for TestDDWindow.xaml
    /// </summary>
    public partial class TestDDWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<ADragVM> Items1 { get; set; }
        public ObservableCollection<ADragVM> Items2 { get; set; }

        public DragManager DragManager { get; set; }

        public TestDDWindow()
        {
            InitializeComponent();            

            this.Items1 = new ObservableCollection<ADragVM>();
            this.Items1.CollectionChanged += Items1_CollectionChanged;
            this.Items1.Add(new DragTestItem(1));
            this.Items1.Add(new DragTestItem(2));
            this.Items1.Add(new DragTestItem(3));
            this.Items1.Add(new DragTestItem(10));
          
            this.Items2 = new ObservableCollection<ADragVM>();
            this.Items2.CollectionChanged += Items1_CollectionChanged;
            this.Items2.Add(new DragTestItem(25));
            this.Items2.Add(new DragTestItem(255));

            DragManager = new DragManager();

            this.DataContext = this;
        }

        void Items1_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                DragManager.OnDragStart(sender);
            }
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            DragManager.OnDrop(sender, e);
        }

        private void Label_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            DragManager.GiveFeedbackEvent();
        }

        private void ItemsControl_DragEnter(object sender, DragEventArgs e)
        {
            DragManager.OnDragEnter(sender, e);
        }

        private void ItemsControl_DragLeave(object sender, DragEventArgs e)
        {
            DragManager.OnDragLeave(sender, e);
        }
    }

    public class DragTestItem : ADragVM
    {
        private int i;
        public DragTestItem(int i)
        {
            this.i = i;
        }
        public override string ToString()
        {
            return i.ToString();
        }

        public override object Clone()
        {
            return new DragTestItem(i);
        }

        public override bool Equals(ADragVM other)
        {
            return this.i == (other as DragTestItem).i;
        }
    }

    public class DragTestItem2 : ADragVM
    {
        private bool _IsPlaying;
        public class InfoClass
        {
            public string Artist { get; set; }
            public string Title { get; set; }
        }

        public InfoClass Info { get; set; }
        public bool IsPlaying
        {
            get { return _IsPlaying;  }
            set { _IsPlaying = value; OnPropertyChanged("IsPlaying"); }
        }
        public DragTestItem2(string artist, string title)
        {
            this.Info = new InfoClass() { Artist = artist, Title = title };
        }
        public override string ToString()
        {
            return Info.Artist + " - " + Info.Title;
        }

        public override object Clone()
        {
            return new DragTestItem2(Info.Artist, Info.Title);
        }

        public override bool Equals(ADragVM other)
        {
            var o = (other as DragTestItem2);
            return this.Info.Artist.Equals(o.Info.Artist) &&
                this.Info.Title.Equals(o.Info.Title)
                && this.IsDropPreview == o.IsDropPreview;
        }
    }
}
