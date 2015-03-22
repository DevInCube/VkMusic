using My.VKMusic.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VkNET.Models;

namespace My.VKMusic.Views.DragManagement
{
    public class DragManager
    {

        private Window _dragdropWindow;
        private ObservableCollection<ADragVM> blockedList = null;

        public event EventHandler<AudioReorderEventArgs> Reorder;

        internal  void OnDragStart(object sender)
        {
            FrameworkElement dragControl = sender as FrameworkElement;            
            dragControl.GiveFeedback += dragControl_GiveFeedback;

            ADragVM item = (dragControl.DataContext as ADragVM);
            int index = item.List.IndexOf(item);
            item.List.Remove(item);

            var clone = item.Clone() as ADragVM;
            clone.IsDropPreview = true;
            item.List.Insert(index, clone);
            CreateDragDropWindow(dragControl.Tag as Visual);
            var ret = DragDrop.DoDragDrop(dragControl, dragControl.DataContext, DragDropEffects.Move);
            CloseDragWindow(false);
        }

        void dragControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            this.GiveFeedbackEvent();
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

            SetWindowPos(w32Mouse);
            this._dragdropWindow.Show();

            var dragItem = (dragElement as FrameworkElement).DataContext as ADragVM;
            this._dragdropWindow.DataContext = new DragContext(dragItem, dragItem.List, DragActionType.Reorder);
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

                if (Reorder != null)
                {
                    var list = dragItem.List;
                    int index = list.IndexOf(dragItem);
                    var audio = new AudioFileInfo(); //@todo
                    var before = index > 0 ? new AudioFileInfo() : null; //@todo
                    var after = index < list.Count - 1 ? new AudioFileInfo() : null; //@todo
                    Reorder.Invoke(this, new AudioReorderEventArgs(audio, before, after)); //@todo
                }

                _dragdropWindow.Close();
                _dragdropWindow = null;
            }
        }

        internal void OnDrop(object sender, DragEventArgs e)
        {            
            var dragContext = this._dragdropWindow.DataContext as DragContext;
            var dragItem = dragContext.Item;
            ObservableCollection<ADragVM> list = null;

            if (sender is ItemsControl)
            {
                if (dragContext.DragAction == DragActionType.Reorder &&
                    (sender as ItemsControl).DataContext != dragItem.List) return;

                list = (sender as ItemsControl).DataContext as ObservableCollection<ADragVM>;
                if (!list.Contains(dragItem))
                    list.Add(dragItem);
                else
                {
                    list.Remove(dragItem);
                    list.Add(dragItem);
                }
            }
            if (sender is UserControl)
            {
                var targetElem = sender as UserControl;
                var targetItem = targetElem.DataContext as ADragVM;

                if (dragContext.DragAction == DragActionType.Reorder &&
                    targetItem.List != dragItem.List) return;

                list = targetItem.List;
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

        internal void OnDragLeave(object sender, DragEventArgs e)
        {
            if (_dragdropWindow != null)
            {
                ItemsControl control = sender as ItemsControl;
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

        internal void OnDragEnter(object sender, DragEventArgs e)
        {
            if (sender is UserControl)
            {
                UserControl dragControl = sender as UserControl;
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
                        if (dragContext.DragAction == DragActionType.Copy
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
            if (sender is ItemsControl)
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
        }

        internal void GiveFeedbackEvent()
        {
            Win32Point w32Mouse = new Win32Point();
            CursorHelper.GetCursorPos(ref w32Mouse);

            SetWindowPos(w32Mouse);
        }

        private void SetWindowPos(Win32Point w32Mouse)
        {
            this._dragdropWindow.Left = w32Mouse.X - _dragdropWindow.ActualWidth - 10;
            this._dragdropWindow.Top = w32Mouse.Y + 10;
        }

    }
}
