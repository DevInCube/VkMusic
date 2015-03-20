using My.VKMusic.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for AudioItem.xaml
    /// </summary>
    public partial class AudioItem : UserControl
    {

        Brush artistFG = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B587A"));
        Brush currentColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#45668e"));
        Brush hoverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1E7ED"));
        private Window _dragdropWindow;

        public AudioItem()
        {
            InitializeComponent();

            this.DataContextChanged += AudioItem_DataContextChanged;
            item.MouseEnter += item_MouseEnter;
            item.MouseLeave += item_MouseLeave;
            this.MouseUp += AudioItem_MouseUp;

            this.PreviewMouseLeftButtonDown += AudioItem_PreviewMouseLeftButtonDown;            
            this.Drop += AudioItem_Drop;            
            //this.GiveFeedback += AudioItem_GiveFeedback;
        }

        void AudioItem_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this._dragdropWindow.Left = w32Mouse.X+20;
            this._dragdropWindow.Top = w32Mouse.Y+20;
        }

        void AudioItem_Drop(object sender, DragEventArgs e)
        {
            var droppedData = e.Data.GetData(typeof(AudioItem)) as AudioItem;
            var target = (sender as AudioItem).DataContext as AudioItem;

            /*int targetIndex = CardListControl.Items.IndexOf(target);

            droppedData.Effect = null;
            droppedData.RenderTransform = null;

            Items.Remove(droppedData);
            Items.Insert(targetIndex, droppedData);*/

            // remove the visual feedback drag and drop item
            
        }

        void AudioItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is AudioItem)
            {
                //AudioItem draggedItem = sender as AudioItem;
                //CreateDragDropWindow(draggedItem);
                //DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                //draggedItem.IsSelected = true;
            }
        }

        private void CreateDragDropWindow(Visual dragElement)
        {
            this._dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
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

        void AudioItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //(this.DataContext as AudioFile).Play();
        }

        void AudioItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AudioFile file = this.DataContext as AudioFile;
            if (file != null)
                file.PropertyChanged += file_PropertyChanged;
        }

        void file_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected"))
            {
                item.Background = (this.DataContext as AudioFile).IsSelected ? currentColor : Brushes.WhiteSmoke;
                artistLabel.Foreground = (this.DataContext as AudioFile).IsSelected ? Brushes.White : artistFG;
                titleLabel.Foreground = divLabel.Foreground =
                    (this.DataContext as AudioFile).IsSelected ? Brushes.White : Brushes.Black;
            }
        }

        void item_MouseLeave(object sender, MouseEventArgs e)
        {
            //item.Background = (this.DataContext as AudioFile).IsSelected ? currentColor : Brushes.WhiteSmoke;
            //Cursor = Cursors.Arrow;
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            //item.Background = (this.DataContext as AudioFile).IsSelected ? currentColor : hoverColor;
            //Cursor = Cursors.Hand;
        }
    }
}
