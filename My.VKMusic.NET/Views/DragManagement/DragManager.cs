using My.VKMusic.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace My.VKMusic.Views.DragManagement
{
    public class DragManager
    {

        private Window dragWindow;
        private ObservableCollection<ADragVM> blockedList = null;

        public void OnDragStart(FrameworkElement visual)
        {
            visual.GiveFeedback += DragManager_GiveFeedback;
            CreateDragDropWindow(visual);
            DragDropEffects res = DragDrop.DoDragDrop(visual, visual.DataContext, DragDropEffects.Move);
            CloseDragWindow(false);
        }

        private void DragManager_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            CursorHelper.GetCursorPos(ref w32Mouse);

            dragWindow.Left = w32Mouse.X - (sender as FrameworkElement).ActualWidth - 10;
            dragWindow.Top = w32Mouse.Y + 10;
        }

        private void CreateDragDropWindow(FrameworkElement dragElement)
        {
            this.dragWindow = new Window();
            dragWindow.WindowStyle = WindowStyle.None;
            dragWindow.AllowsTransparency = true;
            dragWindow.AllowDrop = false;
            dragWindow.Background = null;
            dragWindow.IsHitTestVisible = false;
            dragWindow.Opacity = 0.8;
            dragWindow.SizeToContent = SizeToContent.WidthAndHeight;
            dragWindow.Topmost = true;
            dragWindow.ShowInTaskbar = false;
            dragWindow.Cursor = Cursors.Hand;

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            this.dragWindow.Content = r;

            Win32Point w32Mouse = new Win32Point();
            CursorHelper.GetCursorPos(ref w32Mouse);

            this.dragWindow.Left = w32Mouse.X;
            this.dragWindow.Top = w32Mouse.Y;
            this.dragWindow.Show();

            var dragItem = dragElement.DataContext as ADragVM;
            this.dragWindow.DataContext = new DragContext(dragItem, dragItem.List, DragActionType.Reorder);
        }

        void CloseDragWindow(bool success)
        {
            if (dragWindow != null)
            {
                var dragContext = this.dragWindow.DataContext as DragContext;
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

                dragWindow.Close();
                dragWindow = null;
            }
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            var dragContext = dragWindow.DataContext as DragContext;
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

        public void OnDragEnter(object sender, DragEventArgs e)
        {
            if (sender is DockPanel)
            {
                DockPanel dragControl = sender as DockPanel;
                ADragVM item = dragControl.DataContext as ADragVM;
                var thisList = item.List;

                if (!item.IsDropPreview)
                {
                    var dragContext = this.dragWindow.DataContext as DragContext;
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
                return;
            }
            if (sender is ItemsControl)
            {
                var dragContext = this.dragWindow.DataContext as DragContext;
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

        public void OnDragLeave(object sender, DragEventArgs e)
        {
            if (sender is ItemsControl)
            {
                if (dragWindow != null)
                {
                    ItemsControl control = sender as ItemsControl;
                    ObservableCollection<ADragVM> list = control.DataContext as ObservableCollection<ADragVM>;

                    if (blockedList == list)
                    {
                        blockedList = null;
                        return;
                    }

                    control.BorderThickness = new Thickness(0);

                    var dragContext = this.dragWindow.DataContext as DragContext;
                    var dragItem = dragContext.Item;

                    if (dragContext.SourceList != list)
                    {
                        list.Remove(list.FirstOrDefault(x => x.IsDropPreview == true));
                    }
                }
            }
        }
    }
}
