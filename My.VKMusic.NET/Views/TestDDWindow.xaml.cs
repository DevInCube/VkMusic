using My.VKMusic.Tools;
using My.VKMusic.ViewModels;
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

        private Window _dragdropWindow;
        public ObservableCollection<ADragVM> Items1 { get; set; }
        public ObservableCollection<ADragVM> Items2 { get; set; }

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
                Label dragControl = sender as Label;    
  
                ADragVM item = (dragControl.DataContext as ADragVM);
                int index = item.List.IndexOf(item);
                item.List.Remove(item);

                var clone = item.Clone() as ADragVM;
                clone.IsDragged = true;
                item.List.Insert(index, clone);
                CreateDragDropWindow(dragControl.Parent as Visual);                
                var ret = DragDrop.DoDragDrop(dragControl, dragControl.DataContext, DragDropEffects.Move);
                CloseDragWindow(false);
            }
        }

        void CloseDragWindow(bool success)
        {
            if (_dragdropWindow != null)
            {
                ADragVM dropData = _dragdropWindow.DataContext as ADragVM;

                ADragVM fakeItem = dropData.List.FirstOrDefault(x => x.IsDragged == true);

                if (fakeItem != null)
                {
                    if (!success)
                    {
                        int index = dropData.List.IndexOf(fakeItem);
                        dropData.List.Insert(index, dropData);
                    }
                    dropData.List.Remove(fakeItem);
                }

                _dragdropWindow.Close();
                _dragdropWindow = null;
            }
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            var droppedData = this._dragdropWindow.DataContext as ADragVM;

            if (sender is ItemsControl)
            {
                var target = (sender as ItemsControl).DataContext as ObservableCollection<ADragVM>;
                if (!target.Contains(droppedData))
                    target.Add(droppedData);
                else
                {
                    target.Remove(droppedData);
                    target.Add(droppedData);
                }          
            }
            if (sender is DockPanel)
            {
                var targetElem = sender as DockPanel;
                var targetItem = targetElem.DataContext as ADragVM;
                var list = targetItem.List;
                int index = list.IndexOf(targetItem);
                list.Remove(droppedData);
                list.Insert(index, droppedData);
                e.Handled = true;
            }
            CloseDragWindow(true);
        }

        private void Label_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            CursorHelper.GetCursorPos(ref w32Mouse);

            this._dragdropWindow.Left = w32Mouse.X + 10;
            this._dragdropWindow.Top = w32Mouse.Y + 10;
        }

        private void CreateDragDropWindow(Visual dragElement)
        {
            this._dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.Opacity = 0.8;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            this._dragdropWindow.Content = r;

            Win32Point w32Mouse = new Win32Point();
            CursorHelper.GetCursorPos(ref w32Mouse);

            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
            this._dragdropWindow.Show();
            this._dragdropWindow.DataContext = (dragElement as DockPanel).DataContext;
        }

        private void DockPanel_DragOver(object sender, DragEventArgs e)
        {
            if (sender is DockPanel)
            {
                DockPanel dragControl = sender as DockPanel;
                ADragVM item = dragControl.DataContext as ADragVM;
                ADragVM fakeItem = item.List.FirstOrDefault(x => x.IsDragged == true);
                if (fakeItem != null)
                {
                    int index = item.List.IndexOf(item);
                    item.List.Remove(fakeItem);
                    item.List.Insert(index, fakeItem);
                }
                e.Handled = true;
            }
        }

        private void ItemsControl_DragOver(object sender, DragEventArgs e)
        {
            ItemsControl dragControl = sender as ItemsControl;
            ObservableCollection<ADragVM> list = dragControl.DataContext as ObservableCollection<ADragVM>;
            ADragVM fakeItem = list.FirstOrDefault(x => x.IsDragged == true);
            if (fakeItem != null)
            {
                int index = list.Count - 1;
                list.Remove(fakeItem);
                list.Insert(index, fakeItem);
            }
        }

        private void ItemsControl_DragEnter(object sender, DragEventArgs e)
        {
            var dragItem = this._dragdropWindow.DataContext as ADragVM;

            ItemsControl dragControl = sender as ItemsControl;
            ObservableCollection<ADragVM> list = dragControl.DataContext as ObservableCollection<ADragVM>;

            if (dragItem.List != list)
            {
                dragItem.List.Remove(dragItem.List.FirstOrDefault(x => x.IsDragged == true));
                dragItem.List = list;
            }

            ADragVM fakeItem = list.FirstOrDefault(x => x.IsDragged == true);
            if (fakeItem == null)
            {
                var clone = dragItem.Clone() as ADragVM;
                clone.IsDragged = true;
                list.Add(clone);
            }
        }

        private void ItemsControl_DragLeave(object sender, DragEventArgs e)
        {            
           
        }

       
    }

    public abstract class ADragVM : ObservableObject, ICloneable
    {

        private bool _IsDragged;

        public  ObservableCollection<ADragVM> List { get; set; }

        public bool IsDragged
        {
            get { return _IsDragged; }
            set { _IsDragged = value; OnPropertyChanged("IsDragged"); }
        }

        public abstract object Clone();
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
    }
}
