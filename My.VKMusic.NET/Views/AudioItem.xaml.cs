using My.VKMusic.ViewModels;
using My.VKMusic.Views.DragManagement;
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

        public MouseButtonEventHandler OnDrag
        {
            get { return (MouseButtonEventHandler)this.GetValue(OnDragProperty); }
            set { this.SetValue(OnDragProperty, value); }
        }

        public static readonly DependencyProperty OnDragProperty = DependencyProperty.Register(
          "OnDrag", typeof(MouseButtonEventHandler), typeof(AudioItem), new PropertyMetadata(null, changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioItem).drag.PreviewMouseLeftButtonDown += e.NewValue as MouseButtonEventHandler;
        }

        public ICommand PlayAudioCommand
        {
            get { return (ICommand)this.GetValue(PlayAudioCommandProperty); }
            set { this.SetValue(PlayAudioCommandProperty, value); }
        }

        public static readonly DependencyProperty PlayAudioCommandProperty = DependencyProperty.Register(
          "PlayAudioCommand", typeof(ICommand), typeof(AudioItem), new PropertyMetadata(null, PlayAudioCommandchanged));

        private static void PlayAudioCommandchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioItem).PlayAudioCommand = e.NewValue as ICommand;
        }

        public ICommand EditAudioCommand
        {
            get { return (ICommand)this.GetValue(EditAudioCommandProperty); }
            set { this.SetValue(EditAudioCommandProperty, value); }
        }

        public static readonly DependencyProperty EditAudioCommandProperty = DependencyProperty.Register(
          "EditAudioCommand", typeof(ICommand), typeof(AudioItem), new PropertyMetadata(null, EditAudioCommandchanged));

        private static void EditAudioCommandchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioItem).EditAudioCommand = e.NewValue as ICommand;
        }

        public ICommand DeleteAudioCommand
        {
            get { return (ICommand)this.GetValue(DeleteAudioCommandProperty); }
            set { this.SetValue(DeleteAudioCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteAudioCommandProperty = DependencyProperty.Register(
          "DeleteAudioCommand", typeof(ICommand), typeof(AudioItem), new PropertyMetadata(null, DeleteAudioCommandchanged));

        private static void DeleteAudioCommandchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioItem).DeleteAudioCommand = e.NewValue as ICommand;
        }

        public bool CanReorder
        {
            get { return (bool)this.GetValue(CanReorderProperty); }
            set { this.SetValue(CanReorderProperty, value); }
        }

        public static readonly DependencyProperty CanReorderProperty = DependencyProperty.Register(
          "CanReorder", typeof(bool), typeof(AudioItem), new PropertyMetadata(true, CanReorderChanged));

        private static void CanReorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioItem).CanReorder = bool.Parse(e.NewValue.ToString());            
        }

        Brush artistFG = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B587A"));
        Brush currentColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#45668e"));
        Brush hoverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1E7ED"));

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
            var context = this.DataContext as ADragVM;
            context.IsMouseHover = false;
            item.Background = context.IsSelected ? currentColor : Brushes.WhiteSmoke;
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            var context = this.DataContext as ADragVM;
            context.IsMouseHover = true;
            item.Background = context.IsSelected ? currentColor : hoverColor;
        }
    }
}
