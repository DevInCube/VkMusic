using My.VKMusic.Tools;
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
                clone.IsDropPreview = true;
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

                var dragContext = this._dragdropWindow.DataContext as DragContext;
                var dragItem = dragContext.Item;

                ADragVM fakeItem = dragItem.List.FirstOrDefault(x => x.IsDropPreview == true);

                if (fakeItem != null)
                {
                    if (!success)
                    {
                        int index = dragItem.List.IndexOf(fakeItem);
                        dragItem.List.Insert(index, dragItem);
                    }
                    dragItem.List.Remove(fakeItem);
                }

                _dragdropWindow.Close();
                _dragdropWindow = null;
            }
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            var dragContext = this._dragdropWindow.DataContext as DragContext;
            var dragItem = dragContext.Item;

            if (sender is ItemsControl)
            {
                if (dragContext.DragAction == DragActionType.Reorder &&
                    (sender as ItemsControl).DataContext != dragItem.List) return;

                var target = (sender as ItemsControl).DataContext as ObservableCollection<ADragVM>;
                if (!target.Contains(dragItem))
                    target.Add(dragItem);
                else
                {
                    target.Remove(dragItem);
                    target.Add(dragItem);
                }
            }
            if (sender is DockPanel)
            {
                var targetElem = sender as DockPanel;
                var targetItem = targetElem.DataContext as ADragVM;

                if (dragContext.DragAction == DragActionType.Reorder &&
                    targetItem.List != dragItem.List) return;

                var list = targetItem.List;
                int index = list.IndexOf(targetItem);
                list.Remove(dragItem);
                list.Insert(index, dragItem);
                e.Handled = true;
            }
            
            CloseDragWindow(true);

            ADragVM fakeItem = (dragContext.SourceList as ObservableCollection<ADragVM>).FirstOrDefault(x => x.IsDropPreview == true);
            if (fakeItem != null)
                fakeItem.IsDropPreview = false;
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

            var dragItem = (dragElement as DockPanel).DataContext as ADragVM;
            this._dragdropWindow.DataContext = new DragContext(dragItem, dragItem.List, DragActionType.Copy);
        }

        private void DockPanel_DragOver(object sender, DragEventArgs e)
        {
           // e.Handled = true;
        }

        private void ItemsControl_DragOver(object sender, DragEventArgs e)
        {
            /*ItemsControl dragControl = sender as ItemsControl;
            ObservableCollection<ADragVM> list = dragControl.DataContext as ObservableCollection<ADragVM>;

            var dragContext = this._dragdropWindow.DataContext as DragContext;
            if ((dragContext.DragAction == DragActionType.Reorder
                && dragContext.Item.List == list)
                || (dragContext.Item.List != list))
            {              
                ADragVM fakeItem = list.FirstOrDefault(x => x.IsDropPreview == true);
                if (fakeItem != null)
                {
                    list.Remove(fakeItem);
                    list.Add(fakeItem);
                }
            }*/
        }


        private void DockPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is DockPanel)
            {
                DockPanel dragControl = sender as DockPanel;
                ADragVM item = dragControl.DataContext as ADragVM;
                var thisList = item.List;

                if (!item.IsDropPreview)
                {
                    var dragContext = this._dragdropWindow.DataContext as DragContext;
                    if (((dragContext.DragAction == DragActionType.Reorder)
                        && (dragContext.SourceList == thisList))
                        || ((dragContext.DragAction != DragActionType.Reorder) 
                        && (dragContext.SourceList != thisList)))
                    {
                        if(dragContext.DragAction == DragActionType.Copy 
                            && thisList.Contains(dragContext.Item)) return;

                        ADragVM fakeItem = item.List.FirstOrDefault(x => x.IsDropPreview == true);
                        if (fakeItem == null)
                        {
                            fakeItem = dragContext.Item.Clone() as ADragVM;
                            fakeItem.IsDropPreview = true;
                            thisList.Add(fakeItem);
                        }

                        int oldIndex = thisList.IndexOf(fakeItem);
                        int newIndex = thisList.IndexOf(item);

                        thisList.Move(oldIndex, newIndex);

                        blockedList = thisList;
                    }
                }
                e.Handled = true;
            }
        }

        private void ItemsControl_DragEnter(object sender, DragEventArgs e)
        {
            var dragContext = this._dragdropWindow.DataContext as DragContext;
            var dragItem = dragContext.Item;

            ItemsControl dragControl = sender as ItemsControl;
            dragControl.BorderThickness = new Thickness(2);
            dragControl.BorderBrush = Brushes.Red;
            ObservableCollection<ADragVM> list = dragControl.DataContext as ObservableCollection<ADragVM>;

            if (dragContext.DragAction == DragActionType.Copy
                           && list.Contains(dragContext.Item)) return;

            if (dragContext.DragAction == DragActionType.Move)
            {
                if (dragItem.List != list)
                {
                    dragItem.List.Remove(dragItem.List.FirstOrDefault(x => x.IsDropPreview == true));
                    dragItem.List = list;
                }
            }

            if (dragContext.DragAction == DragActionType.Move
                || dragContext.DragAction == DragActionType.Copy)
            {
                ADragVM fakeItem = list.FirstOrDefault(x => x.IsDropPreview == true);
                if (fakeItem == null)
                {
                    var clone = dragItem.Clone() as ADragVM;
                    clone.IsDropPreview = true;
                    list.Add(clone);
                }
            }
        }

        ObservableCollection<ADragVM> blockedList = null;

        private void ItemsControl_DragLeave(object sender, DragEventArgs e)
        {
            if (_dragdropWindow != null)
            {
                 ItemsControl control = sender as  ItemsControl;
                 ObservableCollection<ADragVM> list = control.DataContext as ObservableCollection<ADragVM>;

                 if (blockedList == list)
                 {
                     blockedList = null;
                     return;
                 }

                 control.BorderThickness = new Thickness(0);
                 
                 var dragContext = this._dragdropWindow.DataContext as DragContext;
                 var dragItem = dragContext.Item;

                 if (dragContext.SourceList != list)
                 {
                     list.Remove(list.FirstOrDefault(x => x.IsDropPreview == true));
                 }
            }
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
}
