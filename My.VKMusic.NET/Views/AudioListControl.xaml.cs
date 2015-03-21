using My.VKMusic.Views.DragManagement;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for AudioListControl.xaml
    /// </summary>
    public partial class AudioListControl : UserControl
    {

        public DragManager DragManager
        {
            get { return (DragManager)this.GetValue(DragManagerProperty); }
            set { this.SetValue(DragManagerProperty, value); }
        }

        public static readonly DependencyProperty DragManagerProperty = DependencyProperty.Register(
          "DragManager", typeof(DragManager), typeof(AudioListControl), new PropertyMetadata(null, changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioListControl).DragManager = e.NewValue as DragManager;
        }

        public MouseButtonEventHandler OnDrag
        {
            get { return (MouseButtonEventHandler)this.GetValue(OnDragProperty); }
            set { this.SetValue(OnDragProperty, value); }
        }

        public static readonly DependencyProperty OnDragProperty = DependencyProperty.Register(
          "OnDrag", typeof(MouseButtonEventHandler), typeof(AudioListControl), new PropertyMetadata(null, ondragchanged));

        private static void ondragchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioListControl).OnDrag = e.NewValue as MouseButtonEventHandler;
        }

        public ICommand PlayAudioCommand
        {
            get { return (ICommand)this.GetValue(PlayAudioCommandProperty); }
            set { this.SetValue(PlayAudioCommandProperty, value); }
        }

        public static readonly DependencyProperty PlayAudioCommandProperty = DependencyProperty.Register(
          "PlayAudioCommand", typeof(ICommand), typeof(AudioListControl), new PropertyMetadata(null, PlayAudioCommandchanged));

        private static void PlayAudioCommandchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioListControl).PlayAudioCommand = e.NewValue as ICommand;
        }

        public ICommand EditAudioCommand
        {
            get { return (ICommand)this.GetValue(EditAudioCommandProperty); }
            set { this.SetValue(EditAudioCommandProperty, value); }
        }

        public static readonly DependencyProperty EditAudioCommandProperty = DependencyProperty.Register(
          "EditAudioCommand", typeof(ICommand), typeof(AudioListControl), new PropertyMetadata(null, EditAudioCommandchanged));

        private static void EditAudioCommandchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioListControl).EditAudioCommand = e.NewValue as ICommand;
        }

        public ICommand DeleteAudioCommand
        {
            get { return (ICommand)this.GetValue(DeleteAudioCommandProperty); }
            set { this.SetValue(DeleteAudioCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteAudioCommandProperty = DependencyProperty.Register(
          "DeleteAudioCommand", typeof(ICommand), typeof(AudioListControl), new PropertyMetadata(null, DeleteAudioCommandchanged));

        private static void DeleteAudioCommandchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AudioListControl).DeleteAudioCommand = e.NewValue as ICommand;
        }


        public AudioListControl()
        {
            InitializeComponent();
        }

        private void AudioItem_DragEnter(object sender, DragEventArgs e)
        {
            this.DragManager.OnDragEnter(sender, e);
        }

        private void AudioItem_Drop(object sender, DragEventArgs e)
        {
            this.DragManager.OnDrop(sender, e);
        }

        private void ItemsControl_DragLeave(object sender, DragEventArgs e)
        {
            this.DragManager.OnDragLeave(sender, e);
        }



    }
}
