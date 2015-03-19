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
        public ObservableCollection<TestItem> Items1 { get; set; }
        public ObservableCollection<TestItem> Items2 { get; set; }

        public TestDDWindow()
        {
            InitializeComponent();            

            this.Items1 = new ObservableCollection<TestItem>()
            {
                new TestItem(1),
                new TestItem(2),
                new TestItem(3),
            };
            this.Items2 = new ObservableCollection<TestItem>()  {
                new TestItem(4),                
                new TestItem(5),
            };

            this.DataContext = this;
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
                CreateDragDropWindow(dragControl.Parent as Visual);
                var ret = DragDrop.DoDragDrop(dragControl, dragControl.DataContext, DragDropEffects.Move);
                if (ret == DragDropEffects.None)
                {
                    if (_dragdropWindow != null)
                    {
                        _dragdropWindow.Close();
                        _dragdropWindow = null;
                    }
                }
            }
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            var droppedData = e.Data.GetData(typeof(TestItem)) as TestItem;

            if (sender is ItemsControl)
            {
                var target = (sender as ItemsControl).DataContext as ObservableCollection<TestItem>;
                if (!target.Contains(droppedData))
                    target.Add(droppedData);                
                if (_dragdropWindow != null)
                {
                    _dragdropWindow.Close();
                    _dragdropWindow = null;
                }
            }
            if (sender is DockPanel)
            {
                var targetElem = sender as DockPanel;
                int index = Items1.IndexOf(targetElem.DataContext as TestItem);
                Items1.Remove(droppedData);
                Items1.Insert(index, droppedData);
            }
        }

        private void Label_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

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
            _dragdropWindow.Opacity = 0.75;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            this._dragdropWindow.Content = r;

            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
            this._dragdropWindow.Show();
    
        }

       
    }

    public class TestItem
    {
        private int i;
        public TestItem(int i)
        {
            this.i = i;
        }
        public override string ToString()
        {
            return i.ToString();
        }
    }
}
