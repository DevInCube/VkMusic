using My.VKMusic.Views.DragManagement;
using System;
using System.Collections;
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


        public AudioListControl()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragManager.OnDragStart((sender as Label).Parent as FrameworkElement);
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            DragManager.OnDrop(sender, e);
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
}
